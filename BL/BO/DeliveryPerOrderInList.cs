using Helpers;

namespace BO;

/// <summary>
/// ClosedDeliveryInList
/// </summary>
/// <param name="DeliveryId">Id of the delivery</param>
/// <param name="CourierId">Id of the courier</param>
/// <param name="CourierName">Couriers name</param>
/// <param name="OrderType">Order type</param>
/// <param name="InOrOut">Is the order incoming or outgoing</param>
/// <param name="DeliveryStartTime">the time the courier started the delivery</param>
/// <param name="OrderStatus">status of the order</param>
/// <param name="DeliveryEndTime">The time the delivery ended </param>
public class DeliveryPerOrderInList
{
    public int DeliveryId { get; init; }
    public int? CourierId { get; init; }
    public string CourierName { get; init; }
    public OrderType OrderType { get; init; }
    public InOrOut InOrOut { get; init; }
    public DateTime DeliveryStartTime {  get; init; }
    public OrderStatus? OrderStatus { get; init; }
    public DateTime? DeliveryEndTime {  get; init; }
    public override string ToString() => this.ToStringProperty();
}
