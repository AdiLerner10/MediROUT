

namespace DalApi;

public interface IDal
{
    public ICourier Courier { get; }
    public IDelivery Delivery { get; }
    public IOrder Order { get; }
    public IConfig Config { get; }
    void ResetDB();


}
