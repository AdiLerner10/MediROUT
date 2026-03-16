
namespace DO;

/// <summary>
/// Courier entity
/// </summary>
/// <param name="Id">Id is the identity key</param>
/// <param name="VehicleType">the type of vehicle used to deliver</param>
/// <param name="Name">name of the courier</param>
/// <param name="Email">email of courier</param>
/// <param name="Password">user password</param>
/// <param name="Active">is the order active or not  </param>
/// <param name="MaxPersonalDistance">maximum distance of orders the courier will deliver</param>
/// <param name="EmploymentDate">couriers employment date</param>

public record Courier
(
    int Id,
    Vehicle? VehicleType=null,
    string PhoneNumber="",
    string Name="",
    string Email="",
    string Password="",
    bool Active=true,
    double? MaxPersonalDistance=0,
    double Salary=35,
    DateTime? EmploymentDate = null
)
{
    public Courier() : this(0) { }
}


