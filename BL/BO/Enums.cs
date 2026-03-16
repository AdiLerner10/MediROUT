namespace BO;

/// <summary>
///The type of vehicle being used for the order
/// </summary>
public enum Vehicle { Car, Motorcycle, Bicycle, OnFoot };

/// <summary>
/// The status of the delivery
/// </summary>
public enum OrderStatus { Pending, Delivered, Refuses, Canceled, NoRespond, Faild, InProcess };

/// <summary>
/// is the order incoming or outgoing
/// </summary>
public enum InOrOut { In, Out, None };

/// <summary>
/// Type of order being delivered
/// </summary>
public enum OrderType { Medicine, Machines, OperatingRoomEquipment, MobilityEquipment, SterilazationEquipment, HomeTests, None };

/// <summary>
/// is the order running on time or not
/// </summary>
public enum ScheduleStatus { RunningOnTime, RunningLate, InRisk, None }

/// <summary>
/// ways to sort the Courier list
/// </summary>
public enum SortCourierByProperty { Id, Name, EmploymentDate, maxPersonalDistance, NumOfProvidedOnTime, None }

public enum OrderInListProperty { OrderId, DeliveryId, OrderType, InOrOut, AirDistance, ScheduleStatus, RemainingTime, TotalTime, NumberOfDeliveries, None }
/// <summary>
/// an enum for the ClosedDeliveryInList entity  
/// </summary>
public enum ClosedDeliveryProperty { DeliveryId, OrderId, Address, ActualDistance, DeliveryTime, OrderStatus, None }

/// <summary>
/// an enum for the OpenOrderInList entity
/// </summary>
public enum OpenOrderProperty { CourierId, OrderId, OrderType, Volume, Fragile, Address, AirDistance, ActualDistance, TimeEstimate, ScheduleStatus, RemainingTime, MaxDeliveryTime, None }

/// <summary>
/// enum for Time units
/// </summary>
public enum TimeUnit { Second, Minute, Hour, Day, Month, Year }
