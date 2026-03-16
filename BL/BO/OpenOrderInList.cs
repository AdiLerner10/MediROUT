using Helpers;

namespace BO;

/// <summary>
/// Open order entity
/// </summary>
/// <param name="CourierId">Courier Id</param>
/// <param name="OrderId">Order Id</param>
/// <param name="OrderType">The type of the order.Medicine , Machines, OperatingRoomEquipment,and etc. </param>
/// <param name="Volume">The volume of the order</param>
/// <param name="Fragile">The fragiglity of the order</param>
/// <param name="Address">The destination of the order</param>
/// <param name="AirDistance">The air distance between the destination to the order origin</param>
/// <param name="ActualDistance">The actual distance between the destination to the order origin</param>
/// <param name="TimeEstimate">The estimated time for the delivery</param>
/// <param name="ScheduleStatus">The status of the schedule.in time or late</param>
/// <param name="RemainingTime">The time remained to deliver the order before it's counted int late</param>
/// <param name="MaxDeliveryTime">The maximum time the delivery can take</param>
public class OpenOrderInList
{
    public int? CourierId {  get; init; }
    public int OrderId {  get; init; }
    public OrderType OrderType { get; init; }
    public double Volume {  get; set; }
    public bool Fragile {  get; set; }
    public string Address { get; init; }
    public double AirDistance { get; init; }
    public double? ActualDistance {  get; init; }
    public TimeSpan? TimeEstimate { get; init; }
    public ScheduleStatus ScheduleStatus { get; init; }
    public TimeSpan RemainingTime { get; init; }
    public DateTime MaxDeliveryTime { get; init; }
    public override string ToString() => this.ToStringProperty();
}
