using Helpers;

namespace BO;

/// <summary>
/// ClosedDeliveryInList
/// </summary>
/// <param name="Id">Courier Id </param>
/// <param name="Name">Courier name </param>
/// <param name="Active">Courier phone number</param>
/// <param name="OrderType">The vehicle the courier will be delivering with</param>
/// <param name="EmploymentDate">Employment Date</param>
/// <param name="NumberOfOrdersDeliveredOnTime">number of orders the courier delivered on time</param>
/// <param name="NumberOfOrdersDeliveredLate">number of orders the courier delivered late</param>
/// <param name="NumberOfCurrentOrderId">number of id of an order that is still in progress</param>
  public class CourierInList
{
    public int Id{ get; init; }
    public string Name {  get; init; }
    public double? Salary{  get; set; }
    public bool Active {  get; init; }
    public Vehicle VehicleType { get; init; }
    public DateTime EmploymentDate { get; init; }
    public int NumberOfOrdersDeliveredOnTime {  get; init; }
    public int NumberOfOrdersDeliveredLate {  get; init; }
    public int? NumberOfCurrentOrderId { get; init; }
    public override string ToString() => this.ToStringProperty();

}
