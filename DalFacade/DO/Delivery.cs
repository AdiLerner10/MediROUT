namespace DO;

/// <summary>
/// Delivery class
/// </summary>
/// <param name="Id">the identity key of the delivery object</param>
/// <param name="OrderId">the order Id number</param>
/// <param name="CourierId">the Id of the courier</param>
/// <param name="Distance">the distance between the destination of the delivery to it's origin</param>
/// <param name="OrderStatus">the way the delivery ended</param>
/// <param name="EndTime">the time the delivery ended</param>
/// <param name="DeliveryStartTime">the time the delivery started</param>
 /// <param name="VehicleType">the vehicle used to deliver</param>
public record Delivery
(
    int Id = 0,   
    int OrderId = 0,
    int CourierId=0,
    double Distance=0,
    OrderStatus? OrderStatus = null,
    DateTime? EndTime = null,
    DateTime ? DeliveryStartTime = null,
    Vehicle ? VehicleType=null
)
{
    public Delivery() : this(0) { }//constructor
}

