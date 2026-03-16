using Helpers;
using System.Reflection.Metadata.Ecma335;

namespace BO;

/// <summary>
/// ClosedDeliveryInList
/// </summary>
/// <param name="Id">Courier Id </param>
/// <param name="Name">Courier name </param>
/// <param name="PhoneNumber">Courier phone number</param>
/// <param name="Email">Courier email</param>
/// <param name="Password">Courier password</param>
/// <param name="Active">is the courier active or not </param>
/// <param name="MaxPersonalDistance">maximum distance the courier is willing to deliver</param>
/// <param name="Vehicle">type of vehicle the courier will be using </param>
/// <param name="EmploymentDate">The date the courier started working in the company</param>
/// <param name="NumOfProvidedOnTime">number of order delivered on time</param>
/// <param name="NumOfProvidedLate">number of orders delivered late</param>
/// <param name="ExistingOrder">does the courier have an existing order?</param>
public class Courier
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public double? Salary{ get; set; }
    public bool? Active { get; set; }
    public double? MaxPersonalDistance { get; set; }
    public  Vehicle Vehicle { get; set; }

    public DateTime EmploymentDate { get; init; }
    public int NumOfProvidedOnTime { get; init; }
    public int NumOfProvidedLate { get; init; }
    public OrderInProgress? ExistingOrder { get; set; }
    public override string ToString() => this.ToStringProperty();
}
