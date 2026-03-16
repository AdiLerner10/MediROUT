using BlApi;
using BO;
using DalApi;
using DO;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Helpers;

internal static class OrderManager
{
    internal static IDal s_dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new();

    public static double PI = 3.141592653589793238462643383279502884197169;
    public static double R = 6731;


    /// <summary>
    /// a function that gets the whole deliveries and returns them as OrderInList list.
    /// </summary>
    /// <returns></returns>
    internal static async Task<IEnumerable<BO.OrderInList>> GetOrdersInList()
    {
        Dictionary<int, List<Delivery>> deliveries = new(DeliveryManager.GetDoDeliveries()
                                   .GroupBy(d => d.OrderId)
                                   .ToDictionary(k => k.Key, v => v.Select(d => d).ToList())//no reason for lazy collection because we need the count and the last and it's better to use Count property than the function
                                   );

        string companyAddressStr;
        lock (AdminManager.BlMutex)
            companyAddressStr = s_dal.Config.CompanyAddress;

        Location companyAddress = await GetLocationCalcValues(s_dal.Config.CompanyAddress);
        var orders = OrderManager.GetDoOrders().Select(order => OrderManager.ConvertToOrderInList(order, companyAddress, deliveries.FirstOrDefault(g => g.Key == order.Id).Value));
        return orders;
    }

    /// <summary>
    /// a function that returns the Orders list from the Dal layer.
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<DO.Order> GetDoOrders()
    {
        lock (AdminManager.BlMutex)
            return s_dal.Order.ReadAll();
    }

