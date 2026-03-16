using Helpers;

namespace BO;

/// <summary>
/// ClosedDeliveryInList
/// </summary>
/// <param name="DeliveryId">Id of the delivery key</param>
/// <param name="OrderId">Id of the order</param>
/// <param name="OrderType">Type of order</param>
/// <param name="Address">Order address</param>
/// <param name="VehicleType">type of vehicle used for the order</param>
/// <param name="ActualDistance">actual distance of the order</param>
/// <param name="MaxDeliveryTime">maximum delivery time</param>
/// <param name="OrderStatus">how the order ended </param>
public class ClosedDeliveryInList
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public string Address { get; init; }
    public Vehicle VehicleType { get; init; }
    public double? ActualDistance { get; init; }
    public TimeSpan DeliveryTime { get; init; }
    public OrderStatus? OrderStatus { get; init; }
    public override string ToString() => this.ToStringProperty();
}
