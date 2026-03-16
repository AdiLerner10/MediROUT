using DO;
using Helpers;
using System.Net;
using System.Reflection.Metadata;

namespace BO;

/// <summary>
/// The features that wuld be shown in the list
/// </summary>
/// <param name="DeliveryId">Delivery Id</param>
/// <param name="OrderId">Order Id</param>
/// <param name="OrderType">The type of the order.Medicine , Machines, OperatingRoomEquipment,and etc. </param>
/// <param name="InOrOut">Detrmine if the order is coming inside the hospital or out of the hospital</param>
/// <param name="AirDistance">The air distance between the destination to the order origin</param>
/// <param name="ScheduleStatus">The status of the schedule.in time or late</param>
/// <param name="RemainingTime">The time remained to deliver the order before it's counted int late</param>
/// <param name="NumberOfDeliveries">The amount of the deliveries conducted to deliver the order</param>
public class OrderInList
{
    private static string? m_CompanyAddress = null;
    private string GetCompanyAddress
    {
        get
        {
            if (string.IsNullOrEmpty(m_CompanyAddress))
                m_CompanyAddress = AdminManager.GetConfig().CompanyAddress;
            return m_CompanyAddress;
        }
    }

    public OrderInList(BO.Order order)
    {
        AirDistance = order.AirDistance;
        InOrOut = order.InOrOut;
        OrderId = order.Id;
        OrderType = order.OrderType;
        RemainingTime = (TimeSpan)order.RemainingTime;
        ScheduleStatus = order.ScheduleStatus;
    }

    public OrderInList(DO.Order order)
    {
        //var locVal = OrderManager.GetLocationCalcValues(order.Address);
        OrderId = order.Id;
        OrderType = (BO.OrderType)order.TypeOfOrder;
        InOrOut = (BO.InOrOut)order.inOrOut;
        //AirDistance = OrderManager.AirDistanceCalculation(locVal);
        InOrOut = order.inOrOut == DO.InOrOut.In ? (BO.InOrOut.In) : (BO.InOrOut.Out);
        //RemainingTime = ExpectedDeliveryTime - AdminManager.Now;
        //AirDistance = order.AirDistance,
        //    RemainingTime = (TimeSpan)order.RemainingTime,
        //    ScheduleStatus = order.ScheduleStatus,
    }

    public int? DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public InOrOut InOrOut { get; init; }
    public double AirDistance { get; set; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public TimeSpan RemainingTime { get; init; }
    public TimeSpan ToatalTime { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public int NumberOfDeliveries { get; init; }
    public override string ToString() => this.ToStringProperty();
}
