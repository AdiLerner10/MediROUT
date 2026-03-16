using DO;
using Helpers;

namespace BO;

/// <summary>
/// Order Entity
/// </summary>
/// <param name="OrderId">Order Id</param>
/// <param name="OrderType">The type of the order.Medicine , Machines, OperatingRoomEquipment,and etc. </param>
/// <param name="InOrOut">Detrmine if the order is coming inside the hospital or out of the hospital</param>
/// <param name="Description">A short description about the spesific order</param>
/// <param name="Address">The destination of the order</param>
/// <param name="Latitude">The latitude value</param>
/// <param name="Longitude">The longitude value</param>
/// <param name="AirDistance">The air distance between the destination to the order origin</param>
/// <param name="ClientName">The client name</param>
/// <param name="ClientPhoneNumber">The client phone number</param>
/// <param name="Volume">The volume of the order</param>
/// <param name="Fragile">The fragiglity of the order</param>
/// <param name="OrderOpenTime">The time the order were opened by the client</param>
/// <param name="OrderDeliveryStartTime">The time the courier started to deliver the order</param>
/// <param name="
/// ">The time the order should be deliver to the client</param>
/// <param name="MaxDeliveryTime">The maximum late time the order can be delivered</param>
/// <param name="OrderStatus">The status of the order.Delivered , Refuses, and etc.</param>
/// <param name="ScheduleStatus">The status of the schedule.in time or late</param>
/// <param name="RemainingTime">The time remained to deliver the order before it's counted int late</param>
/// <param name="DeliveryPerOrderInList">list of the deliveries that tried to deliver the order</param>
public class Order
{
    public int Id { get; init; } = 0;
    public OrderType OrderType { get; set; } = BO.OrderType.Medicine;
    public InOrOut InOrOut { get; set; } = (BO.InOrOut.Out);
    public string Address { get; set; } = "";
    public string? Description { get; set; } = "";
    public double Latitude { get; set; } = 0;
    public double Longitude { get; set; } = 0;
    public double AirDistance { get; set; } = 0;
    public string ClientName { get; set; } = "";
    public string ClientPhoneNumber { get; set; } = "0000000000";
    public double Volume { get; set; } = 0;
    public bool Fragile { get; set; } = false;
    public DateTime OrderOpenedTime { get; init; } = AdminManager.Now;
    public DateTime? ExpectedDeliveryTime { get; init; } = null;
    public OrderStatus OrderStatus { get; set; } = 0;
    public ScheduleStatus ScheduleStatus { get; init; } = ScheduleStatus.RunningOnTime;
    public TimeSpan? RemainingTime { get; init; } = null;
    public List<DeliveryPerOrderInList> DeliveryPerOrderInList { get; init; } = new();
    public override string ToString() => this.ToStringProperty();

    private static TimeSpan? m_MaxTimeWindow = null;

    private TimeSpan GetMaxTimeWindow
    {
        get
        {
            if (!m_MaxTimeWindow.HasValue)
                m_MaxTimeWindow = AdminManager.GetConfig().MaxTimeWindow;
            return m_MaxTimeWindow.Value;
        }
    }

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


    public Order() { }

    public Order(BO.OrderType orderType, BO.InOrOut inOrOut, string? description, string address, string clientName, string clientPhoneNumber, double volume, bool fragile)
    {
        OrderType = orderType;
        InOrOut = inOrOut;
        Description = description;
        Address = address;
        ClientName = clientName;
        ClientPhoneNumber = clientPhoneNumber;
        Volume = volume;
        Fragile = fragile;
        OrderOpenedTime = AdminManager.Now;
        ExpectedDeliveryTime = AdminManager.Now + GetMaxTimeWindow;
        OrderStatus = BO.OrderStatus.Pending;
        ScheduleStatus = ScheduleStatus.RunningOnTime;
        RemainingTime = ExpectedDeliveryTime - AdminManager.Now;
    }

    public Order(DO.Order order, Delivery? delivery, BO.OrderStatus status = BO.OrderStatus.Pending)
    {

        OrderManager.Location orderLoc = new OrderManager.Location()
        {
            Longitude = order.Longitude,
            Latitude = order.Latitude
        };

        Id = order.Id;
        OrderType = (BO.OrderType)order.TypeOfOrder;
        InOrOut = (BO.InOrOut)order.inOrOut;
        Description = order.Description;
        Address = order.Address;
        Latitude = order.Latitude;
        Longitude = order.Longitude;
        ClientName = order.Name;
        ClientPhoneNumber = order.PhoneNumber;
        Volume = order.volume;
        Fragile = order.Fragile;
        OrderOpenedTime = AdminManager.Now;
        ExpectedDeliveryTime = AdminManager.Now + GetMaxTimeWindow;
        OrderStatus = status;
        ScheduleStatus = ScheduleStatus.RunningOnTime;
        RemainingTime = ExpectedDeliveryTime - AdminManager.Now;
    }
}