    /// <summary>
    /// a function that returns the whole list of orders of BO.Orders type.
    /// </summary>
    /// <returns></returns>
    internal static IEnumerable<BO.Order> GetOrders()
    {
        try
        {
            var deliveries = DeliveryManager.GetDoDeliveries();
            lock (AdminManager.BlMutex)
                return s_dal.Order.ReadAll()
                   .Select(o => OrderManager.ConvertToBO(o, deliveries));
            //var orders = s_dal.Order.ReadAll();
            //ConcurrentBag<BO.Order> safeList = new();
            //Parallel.ForEach(orders, o =>
            //{
            //    safeList.Add(ConvertToBO(o));
            //});
            //return safeList;
            //return orders.Select(ConvertToBO).ToList();
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }

    /// <summary>
    /// a function that cast a DO.Order object to a BO.Order object
    /// </summary>
    /// <param name="order">the object of DO.Order type, that should be casted</param>
    /// <returns></returns>
    public static BO.Order ConvertToBO(DO.Order order, IEnumerable<DO.Delivery> deliveries)
    {
        var deliveriesPerOrder = deliveries
            .Where(d => d.OrderId == order.Id)
            .OrderBy(d => d.DeliveryStartTime)
            .ToList();
        var last = deliveriesPerOrder.LastOrDefault();

        BO.OrderStatus status = last != null ? (BO.OrderStatus)last.OrderStatus : BO.OrderStatus.Pending;
        var result = new BO.Order(order, last, status)
        {
            Id = order.Id,
            DeliveryPerOrderInList = deliveriesPerOrder
            .Select(d => DeliveryManager.ConvertToDeliveryPerOrderInList(d, order))
            .ToList()
        };

        return result;
    }



    /// <summary>
    /// a function that cast a DO.Order object to a BO.Order object
    /// </summary>
    /// <param name="order">the object of DO.Order type, that should be casted</param>
    /// <returns></returns>
    public static BO.OrderInList ConvertToOrderInList(DO.Order order, Location companyLoc, List<DO.Delivery>? deliveries)
    {
        Location orderLoc = new Location() { Latitude = order.Latitude, Longitude = order.Longitude };


        var lastDelivery = deliveries?.LastOrDefault();
        TimeSpan remainingTime = RemainingTimeCalculation(order);
        var boOrder = new BO.OrderInList(order)
        {
            NumberOfDeliveries = deliveries?.Count ?? 0,
            DeliveryId = lastDelivery?.Id ?? 0,
            RemainingTime = remainingTime,
            ScheduleStatus = CalculateScheduleStatus(
                remainingTime,
                lastDelivery is null ? DO.OrderStatus.Pending : (DO.OrderStatus)lastDelivery!.OrderStatus),
            AirDistance = AirDistanceCalculation(orderLoc, companyLoc)
        };

        return boOrder;
    }

    /// <summary>
    /// a function that calculate the remaining time by the order start time and the expected end time.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    private static TimeSpan RemainingTimeCalculation(DO.Order order)
    {
        TimeSpan remainingTime = ((AdminManager.GetConfig().MaxTimeWindow) - (AdminManager.Now - order.OrderStartTime)) ?? AdminManager.GetConfig().MaxTimeWindow;
        return remainingTime;
    }

    internal static void PeriodicOrdersUpdates(DateTime oldClock, DateTime newClock)
    {

    }

    /// <summary>
    /// a function that adds an order to the DB
    /// </summary>
    /// <param name="newOrder"></param>
    /// <exception cref="BlAlreadyExistException"></exception>
    /// <exception cref="blFunctionIsNull"></exception>
    internal static async Task Add(BO.Order newOrder)
    {
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Order.Create(ConvertToDO(newOrder));
            Observers.NotifyListUpdated();
        }
        catch (BlAlreadyExistException ex)
        {
            throw new BlAlreadyExistException(ex.Message, ex);
        }

        try
        {
            Location compLoc = await GetLocationCalcValues(AdminManager.GetConfig().CompanyAddress);
            Location orderLoc = await GetLocationCalcValues(newOrder.Address);
            IEnumerable<DO.Courier>? couriers;
            lock (AdminManager.BlMutex)
                couriers = s_dal.Courier.ReadAll();
            // 1. Create a list of tasks that evaluate the condition for each courier
            var courierFilteringTasks = couriers.Select(async c =>
            {
                double speed = Tools.SpeedCalculation((BO.Vehicle)c.VehicleType);
                var distanceKm = await CalculateRoute(compLoc, orderLoc);
                bool isWithinDistance = distanceKm <= c.MaxPersonalDistance;
                return (Courier: c, IsWithinDistance: isWithinDistance);
            });

            // 2. Wait for all tasks to complete and then filter the results synchronously
            var filteringResults = await Task.WhenAll(courierFilteringTasks);
            var couriersToNotify = filteringResults
                            .Where(res => res.IsWithinDistance)
                            .Select(res => res.Courier)
                            .ToList();

            foreach (var courier in couriersToNotify)
            {
                try
                {
                    await Tools.EmailService.Send(
                        courier.Email,
                        "New Order Available Near You",
                        $"Hello {courier.Name},\n\n" +
                        $"A new order is now available within your delivery area.\n\n" +
                        $"Order ID: {newOrder.Id}\n" +
                        $"Address: {newOrder.Address}\n" +
                        $"Log into the courier system to accept the order.\n\n" +
                        $"Delivery Management System"
                    );
                }
                catch (Exception ex)
                {
                }
            }
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }

    /// <summary>
    /// a function that convert a BO Order entity to DO order entity
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public static DO.Order ConvertToDO(BO.Order order)
    {
        return new DO.Order
        {
            Id = order.Id,
            TypeOfOrder = (DO.OrderType)order.OrderType,
            inOrOut = (DO.InOrOut)order.InOrOut,
            Description = order.Description,
            Address = order.Address,
            Latitude = order.Latitude,
            Longitude = order.Longitude,
            Name = order.ClientName,
            PhoneNumber = order.ClientPhoneNumber,
            Fragile = order.Fragile,
            volume = order.Volume,
            OrderStartTime = (DateTime)order.OrderOpenedTime
        };
    }

    /// <summary>
    /// a function that convert a BO order entity to OrderInList entity
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    internal static OrderInList ConvertToOrderInList(BO.Order order)
    {
        return new OrderInList(order);
    }

    /// <summary>
    /// gets an order by an order id 
    /// </summary>
    /// <param name="orderId">id to get </param>
    /// <returns></returns>
    /// <exception cref="BlDoesNotExistException">throws doesnt exist exception</exception>
    internal static BO.Order GetOrder(int orderId)
    {
        DO.Order doOrder;
        lock (AdminManager.BlMutex)
            doOrder = s_dal.Order.Read(orderId) ?? throw new BlDoesNotExistException($"Order with ID={orderId} does not exist");
        var deliveries = DeliveryManager.GetDoDeliveries().Where(d => d.OrderId == orderId);
        return ConvertToBO(doOrder, deliveries);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idToCancel"></param>
    /// <returns></returns>
    internal static bool CanBeCanceled(int idToCancel)
    {
        Delivery? delivery;
        lock (AdminManager.BlMutex)
            delivery = s_dal.Delivery.ReadAll().FirstOrDefault(delivery => delivery.OrderId == idToCancel);
        if (delivery == null)
            return true;
        if (delivery.OrderStatus == DO.OrderStatus.Delivered || delivery.OrderStatus == DO.OrderStatus.Canceled)//checks if order was delivered
            return false;
        return true;
    }

    /// <summary>
    /// cancels an order 
    /// </summary>
    /// <param name="IdToCancel">id of the order we want to cancel</param>
    /// <exception cref="BlDoesNotExistException">doesnt exist exception</exception>
    /// <exception cref="BO.IllegalRequestException">Illegal request exception</exception>
    internal static async Task CancelOrder(int IdToCancel)
    {
        DO.Order doOrder;
        Delivery? delivery;
        lock (AdminManager.BlMutex)
        {
            doOrder = s_dal.Order.Read(IdToCancel) ?? throw new BlDoesNotExistException($"Order with ID={IdToCancel} does not exist");//checks if the order exists
            delivery = s_dal.Delivery.ReadAll().FirstOrDefault(delivery => delivery.OrderId == IdToCancel);
        }
        if (delivery != null)//checks if it is opened for a delivery
        {
            if (!CanBeCanceled(IdToCancel))//checks if order was delivered
                return;
            DO.Delivery updatedDelivery = new DO.Delivery
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                CourierId = delivery.CourierId,
                DeliveryStartTime = delivery.DeliveryStartTime,
                EndTime = AdminManager.Now,
                OrderStatus = DO.OrderStatus.Canceled
            };
            var courier = CourierManager.GetCourier(delivery.CourierId);
            var deliveries = DeliveryManager.GetDoDeliveries();


            var orderBO = new BO.Order(doOrder, delivery, BO.OrderStatus.Canceled);

            try
            {
                await Tools.EmailService.Send(
                     courier.Email,
                     "Order Cancellation Notice",
                     $"Hello {courier.Name},\n\n" +
                     $"The order you were delivering has been cancelled.\n\n" +
                     $"Order ID: {orderBO.Id}\n" +
                     $"Address: {orderBO.Address}\n\n" +
                     $"You may return to available status.\n\n" +
                     $"Delivery Management System"
                 );
            }
            catch (Exception ex)
            {
                throw new blEmailFailedException("failed to send email");
            }

            try
            {
                lock (AdminManager.BlMutex)
                    s_dal.Delivery.Update(updatedDelivery);
                Observers.NotifyItemUpdated(IdToCancel);
                Observers.NotifyListUpdated();
            }
            catch (DalDoesNotExistException ex)
            {
                throw new BlDoesNotExistException(ex.Message, ex);
            }

        }
        else
        {
            DO.Delivery dummyDelivery = new DO.Delivery
            {
                Id = IdToCancel,
                OrderId = IdToCancel,
                CourierId = 0,
                DeliveryStartTime = AdminManager.Now,
                EndTime = AdminManager.Now,
                OrderStatus = DO.OrderStatus.Canceled,

            };

            try
            {
                lock (AdminManager.BlMutex)
                    s_dal.Delivery.Create(dummyDelivery);
                Observers.NotifyItemUpdated(IdToCancel);
                Observers.NotifyListUpdated();
            }
            catch (DalAlreadyExistException ex)
            {
                throw new BlAlreadyExistException(ex.Message, ex);
            }
        }
    }

    /// <summary>
    /// a function that update the order by the manager in the DO.
    /// </summary>
    /// <param name="existingOrder"></param>
    internal static void Update(BO.Order existingOrder)
    {
        DO.Order order = ConvertToDO(existingOrder);
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Order.Update(order);
            Observers.NotifyItemUpdated(existingOrder.Id);
            Observers.NotifyListUpdated();
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(ex.Message, ex);
        }

    }

