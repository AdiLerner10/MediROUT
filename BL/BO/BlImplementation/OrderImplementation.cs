using BlApi;
using DO;
using Helpers;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BO.BlImplementation;

internal class OrderImplementation : IOrder
{
    /// <summary>
    /// a function that compares two objects
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    /// <returns></returns>
    private bool ComparerObjects(object obj1, object obj2)
    {
        var obj1String = JsonSerializer.Serialize(obj1);
        var obj2String = JsonSerializer.Serialize(obj2);
        return obj1String == obj2String;

    }

    /// <summary>
    /// a function that returns OrderInProgress
    /// </summary>
    /// <param name="orderId">the id of the order</param>
    /// <param name="speed">ths speed of the delivery vehicle</param>
    /// <returns></returns>
    public async Task<OrderInProgress> GetOrderInProgress(int orderId, double speed)
    {
        var order = OrderManager.GetOrder(orderId);
        var delivery = order.DeliveryPerOrderInList.Last();

        OrderManager.Location compLoc = await OrderManager.GetLocationCalcValues(AdminManager.GetConfig().CompanyAddress);
        OrderManager.Location orderLoc = await OrderManager.GetLocationCalcValues(order.Address);

        var actualDistance = await OrderManager.CalculateRoute(orderLoc, compLoc);
        return new OrderInProgress()
        {
            OrderId = orderId,
            Address = order.Address,
            AirDistance = order.AirDistance,
            ClientName = order.ClientName,
            ClientPhoneNumber = order.ClientPhoneNumber,
            DeliveryId = delivery.DeliveryId,
            Description = order.Description,
            ExpectedDeliveryTime = delivery.DeliveryStartTime.AddHours(actualDistance / speed),
            RemainingTime = (TimeSpan)order.RemainingTime,
            InOrOut = order.InOrOut,
            MaxDeliveryTime = OrderManager.GetSupplyMaxTime(order.Id),
            OrderDeliveryStartTime = delivery.DeliveryStartTime,
            OrderOpenTime = order.OrderOpenedTime,
            OrderStatus = order.OrderStatus,
            OrderType = order.OrderType,
            ScheduleStatus = order.ScheduleStatus,
            ActualDistance = actualDistance
        };
    }

