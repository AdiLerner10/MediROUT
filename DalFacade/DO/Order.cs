namespace DO;
/// <summary>
/// Order Entity
/// </summary>
/// <param name="Id">the key is the Id</param>
/// <param name="TypeOfOrder">Order type, what kind of products we deliver</param>
/// <param name="Description">the explaination of the deliver</param>
/// <param name="Address">the address origin</param>
/// <param name="Latitude">latitude</param>
/// <param name="Longitude">longitude</param>
/// <param name="Name">Name of customer</param>
/// <param name="PhoneNumber">the customer phone number</param>
/// <param name="Fragile">is it breakeable or not</param>
/// <param name="volume">the volume of the package</param>
/// <param name="OrderStartTime">the time the customer started the order</param>
public record Order
(
   int Id,
   OrderType? TypeOfOrder,
   InOrOut? inOrOut,
   string Description,
   string Address,
   double Latitude,
   double Longitude,
   string Name,
   string PhoneNumber,
   bool Fragile,
   double volume,
   DateTime? OrderStartTime
)
{
public Order() : this(0,null, null, "","", 0, 0, "","", false, 0,null) { }
}