    /// <summary>
    /// a function that returns the whole open Orders in the distance that the sent courier agree to deliver in.
    /// </summary>
    /// <param name="courierId">the id of the sent courier</param>
    /// <param name="filter">the filter type</param>
    /// <param name="sort">the sort type</param>
    /// <returns></returns>
    internal static async Task<IEnumerable<OpenOrderInList>> GetOpenOrders(int courierId, BO.OrderType? filter, OpenOrderProperty? sort)
    {
        var courier = CourierManager.GetCourier(courierId);

        var openOrders = GetOrders();

        Location companyLoc = await GetLocationCalcValues(s_dal.Config.CompanyAddress);
        var ordersTasks = from order in openOrders
                          where Tools.IsOpen(order)
                          select (ConvertToOpenOrderInList(order, courierId, companyLoc));

        var orders = await Task.WhenAll(ordersTasks);
        IEnumerable<OpenOrderInList>? lazyOrders = orders.AsEnumerable();

        lazyOrders = lazyOrders.Where(order => order.ActualDistance <= courier.MaxPersonalDistance);
        if (filter != null)
            lazyOrders = lazyOrders.Where(order => FilterBy(filter, order));
        //from order in orders
        //     where filterBy(filter, order)
        //     select order;
        if (sort != null)
            lazyOrders = lazyOrders.OrderBy(order => SortBy(sort, order));
        //orders = from order in orders
        //         orderby sortBy(sort, order)
        //         select order;

        return lazyOrders;
    }

