using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_couriers_xml = "couriers.xml";
    internal const string s_deliveries_xml = "deliveries.xml";
    internal const string s_orders_xml = "orders.xml";

    /// <summary>
    ///Gets or privately sets the next delivery ID in the XML
    /// </summary>
    internal static int NextDeliveryId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
    }

    /// <summary>
    /// Gets or privately sets the next order ID in the XML
    /// </summary>
    internal static int NextOrderId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
    }

    /// <summary>
    /// Gets or sets the clock in the XML
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    /// <summary>
    /// gets or sets the manager Id in the XML
    /// </summary>
    internal static int ManagerId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
    }

    /// <summary>
    /// Gets or sets the manager password in the XML
    /// </summary>
    internal static string ManagerPassword
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPassword");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPassword", value);
    }

    /// <summary>
    /// Gets or sets the manager Salary in the XML
    /// </summary>
    internal static double? ManagerSalary
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "ManagerSalary");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "ManagerSalary", (double)value);
    }

    /// <summary>
    /// Gets or sets the manager Salary in the XML
    /// </summary>
    internal static double? CourierSalary
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "CourierSalary");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "CourierSalary", (double)value);
    }

    /// <summary>
    /// Gets or sets the company adress in the XML
    /// </summary>
    internal static string CompanyAddress
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value);
    }

    /// <summary>
    /// Gets or sets the latitude in the XML
    /// </summary>
    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Latitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", (double)value);
    }

    /// <summary>
    /// Gets or sets the longitude in the XML
    /// </summary>
    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Longitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", (double)value);
    }

    /// <summary>
    /// Gets or sets the  maximum distance in the XML
    /// </summary>
    internal static double? MaximumDistanceKM
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "MaximumDistanceKM");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "MaximumDistanceKM", (double)value);
    }

    /// <summary>
    /// Gets or sets the average car speed in the XML
    /// </summary>
    internal static double? CarAvgSpeedKMH
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "CarAvgSpeedKMH");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "CarAvgSpeedKMH", (double)value);
    }

    /// <summary>
    /// Gets or sets the average Motorcycle speed in the XML
    /// </summary>
    internal static double? MotorcycleAvgSpeedKMH
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "MotorcycleAvgSpeedKMH");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "MotorcycleAvgSpeedKMH", (double)value);
    }

    /// <summary>
    /// Gets or sets the average bicycle speed in the XML
    /// </summary>
    internal static double? BicycleAvgSpeedKMH
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "BicycleAvgSpeedKMH");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "BicycleAvgSpeedKMH", (double)value);
    }

    /// <summary>
    /// Gets or sets the average speed by foot in the XML
    /// </summary>
    internal static double? OnFootAvgSpeedKMH
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "OnFootAvgSpeedKMH");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "OnFootAvgSpeedKMH", (double)value);
    }

    /// <summary>
    /// Gets or sets the max time window in the XML
    /// </summary>
    internal static TimeSpan MaxTimeWindow
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "MaxTimeWindow");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "MaxTimeWindow", value);
    }

    /// <summary>
    /// Gets or sets the risk range in the XML
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    /// <summary>
    /// Gets or sets the courier not active time span in the XML
    /// </summary>
    internal static TimeSpan CourierNotActive
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "CourierNotActive");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "CourierNotActive", value);
    }

    /// <summary>
    /// reset function
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextDeliveryId = 10010;
        NextOrderId = 10000;
        Clock = DateTime.Now;
        ManagerPassword = "AlCs1234";
        ManagerSalary = 400;
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
