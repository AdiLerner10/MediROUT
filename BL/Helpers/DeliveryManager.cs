using BO;
using DalApi;
using DO;
using System;

namespace Helpers;

internal static class DeliveryManager
{
    private static IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new();

    /// <summary>
    /// a function that update the delivery status to "Completed".
    /// </summary>
    /// <param name="deliveryId">the id of the delivery to complete</param>
    internal static void Complete(int deliveryId, BO.OrderStatus status)
    {
        DO.Delivery delivery = (DO.Delivery)GetDelivery(deliveryId);
        try
        {
            lock (AdminManager.BlMutex)
            {
                s_dal.Delivery.Update(new Delivery
                {
                    Id = delivery.Id,
                    DeliveryStartTime = delivery.DeliveryStartTime,
                    CourierId = delivery.CourierId,
                    OrderId = delivery.OrderId,
                    Distance = delivery.Distance,
                    EndTime = AdminManager.Now,
                    VehicleType = delivery.VehicleType,
                    OrderStatus = (DO.OrderStatus)status,
                });
            }
            Observers.NotifyItemUpdated(deliveryId);
            Observers.NotifyListUpdated();
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException(ex.Message, ex);
        }
    }

    /// <summary>
    /// a function that creates a new delivery by given order and given courier.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="courierId"></param>
    /// <exception cref="BlAlreadyExistException"></exception>
    internal static async Task<DeliveryPerOrderInList> Create(BO.Order order, int courierId)
    {
        Delivery delivery;
        OrderManager.Location loc = await Helpers.OrderManager.GetLocationCalcValues(order.Address);
        OrderManager.Location compLoc = await Helpers.OrderManager.GetLocationCalcValues(s_dal.Config.CompanyAddress);
        double speed = Tools.SpeedCalculation(CourierManager.GetCourier(courierId).Vehicle);
        var distance = await OrderManager.CalculateRoute(loc, compLoc);
        DO.OrderStatus status;

        try
        {
            if (distance == 0)
                status = DO.OrderStatus.Faild;
            else
                status = DO.OrderStatus.InProcess;
            lock (AdminManager.BlMutex)
            {
                s_dal.Delivery.Create(new DO.Delivery
                {
                    CourierId = courierId,
                    DeliveryStartTime = AdminManager.Now,
                    EndTime = null,
                    OrderId = order.Id,
                    OrderStatus = status,
                    VehicleType = (DO.Vehicle)CourierManager.GetCourier(courierId).Vehicle,
                    Distance = distance,
                });

                delivery = s_dal.Delivery.
                   ReadAll().Where(delivery => delivery.CourierId == courierId && delivery.OrderId == order.Id && delivery.OrderStatus == DO.OrderStatus.InProcess).First()!;
            }
            DeliveryPerOrderInList deliveryPerOrder = new DeliveryPerOrderInList()
            {
                CourierId = courierId,
                DeliveryStartTime = AdminManager.Now,
                OrderStatus = (BO.OrderStatus)status,
                CourierName = CourierManager.GetCourier(courierId).Name,
                DeliveryId = delivery.Id
            };
            return deliveryPerOrder;
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BlAlreadyExistException(ex.Message, ex);
        }
    }

    /// <summary>
    /// a function that returns the deliverys list
    /// </summary>
    /// <returns></returns>
    /// <exception cref="blFunctionIsNull"></exception>
    internal static IEnumerable<BO.DeliveryPerOrderInList> GetDeliveries()
    {
        try
        {
            lock (AdminManager.BlMutex)
                return s_dal.Delivery.ReadAll().Select(delivery => ConvertToDeliveryPerOrderInList(delivery));

        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }

    /// <summary>
    /// a function that convert a delivery entity to be BO.DeliveryPerOrderInList entity.
    /// </summary>
    /// <param name="delivery"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    internal static DeliveryPerOrderInList ConvertToDeliveryPerOrderInList(Delivery delivery, DO.Order? order = null)
    {
        string? courierName = "";
        DO.Courier courier;
        lock (AdminManager.BlMutex)
        {
            try
            {
                courier = s_dal.Courier.Read(delivery.CourierId);
                courierName = s_dal.Courier.Read(delivery.CourierId)!.Name;
                order ??= s_dal.Order.Read(delivery.OrderId);
            }
            catch { }
        }

        return new DeliveryPerOrderInList
        {
            CourierId = delivery.CourierId,
            //!!!
            CourierName = courierName!,
            DeliveryId = delivery.Id,
            DeliveryStartTime = delivery.DeliveryStartTime ?? AdminManager.Now,
            InOrOut = (BO.InOrOut)order.inOrOut,
            OrderStatus = (BO.OrderStatus)delivery.OrderStatus,//order.OrderStatus,
            OrderType = (BO.OrderType)order.TypeOfOrder
        };
    }

    /// <summary>
    /// an external function for tha casting function, that gets an IEnumerable and convert oe by one.
    /// </summary>
    /// <param name="deliveries"></param>
    /// <returns></returns>
    internal static IEnumerable<DeliveryPerOrderInList> ConvertToDeliveryPerOrderInList(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Select(d => ConvertToDeliveryPerOrderInList(d));
    }

    /// <summary>
    /// a function that returns a DO delivery by id.
    /// </summary>
    /// <param name="deliveryId"></param>
    /// <returns></returns>
    internal static DO.Delivery GetDelivery(int deliveryId)
    {
        try
        {
            lock (AdminManager.BlMutex)
                return s_dal.Delivery.Read(deliveryId)!;
        }
        catch (DalFunctionIsNull ex)
        {
            throw new blFunctionIsNull(ex.Message, ex);
        }
    }

    internal static void PeriodicDeliveriesUpdates(DateTime oldClock, DateTime newClock)
    {

    }

    /// <summary>
    /// a function that returns the value of the sent "sort" type.
    /// </summary>
    /// <param name="delivery"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    internal static object SortTypeDelivery(ClosedDeliveryInList delivery, BO.ClosedDeliveryProperty sort)
    {
        switch (sort)
        {
            case BO.ClosedDeliveryProperty.DeliveryId:
                return delivery.DeliveryId;
            case BO.ClosedDeliveryProperty.OrderId:
                return delivery.OrderType;
            case BO.ClosedDeliveryProperty.Address:
                return delivery.VehicleType;
            case BO.ClosedDeliveryProperty.ActualDistance:
                return delivery.ActualDistance!;
            case BO.ClosedDeliveryProperty.DeliveryTime:
                return delivery.DeliveryTime;
            case BO.ClosedDeliveryProperty.OrderStatus:
                return delivery.OrderStatus!;
            default:
                return null;
        }
    }

    /// <summary>
    /// a function that returns the Deliveries from the Dal layer.
    /// </summary>
    /// <returns></returns>
    internal static IEnumerable<Delivery> GetDoDeliveries()
    {
        lock (AdminManager.BlMutex)
            return s_dal.Delivery.ReadAll();
    }
}