    /// <summary>
    /// a function that convert an order entity to OpenOrderInList entity
    /// </summary>
    /// <param name="order"></param>
    /// <param name="courierId"></param>
    /// <param name="companyLoc"></param>
    /// <returns></returns>
    private static async Task<OpenOrderInList> ConvertToOpenOrderInList(BO.Order order, int courierId, Location companyLoc)
    {
        Location clientLoc = new Location { Latitude = order.Latitude, Longitude = order.Longitude };

        var courier = CourierManager.GetCourier(courierId);

        double result = 0;

        double speed = Tools.SpeedCalculation(courier.Vehicle);
        try
        {
            if (order.InOrOut == BO.InOrOut.In)
            {
                result = await CalculateRoute(clientLoc, companyLoc);

            }
            else
            {
                result = await CalculateRoute(companyLoc, clientLoc);
            }
        }
        catch
        {
            result = AirDistanceCalculation(clientLoc, companyLoc);
        }
        if(result==0)
            result = AirDistanceCalculation(clientLoc, companyLoc);

        OpenOrderInList newOrder = new OpenOrderInList
        {
            Address = order.Address,
            AirDistance = order.AirDistance,
            TimeEstimate = TimeSpan.FromHours(result/speed),
            ActualDistance = result,
            CourierId = courierId,
            Fragile = order.Fragile,
            OrderType = order.OrderType,
            OrderId = order.Id,
            Volume = order.Volume,
            MaxDeliveryTime = (DateTime)order.ExpectedDeliveryTime,
            RemainingTime = (TimeSpan)order.RemainingTime,
            ScheduleStatus = order.ScheduleStatus,
        };
        return newOrder;
    }

    /// <summary>
    /// a function that check if the order natch the sent filter OrderType. 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    private static bool FilterBy(BO.OrderType? filter, BO.OpenOrderInList order)
    {
        return order.OrderType == filter;
    }

    /// <summary>
    /// a function that return the value of the order by the sent type.
    /// </summary>
    /// <param name="sort"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    private static object SortBy(OpenOrderProperty? sort, OpenOrderInList order)
    {
        switch (sort)
        {
            case OpenOrderProperty.OrderId:
                return order.OrderId;
            case OpenOrderProperty.OrderType:
                return order.OrderType;
            case OpenOrderProperty.Address:
                return order.Address;
            case OpenOrderProperty.Fragile:
                return order.Fragile;
            case OpenOrderProperty.RemainingTime:
                return order.RemainingTime;
            case OpenOrderProperty.ActualDistance:
                return order.ActualDistance;
            case OpenOrderProperty.AirDistance:
                return order.AirDistance;
            case OpenOrderProperty.MaxDeliveryTime:
                return order.MaxDeliveryTime;
            case OpenOrderProperty.ScheduleStatus:
                return order.ScheduleStatus;
            case OpenOrderProperty.TimeEstimate:
                return order.TimeEstimate;
            case OpenOrderProperty.Volume:
                return order.Volume;
            default:
                return null;
        }
    }