    /// <summary>
    /// a function that add the sent order into the data base
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newOrder"></param>
    public async Task Add(int id, Order newOrder)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        await OrderManager.Add(newOrder);
    }

    /// <summary>
    /// cancels an existing order
    /// </summary>
    /// <param name="id">manager id </param>
    /// <param name="idToCancel">id of the order we want to cancel</param>
    public async Task Cancel(int id, int idToCancel)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        await OrderManager.CancelOrder(idToCancel);
    }

    /// <summary>
    /// a function that checks if the order can be canceled.
    /// </summary>
    /// <param name="idToCancel">the id of the order to cancel</param>
    /// <returns></returns>
    public bool CanBeCanceled(int idToCancel)
    {
        return OrderManager.CanBeCanceled(idToCancel);
    }

    /// <summary>
    /// a function that throw an exceprion while there is an attempt to delete an order from the data base
    /// </summary>
    /// <param name="id"></param>
    /// <param name="idToDelete">the id of the order we try to delete.</param>
    /// <exception cref="IllegalRequestException"></exception>
    public void Delete(int id, int idToDelete)
    {
        throw new BlIllegalRequestException("delete order is illegal");
    }

    /// <summary>
    /// gets an order by the id
    /// </summary>
    /// <param name="id">manager id </param>
    /// <param name="orderId">order id </param>
    /// <returns></returns>
    public BO.Order GetOrder(int id, int orderId)
    {
        return OrderManager.GetOrder(orderId);
    }

    /// <summary>
    /// a function that returns all open Orders.
    /// </summary>
    /// <param name="courierId"></param>
    /// <param name="filter">the property we will filter the list by.</param>
    /// <param name="sort">the property we will sort the list by</param>
    /// <returns></returns>
    public async Task<IEnumerable<OpenOrderInList>> GetOpenOrders(int courierId, OrderType? filter, OpenOrderProperty? sort)
    {
        return await OrderManager.GetOpenOrders(courierId, filter, sort);
    }

    /// <summary>
    /// a function that returns all Closed Orders.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="courierId"></param>
    /// <param name="filter">the property we will filter the list by.</param>
    /// <param name="obj"></param>
    /// <param name="sort">the property we will sort the list by</param>
    /// <returns></returns>
    public IEnumerable<ClosedDeliveryInList> GetClosedOrders(int id, int courierId, OrderType? filter, ClosedDeliveryProperty? sort)
    {
        return OrderManager.GetClosedOrders(id, courierId, filter, sort);
    }

    /// <summary>
    /// a function that returns list of "OrderInList" that filtered by a specific property, and also sorted by a spesific property.
    /// </summary>
    /// <param name="id">the id of the request entity</param>
    /// <param name="filter">the property we will filter the list by.</param>
    /// <param name="obj">the list contains the property that equals the sent object</param>
    /// <param name="sort">the property we will sort the list by</param>
    /// <returns></returns>
    public async Task<IEnumerable<BO.OrderInList>> GetOrdersInList(int id, OrderInListProperty? filter, object? obj, OrderInListProperty? sort)
    {
        IEnumerable<BO.OrderInList> list = await OrderManager.GetOrdersInList();
        if (filter == null || filter == BO.OrderInListProperty.None)
            list = list.OrderBy(order => Tools.SortByProperty(sort, order));
        else
        {
            list = list.Where(order => ComparerObjects(Tools.SortByProperty(filter, order), obj));
            list = list.OrderBy(order => Tools.SortByProperty(sort, order));
        }
        return list;
    }

    /// <summary>
    /// a function that filter the list of orders only by Status value
    /// </summary>
    /// <param name="id">the requested Id</param>
    /// <param name="filter">the status value to filter</param>
    /// <returns></returns>
    public async Task<IEnumerable<BO.OrderInList>> GetOrdersInListByStatus(int id, OrderStatus? filter)
    {
        if (filter != null)
            return OrderManager.GetOrdersInListByStatus((BO.OrderStatus)filter);
        return await GetOrdersInList(id, BO.OrderInListProperty.None, null, BO.OrderInListProperty.None);
    }


    /// <summary>
    /// a function that returns an array of  the numbers of the orders from every type, using the index of the array.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int[] OrderAmountsByStatus(int id)
    {
        return OrderManager.OrderAmountByStatus();
    }

    /// <summary>
    /// a function that change the order values to a completed one.
    /// </summary>
    /// <param name="id">the id of the entity who call the function.</param>
    /// <param name="courierId">the id of the courier who delivered the order.</param>
    /// <param name="deliveryId">the id of the delivery.<param>
    /// <exception cref="IllegalRequestException">an exception for illegal requests.</exception>
    public void OrderCompletion(int id, int courierId, int orderId, BO.OrderStatus status)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        var order = OrderManager.GetOrder(orderId);
        if (order.DeliveryPerOrderInList!.Last().CourierId != courierId)
            throw new BlIllegalRequestException("this courier isn't responsible for this order!");
        try
        {
            OrderManager.Completion(courierId, orderId, status);

            order.OrderStatus = status;
            OrderManager.Update(order);

        }
        catch { }
    }
    /// <summary>
    /// a function that calculates the maximum time the delivery could take.
    /// </summary>
    /// <param name="id">the id of the order will be delivered</param>
    /// <returns></returns>
    public DateTime GetSupplyMaxTime(int orderId)
    {
        return OrderManager.GetSupplyMaxTime(orderId);
    }

    /// <summary>
    /// a function that take a chosen order and create a delivery for it.
    /// </summary>
    /// <param name="id">the id of the entity who call this function.</param>
    /// <param name="courierId">the id of the chosen courier to thi delivery.</param>
    /// <param name="orderId">the id of the chosen order.</param>
    /// <exception cref="IllegalRequestException"></exception>
    public async Task OrderToHandle(int id, int courierId, int orderId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        await OrderManager.OrderToHandle(id, courierId, orderId);
    }

    /// <summary>
    /// a function that updates a chosen order details'.
    /// </summary>
    /// <param name="id">the id of the entity who call the function.</param>
    /// <param name="existingOrder">the chosen order to be updated.</param>
    public void Update(int id, BO.Order existingOrder)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        if (id == AdminManager.GetConfig().ManagerId)
            OrderManager.Update(existingOrder);
    }

    public void AddObserver(Action listObserver) => OrderManager.Observers.AddListObserver(listObserver);

    public void AddObserver(int id, Action observer) => OrderManager.Observers.AddObserver(id, observer);

    public void RemoveObserver(Action listObserver) => OrderManager.Observers.RemoveListObserver(listObserver);

    public void RemoveObserver(int id, Action observer) => OrderManager.Observers.RemoveObserver(id, observer);


}
