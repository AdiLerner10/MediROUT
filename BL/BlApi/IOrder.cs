using BO;

namespace BlApi;

/// <summary>
/// /// The Order functions definition in the BL
/// </summary>
public interface IOrder : IObservable
{
    public int[] OrderAmountsByStatus(int id);
    public Task<IEnumerable<BO.OrderInList>> GetOrdersInList(int id, OrderInListProperty? filter, object? obj, OrderInListProperty? sort);
    public void Update(int id, BO.Order existingOrder);
    public Task Cancel(int id, int idToCancel);
    public bool CanBeCanceled(int idToCancel);
    public void Delete(int id, int idToDelete);
    public Task Add(int id, Order newOrder);
    public void OrderCompletion(int id, int courierId, int deliveryId, BO.OrderStatus status);
    public Task OrderToHandle(int id, int courierId, int orderId);
    public BO.Order GetOrder(int id, int orderId);
    public IEnumerable<ClosedDeliveryInList> GetClosedOrders(int id, int courierId, OrderType? filter, ClosedDeliveryProperty? sort);
    public Task<IEnumerable<OpenOrderInList>> GetOpenOrders(int courierId, OrderType? filter, OpenOrderProperty? sort);
    public Task<IEnumerable<BO.OrderInList>> GetOrdersInListByStatus(int id, OrderStatus? filter);
    public DateTime GetSupplyMaxTime(int id);
    public Task<OrderInProgress> GetOrderInProgress(int orderId, double speed);
}
