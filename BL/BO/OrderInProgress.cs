using Helpers;

namespace BO;

/// <summary>
/// Order In Progress Entity
/// </summary>
/// <param name="DeliveryId">Delivery Id</param>
/// <param name="OrderId">Order Id</param>
/// <param name="OrderType">The type of the order.Medicine , Machines, OperatingRoomEquipment,and etc. </param>
/// <param name="InOrOut">Detrmine if the order is coming inside the hospital or out of the hospital</param>
/// <param name="Description">A short description about the spesific order</param>
/// <param name="Address">The destination of the order</param>
/// <param name="AirDistance">The air distance between the destination to the order origin</param>
/// <param name="ActualDistance">The actual distance (not air distance) between the destination to the origin</param>
/// <param name="ClientName">The client name</param>
/// <param name="ClientPhoneNumber">The client phone number</param>
/// <param name="OrderOpenTime">The time the order were opened by the client</param>
/// <param name="OrderDeliveryStartTime">The time the courier started to deliver the order</param>
/// <param name="ExpectedDeliveryTime">The time the order should be deliver to the client</param>
/// <param name="MaxDeliveryTime">The maximum late time the order can be delivered</param>
/// <param name="OrderStatus">The status of the order.Delivered , Refuses, and etc.</param>
/// <param name="ScheduleStatus">The status of the schedule.in time or late</param>
/// <param name="RemainingTime">The time remained to deliver the order before it's counted int late</param>
public class OrderInProgress
{
    public OrderInProgress(BO.Order order, BO.Courier courier)
    {
        var delivery = DeliveryManager.GetDelivery(order.DeliveryPerOrderInList.Last().DeliveryId) ?? null;
        //var deliveries = DeliveryManager.GetFullDeliveries().Where(d => d.OrderId == order.Id && d.OrderStatus == DO.OrderStatus.InProcess&&d.CourierId==courier.Id).FirstOrDefault();
        DeliveryId = delivery?.Id ?? 0;
        OrderId = delivery?.OrderId ?? 0;
        OrderType = order.OrderType;
        InOrOut = order.InOrOut;
        Description = order.Description;
        Address = order.Address;
        AirDistance = order.AirDistance;
        ActualDistance = delivery?.Distance ?? 0;
        ClientName = order.ClientName;
        ClientPhoneNumber = order.ClientPhoneNumber;
        OrderOpenTime = order.OrderOpenedTime;
        OrderDeliveryStartTime = delivery?.DeliveryStartTime ?? DateTime.MinValue;
        ExpectedDeliveryTime = (delivery?.DeliveryStartTime ?? DateTime.MinValue)
       + TimeSpan.FromHours((ActualDistance ?? 0) / Tools.SpeedCalculation((BO.Vehicle)delivery?.VehicleType));
        MaxDeliveryTime = OrderDeliveryStartTime + AdminManager.GetConfig().MaxTimeWindow;
        OrderStatus = order.OrderStatus;
        ScheduleStatus = order.ScheduleStatus;
        RemainingTime = (AdminManager.GetConfig().MaxTimeWindow - (AdminManager.Now - OrderOpenTime));
    }

    public OrderInProgress(
 DO.Order order)
    {
        OrderId = order.Id;
        Address = order.Address;
        ClientName = order.Name;
        ClientPhoneNumber = order.PhoneNumber;
    }

    public static string TOKEN = "pk.be7a3a2d7a587b7e3de08da593f31e71";
    public OrderInProgress() {}

    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public InOrOut InOrOut { get; init; }
    public string? Description { get; init; }
    public string Address { get; init; }
    public double AirDistance { get; init; }
    public double? ActualDistance { get; init; }
    public string ClientName { get; init; }
    public string ClientPhoneNumber { get; init; }
    public DateTime OrderOpenTime { get; init; }
    public DateTime OrderDeliveryStartTime { get; init; }
    public DateTime ExpectedDeliveryTime { get; init; }
    public DateTime MaxDeliveryTime { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public TimeSpan RemainingTime { get; init; }
    public override string ToString() => this.ToStringProperty();
}