    /// <summary>
    /// a function that returns the whole closed orders by their status
    /// </summary>
    /// <param name="id"></param>
    /// <param name="courierId"></param>
    /// <param name="filter"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    internal static IEnumerable<ClosedDeliveryInList> GetClosedOrders(int id, int courierId, BO.OrderType? filter, ClosedDeliveryProperty? sort)
    {
        IEnumerable<Delivery> deliveries;
        if (filter == null || filter == BO.OrderType.None)
            deliveries = DeliveryManager.GetDoDeliveries().Where(d => d.OrderStatus != DO.OrderStatus.InProcess);
        else
        {
            deliveries = DeliveryManager.GetDoDeliveries().Where(delivery => OrderManager.GetOrder(delivery.OrderId).OrderType == filter).Where(d => d.OrderStatus != DO.OrderStatus.InProcess); ;
        }
        if (sort == null)
        {
            deliveries = deliveries.OrderBy(delivery => delivery.OrderStatus);
        }
        deliveries = deliveries.Where(delivery => delivery.CourierId == courierId);
        var closedDeliveries = from delivery in deliveries
                               select new ClosedDeliveryInList
                               {
                                   ActualDistance = delivery.Distance,
                                   Address = OrderManager.GetOrder(delivery.OrderId).Address,
                                   DeliveryId = delivery.Id,
                                   DeliveryTime = (TimeSpan)(delivery.EndTime - delivery.DeliveryStartTime),//change to timespan?
                                   OrderId = delivery.OrderId,
                                   OrderStatus = OrderManager.GetOrder(delivery.OrderId).OrderStatus,
                                   OrderType = OrderManager.GetOrder(delivery.OrderId).OrderType,
                                   VehicleType = (BO.Vehicle)delivery.VehicleType
                               };

        if (sort != null)
        {
            closedDeliveries = closedDeliveries.OrderBy(delivery => DeliveryManager.SortTypeDelivery(delivery, (ClosedDeliveryProperty)sort));
        }

        return closedDeliveries;
    }

    /// <summary>
    /// a function that returns an array that contain the numbers of orders from avery type of status.
    /// </summary>
    /// <returns></returns>
    internal static int[] OrderAmountByStatus()
    {
        var ordersByStatus = OrderManager.GetOrders();
        var ordersGroup = from order in ordersByStatus
                          group order by order.OrderStatus into groupType
                          select groupType;

        int[] ordersGroupsCount = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
        foreach (var order in ordersGroup)
        {
            ordersGroupsCount[(int)(order.Key)] = order.Count();
        }

        return ordersGroupsCount;
    }

    /// <summary>
    /// a function that take care of an order that been chosen by a courier, and also take care of the order status, the courier status, and the delivery status.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="courierId">the sent courier id</param>
    /// <param name="orderId">the sent order id to ba handled</param>
    /// <exception cref="BlIllegalRequestException"></exception>
    /// <exception cref="blEmailFailedException"></exception>
    internal static async Task OrderToHandle(int id, int courierId, int orderId)
    {
        var order = GetOrder(orderId);
        if (!Tools.IsOpen(order))
            throw new BlIllegalRequestException("can't handle this order anymore");
        if (!(await Tools.CheckDistance(courierId, orderId)))
            throw new BlIllegalRequestException("Distance is further then the courier maximun distance");


        order.DeliveryPerOrderInList.Add(await DeliveryManager.Create(order, courierId));
        order.OrderStatus = BO.OrderStatus.InProcess;

        OrderManager.Update(order);

        CourierManager.Observers.NotifyItemUpdated(courierId);
        CourierManager.Observers.NotifyListUpdated();
        Observers.NotifyItemUpdated(orderId);
        Observers.NotifyListUpdated();




        var courier = CourierManager.GetCourier(courierId);
        try
        {
            await Tools.EmailService.Send(
                 courier.Email,
                 "Order Assigned to You",
                 $"Hello {courier.Name},\n\n" +
                 $"You have been assigned to a new order.\n\n" +
                 $"Order ID: {order.Id}\n" +
                 $"Client Name: {order.ClientName}\n" +
                 $"Client Phone: {order.ClientPhoneNumber}\n" +
                 $"Address: {order.Address}\n" +
                 $"Description: {order.Description}\n" +
                 $"Fragile: {order.Fragile}\n" +
                 $"Volume: {order.Volume}\n\n" +
                 $"Please proceed with the delivery.\n\n" +
                 $"Delivery Management System"
             );
        }
        catch (Exception ex)
        {

            throw new blEmailFailedException("failed to send email");
        }

    }

