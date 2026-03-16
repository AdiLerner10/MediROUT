using Helpers;

namespace BO;

public class Config
{
    public DateTime Clock { get; set; } = DateTime.Now;
    public int ManagerId { get; set; } = 215868001;
    public string ManagerPassword { get; set; } = "AlCs1234";
    public double ManagerSalary { get; set; } = 400;
    public double CourierSalary { get; set; } = 35;
    public string? CompanyAddress { get; set; } = "12, Shmuel Bayit Street, Jerusalem";
    public double? MaximumDistanceKM { get; set; } = 500;
    public TimeSpan MaxTimeWindow { get; set; } = TimeSpan.FromDays(7);
    // the orders are delivered within a week

    public TimeSpan RiskRange { get; set; } = TimeSpan.FromDays(2);
    //two days before the last date the order can be delivered are "risk" days
    public TimeSpan CourierNotActive { get; set; } = TimeSpan.FromDays(14);

    public double? CarAvgSpeedKMH { get; set; } = 50;
    public double MotorcycleAvgSpeedKMH { get; set; } = 60;
    public double BicycleAvgSpeedKMH { get; set; } = 22;
    public double OnFootAvgSpeedKMH { get; set; } = 5;

    public override string ToString() => this.ToStringProperty();

}
