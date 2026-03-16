namespace Dal;
using DalApi;
using DO;

sealed internal class DalXml : IDal
{
    /// <summary>
    /// readonly- can only be created once. Lazy<IDal>-always uses the same one.
    /// Lazy<IDal>(() => new DalList()- this will create the object when we call it.
    /// </summary>
    private static readonly Lazy<IDal> Instance1 = new Lazy<IDal>(() => new DalXml());

    /// <summary>
    /// returns the value of lazy instance to thread.
    /// </summary>
    public static IDal Instance=> Instance1.Value;
    private DalXml() { }    
    public ICourier Courier { get; } = new CourierImplementation();
    public IDelivery Delivery { get; } = new DeliveryImplementation();

    public IOrder Order { get; } = new OrderImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();
        Config.Reset();
    }
}