    /// <summary>
    /// a function that calculate the schedule status of an order by its remaining time, and its status.
    /// </summary>
    /// <param name="remainingTime"></param>
    /// <param name="orderStatus"></param>
    /// <returns></returns>
    public static ScheduleStatus CalculateScheduleStatus(TimeSpan remainingTime, DO.OrderStatus orderStatus)
    {
        if (orderStatus == DO.OrderStatus.InProcess || orderStatus == DO.OrderStatus.Pending)
        {
            if (remainingTime <= TimeSpan.Zero)
                return ScheduleStatus.RunningLate;
            if (remainingTime <= AdminManager.GetConfig().RiskRange)
                return ScheduleStatus.InRisk;
        }
        return ScheduleStatus.RunningOnTime;
    }

    /// <summary>
    /// a class that contain latitude an longitude for location.
    /// </summary>
    public class Location
    {
        [JsonPropertyName("lat")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double Latitude { get; set; }

        [JsonPropertyName("lon")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double Longitude { get; set; }
    };

    private static ConcurrentDictionary<string, Location> locationCache = new();
    private static readonly List<string> locationWebsiteTokens = new()
    {
        "pk.be7a3a2d7a587b7e3de08da593f31e71",
        "pk.d9ff36f8256286eea22db39e26961ee5",
        "pk.5effd0ac39b9afa26e40148a92ccd9df"
    };

    private static readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// a function that return Location by given address
    /// </summary>
    /// <param name="address">the sent address</param>
    /// <returns></returns>
    /// <exception cref="blInvalidInputException"></exception>
    public static async Task<Location> GetLocationCalcValues(string address)
    {
        if (locationCache.ContainsKey(address))
            return locationCache[address];

        Location? res;

        // basic input check
        if (string.IsNullOrWhiteSpace(address))
        {
            res = new();
            locationCache.TryAdd(address, res);
            return res;
        }

        int tryNumber = 0;
        HttpResponseMessage? response = null;
        while (tryNumber < locationWebsiteTokens.Count)
        {
            response = await SendGetLocationRequest(address, tryNumber);
            if (!response.IsSuccessStatusCode)
                tryNumber++;
            else
                break;
        }
        if (response is null)
            throw new Exception("Couldn't get a valid response.");
        string json = response.Content.ReadAsStringAsync().Result;
        var data = System.Text.Json.JsonSerializer.Deserialize<Location[]>(json);

        if (data == null || data.Length == 0)
            throw new blInvalidInputException("Address not found.");

        res = data[0];
        locationCache.TryAdd(address, res);
        return res;
    }

    private static async Task<HttpResponseMessage> SendGetLocationRequest(string address, int tryNumber)
    {
        string url =
            $"https://us1.locationiq.com/v1/search.php?key={locationWebsiteTokens[tryNumber]}&q={Uri.EscapeDataString(address)}&format=json&min_time=1";

        return await httpClient.GetAsync(url);//.Result;
    }

    private static async Task<HttpResponseMessage> SendGetRouteRequest(
        double startLon,
        double endLon,
        double startLat,
        double endLat,
        int tryNumber)
    {
        string url = $"https://us1.locationiq.com/v1/directions/driving/{startLon},{startLat};{endLon},{endLat}?key={locationWebsiteTokens[tryNumber]}&overview=false";

        return await httpClient.GetAsync(url);
    }


    private static ConcurrentDictionary<string, double> airDistanceCache = new();
    /// <summary>
    /// calcultes the air distance between two given addresses.
    /// </summary>
    /// <param name="address1">the first address</param>
    /// <param name="address2">the second address</param>
    /// <returns></returns>
    public static double AirDistanceCalculation(Location loc1, Location loc2)
    {
        double lat1 = loc1.Latitude * Math.PI / 180;
        double lon1 = loc1.Longitude * Math.PI / 180;
        double lat2 = loc2.Latitude * Math.PI / 180;
        double lon2 = loc2.Longitude * Math.PI / 180;

        var cacheKey = $"{lat1}_{lon1}-{lat2}_{lon2}";

        if (airDistanceCache.TryGetValue(cacheKey, out double value))
        {
            return value;
        }

        double differenceLat = lat2 - lat1;
        double differenceLon = lon2 - lon1;

        // חישוב Haversine
        double a = Math.Sin(differenceLat / 2) * Math.Sin(differenceLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(differenceLon / 2) * Math.Sin(differenceLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = R * c; // ודא ש-R הוא רדיוס כדור הארץ (בערך 6371 ק"מ)

        airDistanceCache[cacheKey] = distance;
        return distance;
    }


    /// <summary>
    /// a function that complete a delivery and changes the order status to delivered.
    /// </summary>
    /// <param name="courierId"></param>
    /// <param name="orderId"></param>
    /// <exception cref="BlIllegalRequestException"></exception>
    internal static void Completion(int courierId, int orderId, BO.OrderStatus status)
    {
        var delivery = OrderManager.GetOrder(orderId).DeliveryPerOrderInList.LastOrDefault();
        //  .FirstOrDefault(delivery => (delivery.CourierId == courierId && Tools.IsOpen(OrderManager.GetOrder(orderId))));
        if (delivery != null && delivery.CourierId == courierId)
        {
            try
            {
                DeliveryManager.Complete(delivery!.DeliveryId, status);
                Observers.NotifyListUpdated();
                CourierManager.Observers.NotifyListUpdated();
                CourierManager.Observers.NotifyItemUpdated(courierId);
            }
            catch
            {
                Console.WriteLine("couldn't complete the task.");
            }
        }
    }


    private static ConcurrentDictionary<string, double> distanceCache = new();

    /// <summary>
    /// using AI by chatGPT.
    /// </summary>
    public static async Task<double> CalculateRoute(
        Location startAddress,
        Location endAddress)
    {
        // first - get the coardinations from the calculation functions.
        double startLon = startAddress.Longitude;
        double startLat = startAddress.Latitude;

        // get the coardinations from the calculation functions for the end.
        double endLon = endAddress.Longitude;
        double endLat = endAddress.Latitude;

        var cacheKey = $"{startLat}_{startLon}-{endLat}_{endLon}";
        HttpResponseMessage? response = null;
        int resetRetries = 0;
        while (resetRetries < 3)
        {
            int tryNumber = 0;
            while (tryNumber < locationWebsiteTokens.Count)
            {
                if (distanceCache.ContainsKey(cacheKey))
                {
                    return distanceCache[cacheKey];
                }
                response = await SendGetRouteRequest(startLon, endLon, startLat, endLat, tryNumber);
                if (!response.IsSuccessStatusCode)
                    tryNumber++;
                else
                    break;
            }
            await Task.Delay(1000);
            resetRetries++;
        }
        if (response is null || !response.IsSuccessStatusCode)
            return 0; // if the request failed.

        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(json);
        var route = doc.RootElement.GetProperty("routes")[0];

        double distanceMeters = route.GetProperty("distance").GetDouble(); //route daistance in KM.
        double distanceKm = distanceMeters / 1000.0;

        distanceCache.TryAdd(cacheKey, distanceKm);
        return distanceKm;
    }

    /// <summary>
    /// a function that return the latest time the sent order can be delivered.
    /// </summary>
    /// <param name="orderId">the sent order.</param>
    /// <returns></returns>
    internal static DateTime GetSupplyMaxTime(int orderId)
    {
        var open = OrderManager.GetOrder(orderId).OrderOpenedTime;
        return open + AdminManager.GetConfig().MaxTimeWindow;
    }

    /// <summary>
    /// a function that returns a list of orders by their "and status"
    /// </summary>
    /// <param name="filter">the chosen status</param>
    /// <returns></returns>
    internal static IEnumerable<OrderInList> GetOrdersInListByStatus(BO.OrderStatus filter)
    {
        var ordersByStatus = OrderManager.GetOrders();
        var ordersGroup = from order in ordersByStatus
                          group order by order.OrderStatus into groupType
                          select groupType;

        var specificGroup = ordersGroup.FirstOrDefault(g => g.Key == filter)!.Select(ConvertToOrderInList);
        return specificGroup;
    }
}