using BlApi;
using BO;
using BO.BlImplementation;
using DalApi;
using DO;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Globalization;
namespace Helpers;

internal static class CourierManager
{
    private static readonly AsyncMutex s_periodicMutex = new(); //stage 7

    private static IDal s_dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new();

    /// <summary>
    /// Adds a courier to the data base 
    /// </summary>
    /// <param name="managerId">managers id </param>
    /// <param name="boCourier">the BO courier to add to the data base</param>
    internal static void AddCourier(int managerId, BO.Courier boCourier)
    {
        Tools.IsValidId(boCourier.Id);
        Tools.IsStrongPassword(boCourier.Password);
        Tools.IsValidEmail(boCourier.Email);
        Tools.IsValidPhoneNumber(boCourier.PhoneNumber);
        DO.Courier doCourier = new DO.Courier()
        {
            Id = boCourier.Id,
            Name = boCourier.Name,
            PhoneNumber = boCourier.PhoneNumber,
            Salary = CourierManager.CalculateSalaryPerHour(boCourier),
            Email = boCourier.Email,
            Password = Tools.Encryption(boCourier.Password),
            VehicleType = (DO.Vehicle)boCourier.Vehicle,
            Active = boCourier.Active ?? true,
            MaxPersonalDistance = boCourier.MaxPersonalDistance,
            EmploymentDate = boCourier.EmploymentDate
        };
        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Courier.Create(doCourier);
            Observers.NotifyListUpdated();
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BlAlreadyExistException(ex.Message, ex);
        }
    }

    /// <summary>
    /// calculates a couriers salary per hour 
    /// </summary>
    /// <param name="boCourier">the courier we are calculating the salary for </param>
    /// <returns></returns>
    private static double CalculateSalaryPerHour(BO.Courier boCourier)
    {
        double cnfgSalary;
        int years;
        lock (AdminManager.BlMutex)
        {
            cnfgSalary = AdminManager.GetConfig().CourierSalary;
            years = AdminManager.Now.Year - boCourier.EmploymentDate.Year;
        }
        return (cnfgSalary + years);
    }

    /// <summary>
    /// Deletes a courier from the data base
    /// </summary>
    /// <param name="managerId">managers id</param>
    /// <param name="courierId">Id of the courier to delete</param>
    internal static void DeleteCourier(int managerId, int courierId)
    {
        DO.Courier? courier;
        lock (AdminManager.BlMutex)
            courier = s_dal.Courier.Read(courierId);//checks if the courier exists
        if (courier == null)
            throw new BO.BlDoesNotExistException($"Courier with ID={courierId} does not exist");
        try
        {
            IEnumerable<Delivery>? deliveries;
            lock (AdminManager.BlMutex)
                deliveries = s_dal.Delivery.ReadAll(d => d.CourierId == courierId);
            if (deliveries.FirstOrDefault() != null)
                throw new BO.BlIllegalRequestException("Cannot delete a courier who is currently handling or has handled orders.");
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }

        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Courier.Delete(courierId);
            Observers.NotifyItemUpdated(courierId);
            Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(ex.Message, ex);
        }
    }

    /// <summary>
    /// returns a courier with a specific Id
    /// </summary>
    /// <param name="courierid">id of the courier to get</param>
    /// <returns></returns>
    /// <exception cref="BlDoesNotExistException">if there is no courier with the given id throws an exception</exception>
    internal static BO.Courier GetCourier(int courierid)
    {
        Delivery? delivery;
        DO.Courier doCourier;
        lock (AdminManager.BlMutex)
        {
            doCourier = s_dal.Courier.Read(courierid) ?? throw new BlDoesNotExistException($"Courier with ID={courierid} does not exist");
            delivery = s_dal.Delivery
          .ReadAll(d =>
              d.CourierId == courierid &&
              d.OrderStatus == DO.OrderStatus.InProcess)
          .FirstOrDefault();
        }
        //var deliveries = DeliveryManager.GetFullDeliveries();

        BO.Courier boCourier = new BO.Courier()
        {
            Id = courierid,
            Name = doCourier.Name,
            Email = doCourier.Email,
            Password = doCourier.Password,
            Vehicle = (BO.Vehicle)doCourier.VehicleType,
            PhoneNumber = doCourier.PhoneNumber,
            Active = doCourier.Active,
            MaxPersonalDistance = doCourier.MaxPersonalDistance,
            EmploymentDate = (DateTime)doCourier.EmploymentDate,
            ExistingOrder = null
        };
        BO.Order? order = null;
        if (delivery != null)
        {
            order = OrderManager.GetOrder(delivery.OrderId);

            var tempExistingOrder = order != null ? CourierManager.ConverToOrderInProgress(order, boCourier)
               : null;

            boCourier.ExistingOrder = tempExistingOrder;
        }
        boCourier.Salary = CalculateSalaryPerHour(boCourier);
        return boCourier;
    }

    /// <summary>
    /// uodates a courier in the data base
    /// </summary>
    /// <param name="boCourier">courier to update</param>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    internal static void UpdateCourier(BO.Courier boCourier)
    {
        DO.Courier doCourier;
        lock (AdminManager.BlMutex)
            doCourier = s_dal.Courier.Read(boCourier.Id)
        ?? throw new BO.BlDoesNotExistException($"Courier with ID={boCourier.Id} does not exist");

        string newPassword = boCourier.Password;
        if (newPassword != doCourier.Password)
            newPassword = Tools.Encryption(newPassword);


        Tools.IsStrongPassword(newPassword);
        Tools.IsValidId(boCourier.Id);
        Tools.IsStrongPassword(boCourier.Password);
        Tools.IsValidEmail(boCourier.Email);
        Tools.IsValidPhoneNumber(boCourier.PhoneNumber);
        DO.Courier newdoCourier = new DO.Courier()
        {
            Id = boCourier.Id,
            PhoneNumber = boCourier.PhoneNumber,
            Name = boCourier.Name,
            Email = boCourier.Email,
            VehicleType = (DO.Vehicle)boCourier.Vehicle,
            Salary = CourierManager.CalculateSalaryPerHour(boCourier),
            Active = boCourier.Active ?? true,
            MaxPersonalDistance = boCourier.MaxPersonalDistance,
            EmploymentDate = boCourier.EmploymentDate,
            Password = newPassword,
        };

        try
        {
            lock (AdminManager.BlMutex)
                s_dal.Courier.Update(newdoCourier);
            Observers.NotifyItemUpdated(boCourier.Id);
            Observers.NotifyListUpdated();
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(ex.Message, ex);
        }
    }

    /// <summary>
    /// checks if the username and passowrd are correct. 
    /// If they are return the user type
    /// </summary>
    /// <param name="username">username </param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlInvalidCredentials"></exception>
    internal static string Entrance(int id, string password)
    {
        if (Tools.CheckManagerPassword(id, password))
        {
            return "Manager";
        }

        try
        {
            DO.Courier? doCourier;
            lock (AdminManager.BlMutex)
                doCourier = s_dal.Courier.Read(c => c.Id == id);
            if (doCourier == null)
                throw new BO.BlInvalidCredentials($"User '{id}' does not exist");
            if (doCourier.Password != Tools.Encryption(password))
                throw new BO.BlInvalidCredentials("Incorrect password");
            return "Courier";
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }


    /// <summary>
    /// a function that convert a CourierInList object to BO.Courier object
    /// </summary>
    /// <param name="courier">the given CourierInList object</param>
    /// <returns></returns>
    public static BO.Courier ConvertToBOCourier(CourierInList courier)
    {
        return GetCourier(courier.Id);
    }

    /// <summary>
    /// a function that convert a DO.Courier object to BO.Courier object
    /// </summary>
    /// <param name="courier">the given DO.Courier object</param>
    /// <returns></returns>
    public static BO.Courier ConvertToBOCourier(DO.Courier courier)
    {
        return GetCourier(courier.Id);
    }

    /// <summary>
    /// gets a courier list sorted by a certain filter 
    /// </summary>
    /// <param name="id">manager id</param>
    /// <param name="active">if not null returns only active couriers</param>
    /// <param name="sortType">the filter to sort by</param>
    /// <returns></returns>
    internal static IEnumerable<BO.CourierInList> GetCourierList(bool? active, BO.SortCourierByProperty? sortType)
    {
        try
        {
            IEnumerable<DO.Courier>? allCouriers;
            lock (AdminManager.BlMutex)
                allCouriers = s_dal.Courier.ReadAll();
            if (active != null)
            {
                allCouriers = allCouriers.Where(c => c.Active == active.Value);
            }

            var list = allCouriers.Select(c => new BO.CourierInList
            {
                Id = c.Id,
                Name = c.Name,
                Active = c.Active,
                Salary = CalculateSalaryPerHour(ConvertToBOCourier(c)),
                VehicleType = (BO.Vehicle)c.VehicleType,
                EmploymentDate = (DateTime)c.EmploymentDate,
                NumberOfOrdersDeliveredOnTime = DeliveredOnTime(c),
                NumberOfOrdersDeliveredLate = DeliveredLate(c),
                NumberOfCurrentOrderId = getCurrentOrderId(c)
            });
            list = list.OrderBy(c =>
            {
                if (sortType == null)
                    return c.Id;

                switch (sortType.Value)
                {
                    case SortCourierByProperty.Id:
                        return (object)c.Id;
                    case SortCourierByProperty.Name:
                        return (object)c.Name;
                    case SortCourierByProperty.EmploymentDate:
                        return (object)c.EmploymentDate;
                    case SortCourierByProperty.NumOfProvidedOnTime:
                        return (object)c.NumberOfOrdersDeliveredOnTime;
                    default:
                        return (object)c.Id;
                }
            });

            return list;
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }


    /// <summary>
    /// checks how many orders the courier delivered on time
    /// </summary>
    /// <param name="courier">the courier we want to check</param>
    /// <returns></returns>
    internal static int DeliveredOnTime(DO.Courier courier)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                return s_dal.Delivery
                .ReadAll(d => d.CourierId == courier.Id && d.OrderStatus == DO.OrderStatus.Delivered)
                .Count(d => d.DeliveryStartTime != null
                            && d.EndTime != null
                            && (d.EndTime.Value - d.DeliveryStartTime.Value).TotalDays <= s_dal.Config.MaxTimeWindow.Days);
            }
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }

    /// <summary>
    /// checks how many orders the courier delivered late
    /// </summary>
    /// <param name="courier">the courier we want to check</param>
    /// <returns></returns>
    internal static int DeliveredLate(DO.Courier courier)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                return s_dal.Delivery
          .ReadAll(d => d.CourierId == courier.Id && d.OrderStatus == DO.OrderStatus.Delivered)
          .Count(d => d.DeliveryStartTime != null
                      && d.EndTime != null
                      && (d.EndTime.Value - d.DeliveryStartTime.Value).TotalDays > s_dal.Config.MaxTimeWindow.Days);
            }
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }


    /// <summary>
    /// gets the order id of the order the courier is currently handeling
    /// if he is not handeling any order returns null
    /// </summary>
    /// <param name="courier">the courier we are checking</param>
    /// <returns></returns>
    internal static int? getCurrentOrderId(DO.Courier courier)
    {
        Delivery? currentDelivery;
        lock (AdminManager.BlMutex)
        {
            currentDelivery = s_dal.Delivery
            .ReadAll(d => d.CourierId == courier.Id && d.OrderStatus == DO.OrderStatus.InProcess)
            .FirstOrDefault();
        }

        return currentDelivery?.OrderId;
    }

    internal static BO.OrderInProgress ConverToOrderInProgress(BO.Order order, BO.Courier courier)
    {
        return new BO.OrderInProgress(order, courier);
    }



    /// <summary>
    /// a function that happens once in while, and updates the couriers.
    /// </summary>
    internal static void PeriodicCouriersUpdate(DateTime oldDate, DateTime newDate)
    {
        //if (s_periodicMutex.CheckAndSetInProgress())
        //    return;
        //IConfig cnfg;

        //List<DO.Courier>? list;
        //List<DO.Delivery>? deliveries;
        //lock (AdminManager.BlMutex)
        //{
        //    cnfg = s_dal.Config;
        //    list = s_dal.Courier.ReadAll().ToList();
        //    deliveries = s_dal.Delivery.ReadAll().ToList();
        //}

        //foreach (var doCourier in list)
        //{
        //    var courierDeliveries = deliveries.Where(d => d.CourierId == doCourier.Id).ToList();
        //    bool ActiveCheck = false;
        //    foreach (var delivery in courierDeliveries)
        //    {
        //        if (((BO.OrderStatus?)delivery.OrderStatus == BO.OrderStatus.InProcess || ((TimeSpan)(AdminManager.Now - delivery.EndTime)) <= cnfg.CourierNotActive))
        //            ActiveCheck = true;
        //    }

        //    //if the courier didn't work for max time window days
        //    //then courier should be automatically updated to 'not active'
        //    if (!ActiveCheck)
        //    {
        //        try
        //        {
        //            lock (AdminManager.BlMutex)
        //                s_dal.Courier.Update(doCourier with { Active = false });
        //            Observers.NotifyListUpdated();
        //            Observers.NotifyItemUpdated(doCourier.Id);
        //        }
        //        catch (DalDoesNotExistException ex)
        //        {
        //            throw new BlDoesNotExistException(ex.Message, ex);
        //        }

        //    }
        //} // Ensure the Select operation is executed immediately for all items
        //s_periodicMutex.UnsetInProgress();
        if (s_periodicMutex.CheckAndSetInProgress()) return;

        IConfig cnfg;
        List<DO.Courier> list;
        List<DO.Delivery> deliveries;
        lock (AdminManager.BlMutex)
        {
            cnfg = s_dal.Config;
            list = s_dal.Courier.ReadAll().ToList();
            deliveries = s_dal.Delivery.ReadAll().ToList();
        }
        bool listChanged = false;
        foreach (var doCourier in list.Where(c => c.Active))
        {
            var courierDeliveries = deliveries.Where(d => d.CourierId == doCourier.Id).ToList();

            // Stay active if:
            // 1. Hired recently (less than cnfg.CourierNotActive days ago)
            // 2. Has an order in process
            // 3. Finished an order recently
            bool hasActivity = (AdminManager.Now - doCourier.EmploymentDate).Value.TotalDays < cnfg.CourierNotActive.TotalDays;

            if (!hasActivity)
            {
                hasActivity = courierDeliveries.Any(d =>
                    d.OrderStatus == DO.OrderStatus.InProcess ||
                    (d.EndTime != null && (AdminManager.Now - d.EndTime.Value).TotalDays < cnfg.CourierNotActive.TotalDays));
            }
            if (!hasActivity)
            {
                try
                {
                    lock (AdminManager.BlMutex)
                        s_dal.Courier.Update(doCourier with { Active = false });
                    listChanged = true;
                    Observers.NotifyItemUpdated(doCourier.Id);
                }
                catch (DalDoesNotExistException) { }
            }
        }
        if (listChanged) Observers.NotifyListUpdated();
        s_periodicMutex.UnsetInProgress();
    }


    private static readonly Random s_rand = new();
    private static readonly AsyncMutex s_simulationMutex = new();
    private static ConcurrentDictionary<int, Tuple<DO.Order, double>> ordersDistance = new();
    private static HashSet<int> visitedOrders = new();
    private static HashSet<int> visitedCouriers = new();


    internal static async Task SimulateCourierCollectAndDeliverAsync()
    {
        if (s_simulationMutex.CheckAndSetInProgress())
            return;


        List<DO.Courier> activeCouriers;
        List<DO.Delivery> deliveries;
        List<DO.Order> orders;


        List<Task<Tuple<DO.Order, double>>>? ordersDistanceTasks = null;
        OrderManager.Location companyLoc = new OrderManager.Location { Longitude = (double)s_dal.Config.Longitude!, Latitude = (double)s_dal.Config.Latitude! };
        lock (AdminManager.BlMutex)
        {
            activeCouriers = s_dal.Courier.ReadAll().Where(c => c.Active).ToList();
            deliveries = s_dal.Delivery.ReadAll().ToList();
            orders = s_dal.Order.ReadAll().ToList();

        }

        bool useHeavyCalculateRoute = false;
        lock (AdminManager.SimulatorDistanceMutex)
        {
            ordersDistanceTasks = orders.Select(async o =>
            {
                var orderLoc = new OrderManager.Location { Longitude = o.Longitude, Latitude = o.Latitude };

                double route = 0;
                if (useHeavyCalculateRoute)
                {
                    route = o.inOrOut == DO.InOrOut.In
                        ? await OrderManager.CalculateRoute(orderLoc, companyLoc)
                        : await OrderManager.CalculateRoute(companyLoc, orderLoc);
                }
                else
                {
                    route = o.inOrOut == DO.InOrOut.In
                       ? OrderManager.AirDistanceCalculation(orderLoc, companyLoc)
                       : OrderManager.AirDistanceCalculation(companyLoc, orderLoc);
                }

                return new Tuple<DO.Order, double>(o, route);
            }).ToList();
        }

        if (ordersDistanceTasks.Count != ordersDistance.Count)
        {
            foreach (var t in ordersDistanceTasks)
            {
                var taskRes = await t;
                ordersDistance.TryAdd(taskRes.Item1.Id, taskRes);
            }
        }

        foreach (var courier in activeCouriers)
        //Parallel.ForEach(activeCouriers, courier =>
        {
            bool courierTaken = false;
            lock (visitedCouriers)
            {
                courierTaken = visitedCouriers.Add(courier.Id);
            }
            deliveries = deliveries.Where(d => d.CourierId == courier.Id && d.OrderStatus == DO.OrderStatus.InProcess).ToList();


            if (!courierTaken)
                return;

            if (deliveries is null || deliveries.Count == 0)
            {
                if (s_rand.Next(100) < 50)
                {
                    // 4. סינון סופי של ההזמנות שמתאימות למרחק של השליח
                    var filteredOrders = ordersDistance
                        .Where(r => courier.MaxPersonalDistance >= r.Value.Item2)
                        .ToList();

                    if (s_rand.Next(10) > 3 && filteredOrders?.Count > 0)
                    {
                        var selectedResult = filteredOrders[s_rand.Next(filteredOrders.Count)];
                        bool orderTaken = false;
                        lock (visitedOrders) // נעילה של ה-HashSet כדי למנוע התנגשות בין Threads
                        {
                            // בדיקה אם ההזמנה כבר נלקחה על ידי שליח אחר בריצה הזו
                            if (visitedOrders.Add(selectedResult.Key))
                            {
                                orderTaken = true;
                            }
                        }
                        if (orderTaken)
                        {
                            var order = selectedResult.Value.Item1;
                            double distance = 0;
                            if (useHeavyCalculateRoute)
                                distance = selectedResult.Value.Item2;
                            else
                                distance = await OrderManager.CalculateRoute(
                                    new()
                                    {
                                        Latitude = selectedResult.Value.Item1.Latitude,
                                        Longitude = selectedResult.Value.Item1.Latitude
                                    },
                                    companyLoc
                                );
                            DO.Delivery delivery = new DO.Delivery
                            {
                                CourierId = courier.Id,
                                DeliveryStartTime = AdminManager.Now,
                                Distance = distance,
                                OrderId = order.Id,
                                OrderStatus = DO.OrderStatus.InProcess,
                                VehicleType = courier.VehicleType,
                            };
                            lock (AdminManager.BlMutex)
                            {
                                s_dal.Delivery.Create(delivery);
                            }
                            OrderManager.Observers.NotifyListUpdated();
                            CourierManager.Observers.NotifyListUpdated();
                            DeliveryManager.Observers.NotifyListUpdated();
                            CourierManager.Observers.NotifyItemUpdated(courier.Id);
                            OrderManager.Observers.NotifyItemUpdated(order.Id);

                            lock (visitedOrders)
                            {
                                visitedOrders.Remove(selectedResult.Key);
                            }
                        }
                    }
                }
            }
            else
            {
                DO.Order order = orders.First(o => o.Id == deliveries.Last().OrderId); // s_dal.Order.Read(deliveries.LastOrDefault()!.OrderId)!;

                OrderManager.Location orderLoc = new OrderManager.Location { Latitude = order.Latitude, Longitude = order.Longitude };

                var route = ordersDistance[order.Id];


                var speed = Tools.SpeedCalculation((BO.Vehicle)courier.VehicleType);
                int deliveryTime = speed > 0 ? (int)(route.Item2 / speed) : 0;

                double passedTime = (AdminManager.Now - deliveries.Last().DeliveryStartTime!).Value.TotalDays;
                if (passedTime >= deliveryTime + s_rand.Next(4))
                {
                    DO.OrderStatus status = s_rand.Next(0, 20) switch
                    {
                        >= 1 and <= 4 => DO.OrderStatus.Refuses,
                        >= 5 and <= 9 => DO.OrderStatus.NoRespond,
                        >= 10 and <= 13 => DO.OrderStatus.Faild,
                        _ => DO.OrderStatus.Delivered
                    };

                    DO.Delivery delivery = new Delivery
                    {
                        CourierId = courier.Id,
                        DeliveryStartTime = deliveries.LastOrDefault()!.DeliveryStartTime,
                        Distance = route.Item2,
                        EndTime = AdminManager.Now,
                        Id = deliveries.LastOrDefault()!.Id,
                        OrderId = order.Id,
                        OrderStatus = status,
                        VehicleType = courier.VehicleType
                    };

                    lock (AdminManager.BlMutex)
                        s_dal.Delivery.Update(delivery);
                    DeliveryManager.Observers.NotifyListUpdated();
                    OrderManager.Observers.NotifyListUpdated();
                    CourierManager.Observers.NotifyListUpdated();
                    CourierManager.Observers.NotifyItemUpdated(courier.Id);
                    OrderManager.Observers.NotifyItemUpdated(order.Id);
                }
                else
                {
                    if (s_rand.Next(100) < 5)
                    {
                        DO.Delivery delivery = new Delivery
                        {
                            CourierId = courier.Id,
                            DeliveryStartTime = deliveries.LastOrDefault()!.DeliveryStartTime,
                            Distance = route.Item2,
                            EndTime = AdminManager.Now,
                            Id = deliveries.Last().Id,
                            OrderId = order.Id,
                            OrderStatus = DO.OrderStatus.Canceled,
                            VehicleType = courier.VehicleType
                        };

                        lock (AdminManager.BlMutex)
                            s_dal.Delivery.Update(delivery);

                    }
                    DeliveryManager.Observers.NotifyListUpdated();
                    CourierManager.Observers.NotifyListUpdated();
                    CourierManager.Observers.NotifyItemUpdated(courier.Id);
                    OrderManager.Observers.NotifyItemUpdated(order.Id);
                    OrderManager.Observers.NotifyListUpdated();
                }
            }

            lock (visitedCouriers)
            {
                visitedCouriers.Remove(courier.Id);
            }
        }
        //);
        s_simulationMutex.UnsetInProgress();
    }
}
