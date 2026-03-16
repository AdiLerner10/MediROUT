namespace BlApi;

/// <summary>
/// properties that implement the interfaces entities
/// </summary>
public interface IBl
{
    ICourier Courier { get; }
    IOrder Order { get; }
    IAdmin Admin { get; }
}
