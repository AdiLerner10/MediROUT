using BO;
using System.Collections;
using System.Collections.Generic;
namespace PL.Helpers;
internal class CouriersCollection : IEnumerable
{
    static readonly IEnumerable<SortCourierByProperty> s_enums =
           (Enum.GetValues(typeof(SortCourierByProperty)) as
           IEnumerable<SortCourierByProperty>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
internal class VehicleCollection : IEnumerable
{
    static readonly IEnumerable<Vehicle> s_enums =
           (Enum.GetValues(typeof(Vehicle)) as
           IEnumerable<Vehicle>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class OrdersCollection : IEnumerable
{
    static readonly IEnumerable<OrderInListProperty> s_enums =
(Enum.GetValues(typeof(OrderInListProperty)) as IEnumerable<OrderInListProperty>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class InOrOutCollection : IEnumerable
{
    static readonly IEnumerable<InOrOut> s_inOrOuts =
(Enum.GetValues(typeof(InOrOut)) as IEnumerable<InOrOut>)!;

    public IEnumerator GetEnumerator() => s_inOrOuts.GetEnumerator();
}

public class OrderTypeCollection : IEnumerable
{
    static readonly IEnumerable<OrderType> s_orderTypes =
(Enum.GetValues(typeof(OrderType)) as IEnumerable<OrderType>)!;

    public IEnumerator GetEnumerator() => s_orderTypes.GetEnumerator();
}

public class ScheduleStatusCollection : IEnumerable
{
    static readonly IEnumerable<ScheduleStatus> s_scheduleStatuses =
(Enum.GetValues(typeof(ScheduleStatus)) as IEnumerable<ScheduleStatus>)!;

    public IEnumerator GetEnumerator() => s_scheduleStatuses.GetEnumerator();
}

internal class OpenOrderCollection : IEnumerable
{
    static readonly IEnumerable<OpenOrderProperty> s_enums =
(Enum.GetValues(typeof(OpenOrderProperty)) as IEnumerable<OpenOrderProperty>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class ClosedDeliveryPropertyCollection : IEnumerable
{
    static readonly IEnumerable<ClosedDeliveryProperty> s_ClosedDeliveryProperty =
(Enum.GetValues(typeof(ClosedDeliveryProperty)) as IEnumerable<ClosedDeliveryProperty>)!;

    public IEnumerator GetEnumerator() => s_ClosedDeliveryProperty.GetEnumerator();
}

public class OrderStatusCollection : IEnumerable
{
    static readonly IEnumerable<OrderStatus> s_OrderStatus =
(Enum.GetValues(typeof(OrderStatus)) as IEnumerable<OrderStatus>)!;

    public IEnumerator GetEnumerator() => s_OrderStatus.GetEnumerator();
}







