namespace DalApi;

/// <summary>
/// interface for all static veriabales
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }

    int ManagerId { get; set; }
    string? ManagerPassword { get; set; }
    double ManagerSalary { get; set; }
    double CourierSalary { get; set; }
    string CompanyAddress { get; set; }
    double? Longitude { get; set; }
    double? Latitude { get; set; }
    double? MaximumDistanceKM { get; set; }
    double? CarAvgSpeedKMH { get; set; }

    double? MotorcycleAvgSpeedKMH { get; set; }
    double? BicycleAvgSpeedKMH { get; set; }
    double? OnFootAvgSpeedKMH { get; set; }
    TimeSpan MaxTimeWindow { get; set; }
    TimeSpan RiskRange { get; set; }
    TimeSpan CourierNotActive { get; set; }

    void Reset();
}
