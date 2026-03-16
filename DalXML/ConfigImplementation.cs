namespace Dal;
using DalApi;

/// <summary>
/// implementation of Config interface
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// gets and sets the clock value 
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// gets and sets the manager Id
    /// </summary>
    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }

    /// <summary>
    /// gets and sets the manager password
    /// </summary>
    public string ManagerPassword
    {
        get => Config.ManagerPassword;
        set => Config.ManagerPassword = value;
    }

    /// <summary>
    /// gets and sets the manager salary
    /// </summary>
    public double? ManagerSalary
    {
        get => Config.ManagerSalary;
        set => Config.ManagerSalary = value;
    }

    /// <summary>
    /// gets and sets the manager salary
    /// </summary>
    public double? CourierSalary
    {
        get => Config.CourierSalary;
        set => Config.CourierSalary = value;
    }

    /// <summary>
    /// gets and sets the company adress
    /// </summary>
    public string CompanyAddress
    {
        get => Config.CompanyAddress!;
        set => Config.CompanyAddress = value;
    }

    /// <summary>
    /// gets and sets the longitude
    /// </summary>
    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }

    /// <summary>
    /// gets and sets the latitude
    /// </summary>
    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }

    /// <summary>
    /// gets and sets the maximum distance
    /// </summary>
    public double? MaximumDistanceKM
    {
        get => Config.MaximumDistanceKM;
        set => Config.MaximumDistanceKM = value;
    }

    /// <summary>
    /// gets and sets the average car speed
    /// </summary>
    public double? CarAvgSpeedKMH
    {
        get => Config.CarAvgSpeedKMH;
        set => Config.CarAvgSpeedKMH = value;
    }

    /// <summary>
    /// gets and sets the average Motorcycle speed
    /// </summary>
    public double? MotorcycleAvgSpeedKMH
    {
        get => Config.MotorcycleAvgSpeedKMH;
        set => Config.MotorcycleAvgSpeedKMH = value;
    }

    /// <summary>
    /// gets and sets the average bicycle speed
    /// </summary>
    public double? BicycleAvgSpeedKMH
    {
        get => Config.BicycleAvgSpeedKMH;
        set => Config.BicycleAvgSpeedKMH = value;
    }

    /// <summary>
    /// gets and sets the average onfoot speed
    /// </summary>
    /// 
    public double? OnFootAvgSpeedKMH
    {
        get => Config.OnFootAvgSpeedKMH;
        set => Config.OnFootAvgSpeedKMH = value;
    }

    /// <summary>
    /// gets and sets the maximum time window
    /// </summary>
    public TimeSpan MaxTimeWindow
    {
        get => Config.MaxTimeWindow;
        set => Config.MaxTimeWindow = value;
    }

    /// <summary>
    /// gets and sets the risk range
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// gets and sets courier not active
    /// </summary>
    public TimeSpan CourierNotActive
    {
        get => Config.CourierNotActive;
        set => Config.CourierNotActive = value;
    }

    /// <summary>
    /// gets and sets the manager salary
    /// </summary>
    double IConfig.ManagerSalary 
    {
        get => (double)Config.ManagerSalary;
        set => Config.ManagerSalary = value;
    }

    /// <summary>
    /// gets and sets the courier salary
    /// </summary>
    double IConfig.CourierSalary
    {
        get => (double)Config.CourierSalary;
        set => Config.CourierSalary = value;
    }

    /// <summary>
    /// Reset function
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}
