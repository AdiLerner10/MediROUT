namespace Dal;

/// <summary>
/// Lists that will contain the data of the three entitys
/// </summary>
internal static class DataSource
{
    internal static List<DO.Courier> Couriers {get;}=new();
    internal static List<DO.Delivery> Deliveries {get;}=new();
    internal static List<DO.Order> Orders {get;}=new();
}
