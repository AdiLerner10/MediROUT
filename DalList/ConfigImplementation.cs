using DalApi;

namespace Dal;

/// <summary>
/// implementation of Config interface
/// </summary>
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }

    public string ManagerPassword
    {
        get => Config.ManagerPassword;
        set => Config.ManagerPassword = value;
    }

    public double? ManagerSalary
    {
        get => Config.ManagerSalary;
        set => Config.ManagerSalary = value;
    }

    public double? CourierSalary
    {
        get => Config.CourierSalary;
        set => Config.CourierSalary = value;
    }

    public string CompanyAddress
    {
        get => Config.CompanyAddress!;
        set => Config.CompanyAddress = value;
    }

    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }

    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }

    public double? MaximumDistanceKM
    {
        get => Config.MaximumDistanceKM;
        set => Config.MaximumDistanceKM = value;
    }

    public double? CarAvgSpeedKMH
    {
        get => Config.CarAvgSpeedKMH;
        set => Config.CarAvgSpeedKMH = value;
    }

    public double? MotorcycleAvgSpeedKMH
    {
        get => Config.MotorcycleAvgSpeedKMH;
        set => Config.MotorcycleAvgSpeedKMH = (double)value;
    }

    public double? BicycleAvgSpeedKMH
    {
        get => Config.BicycleAvgSpeedKMH;
        set => Config.BicycleAvgSpeedKMH = (double)value;
    }

    public double? OnFootAvgSpeedKMH
    {
        get => Config.OnFootAvgSpeedKMH;
        set => Config.OnFootAvgSpeedKMH = (double)value;
    }
    public TimeSpan MaxTimeWindow
    {
        get => Config.MaxTimeWindow;
        set => Config.MaxTimeWindow = value;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public TimeSpan CourierNotActive
    {
        get => Config.CourierNotActive;
        set => Config.CourierNotActive = value;
    }
    double IConfig.ManagerSalary
    {
        get => (double)Config.ManagerSalary;
        set => Config.ManagerSalary = value;
    }
    double IConfig.CourierSalary
    {
        get => (double)Config.CourierSalary;
        set => Config.CourierSalary = value;
    }

    //Reset function
    public void Reset()
    {
        Config.Reset();
    }
}
