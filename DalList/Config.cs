
using System.Runtime.CompilerServices;

namespace Dal;

/// <summary>
/// The static members of the project
/// ,and the reset function
/// </summary>
internal static class Config
{
    internal static int StartDeliveryId = 10010;
    private static int nextDeliveryId = StartDeliveryId;
    internal static int NextDeliveryId { get => nextDeliveryId += 2; }
    //automatic number for delivery id

    internal static int StartOrderId = 10000;
    private static int nextOrderId = StartOrderId;
    internal static int NextOrderId { get => nextOrderId++; }
    //automatic number for order id 

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static int ManagerId { get; set; } = 215868001;
    internal static string ManagerPassword { get; set; } = "AlCs1234";
    internal static double? ManagerSalary { get; set; } = 400;
    internal static double? CourierSalary { get; set; } = 35;

    internal static string? CompanyAddress { get; set; } = "Shmuel (Hans) Beyth St 12, Jerusalem, 9103102";
    internal static double? Latitude { get; set; } = 31.7705;
    internal static double? Longitude { get; set; } = 35.2294;
    internal static double? MaximumDistanceKM { get; set; } = 500;
    //there is no maximum distance, the hospital delivers to the whole Israel, and from every factory in Israel 

    internal static double? CarAvgSpeedKMH { get; set; } = 50;
    internal static double MotorcycleAvgSpeedKMH { get; set; } = 60;
    internal static double BicycleAvgSpeedKMH { get; set; } = 22;
    internal static double OnFootAvgSpeedKMH { get; set; } = 5;
    internal static TimeSpan MaxTimeWindow { get; set; } = TimeSpan.FromDays(7);
    // the orders are delivered within a week

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromDays(2);
    //two days before the last date the order can be delivered are "risk" days
    internal static TimeSpan CourierNotActive { get; set; } = TimeSpan.FromDays(14);

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        Clock = DateTime.Now;
        ManagerPassword = "AlCs1234";
        ManagerSalary = 400;
        CourierSalary = 35;
        CompanyAddress = "12, Shmuel Bayit Street, Jerusalem";
        Latitude = 31.7705;
        Longitude = 35.2294;
        MaximumDistanceKM = 320;
        CarAvgSpeedKMH = 50;
        MotorcycleAvgSpeedKMH = 60;
        BicycleAvgSpeedKMH = 22;
        OnFootAvgSpeedKMH = 5;
        MaxTimeWindow = TimeSpan.FromDays(7);
        RiskRange = TimeSpan.FromDays(2);
        CourierNotActive = TimeSpan.FromDays(14);
    }
}
