namespace Dal;
using DalApi;
using System.ComponentModel;

sealed internal class DalList : IDal
{
    /// <summary>
    /// readonly- can only be created once. Lazy<IDal>-always uses the same one.
    /// Lazy<IDal>(() => new DalList()- this will create the object when we call it.
    /// </summary>
    private static readonly Lazy<IDal> lazyInstance = new Lazy<IDal>(() => new DalList());

    /// <summary>
    /// returns the value of lazy instance to thread.
    /// </summary>
    public static IDal Instance => lazyInstance.Value;

    private DalList(){ }
    public ICourier Courier { get; } = new CourierImplementation();
    public IDelivery Delivery { get; } = new DeliveryImplementation();
    public IOrder Order { get; } = new OrderImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public void ResetDB()
    {
        Delivery.DeleteAll();
        Order.DeleteAll();
        Courier.DeleteAll();
        Config.Reset();
    }
   
}
