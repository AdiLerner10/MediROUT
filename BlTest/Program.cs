using BO;
using DalApi;
using DO;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BlTest;
public enum MainMenu { EXIT, CourierMenu, AdminMenu, OrderMenu, DataInitialization, PresentData, ResetData }
public enum CourierMenu { MainMenu, AddCourier, CourierEntrance, DeleteCourier, GetCourier, GetCourierList, UpdateCourier }
public enum OrderMenu { MainMenu, AddOrder, Cancel, DeleteOrder, UpdateOrder, GetOrder, GetOpenOrders, GetClosedOrder, GetOrdersInList, OrderAmountByStatus, OrderCompletion, OrderToHandle }
public enum AdminMenu { MainMenu, ForwardClock, GetClock, GetConfig, InitializeDB, ResetDB, SetConfig }
public enum ConfigValues { Done, ManagerId, ManagerPassword, ManagerSalary, CourierSalary, CompanyAddress, MaximumDistanceKM, MaxTimeWindow, RiskRange, CourierNotActive }
internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static readonly int managerId = s_bl.Admin.GetConfig().ManagerId;

    private static void Main(string[] args)
    {
        MainMenu choice = PrintMainMenu();

        while (choice != MainMenu.EXIT)
        {
            try
            {
                switch (choice)
                {
                    case MainMenu.CourierMenu:
                        CourierMenu courierChoice = CourierMenu();
                        CourierActions(courierChoice);
                        break;
                    case MainMenu.OrderMenu:
                        OrderMenu orderChoice = OrderMenu();
                        OrderActions(orderChoice);
                        break;
                    case MainMenu.AdminMenu:
                        AdminMenu adminChoice = AdminMenu();
                        AdminActions(adminChoice);
                        break;
                    case MainMenu.DataInitialization:
                        s_bl.Admin.InitializeDB();
                        break;
                    case MainMenu.PresentData:
                        Console.WriteLine(s_bl.Admin.GetConfig());
                        break;
                    case MainMenu.ResetData:
                        s_bl.Admin.ResetDB();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            choice = PrintMainMenu();
        }
    }

    /// <summary>
    /// a function that performs a chosen admin action.
    /// </summary>
    /// <param name="adminChoice">the chosen admin action to perform</param>
    private static void AdminActions(AdminMenu adminChoice)
    {
        switch (adminChoice)
        {
            case BlTest.AdminMenu.ForwardClock:
                Console.WriteLine("Enter time unit(0 - Second, 1 - Minute, 2 - Hour, 3 - Day, 4 - Month, 5 - Year):");
                if (!BO.TimeUnit.TryParse(Console.ReadLine(), out BO.TimeUnit unit))
                    throw new blUnSuccessfulCasting("double casting failed");
                s_bl.Admin.ForwardClock(unit);
                break;
            case BlTest.AdminMenu.GetClock:
                Console.WriteLine(s_bl.Admin.GetClock());
                break;
            case BlTest.AdminMenu.GetConfig:
                Console.WriteLine(s_bl.Admin.GetConfig());
                break;
            case BlTest.AdminMenu.InitializeDB:
                s_bl.Admin.InitializeDB();
                break;
            case BlTest.AdminMenu.ResetDB:
                s_bl.Admin.ResetDB();
                break;
            case BlTest.AdminMenu.SetConfig:
                BO.Config config = SetConfig();
                s_bl.Admin.SetConfig(config);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// a function that create a new config values and update them in the BL.
    /// </summary>
    /// <returns>the new config</returns>
    private static Config SetConfig()
    {
        Config newConfig = new Config();
        ConfigValuesMenu();
        if (!ConfigValues.TryParse(Console.ReadLine(), out ConfigValues choose))
            throw new blUnSuccessfulCasting("ConfigValues casting failed");
        while (choose != ConfigValues.Done)
        {
            switch (choose)
            {
                case ConfigValues.ManagerId:
                    Console.WriteLine("Enter new manager Id:");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                        throw new blUnSuccessfulCasting("int casting failed");
                    newConfig.ManagerId = id;
                    break;
                case ConfigValues.ManagerPassword:
                    Console.WriteLine("Enter new manager Password(must include 8 characters, at least one capital letter, one lower case letter, one number and one special character):");
                    newConfig.ManagerPassword = Console.ReadLine();
                    break;
                case ConfigValues.ManagerSalary:
                    Console.WriteLine("Enter new manager salary:");
                    if (!double.TryParse(Console.ReadLine(), out double managerSalary))
                        throw new blUnSuccessfulCasting("double casting failed");
                    newConfig.ManagerSalary = managerSalary;
                    break;
                case ConfigValues.CourierSalary:
                    Console.WriteLine("Enter new courier salary:");
                    if (!int.TryParse(Console.ReadLine(), out int courierSalary))
                        throw new blUnSuccessfulCasting("int casting failed");
                    newConfig.CourierSalary = courierSalary;
                    break;
                case ConfigValues.CompanyAddress:
                    Console.WriteLine("Enter new company address:");
                    newConfig.CompanyAddress = Console.ReadLine();
                    break;
                case ConfigValues.MaximumDistanceKM:
                    Console.WriteLine("Enter new maximum distance kilometer:");
                    if (!int.TryParse(Console.ReadLine(), out int maxDistance))
                        throw new blUnSuccessfulCasting("int casting failed");
                    newConfig.MaximumDistanceKM = maxDistance;
                    break;
                case ConfigValues.MaxTimeWindow:
                    Console.WriteLine("Enter new maximum time window:");
                    if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan maxTimeWindow))
                        throw new blUnSuccessfulCasting("TimeSpan casting failed");
                    newConfig.MaxTimeWindow = maxTimeWindow;
                    break;
                case ConfigValues.RiskRange:
                    Console.WriteLine("Enter new risk range:");
                    if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan riskRange))
                        throw new blUnSuccessfulCasting("TimeSpan casting failed");
                    newConfig.RiskRange = riskRange;
                    break;
                case ConfigValues.CourierNotActive:
                    Console.WriteLine("Enter new time for courier activity demand:");

                    if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan courierNotActive))
                        throw new blUnSuccessfulCasting("TimeSpan casting failed");
                    newConfig.CourierNotActive = courierNotActive;
                    break;
                default:
                    break;
            }
            ConfigValuesMenu();
            if (!ConfigValues.TryParse(Console.ReadLine(), out ConfigValues chooseInput))
                throw new blUnSuccessfulCasting("ConfigValues casting failed");
            choose = chooseInput;
        }
        return newConfig;
    }

    /// <summary>
    /// a function that prints the config action options.
    /// </summary>
    private static void ConfigValuesMenu()
    {
        Console.WriteLine(@"***Confog Options***
0  - Done
1  - ManagerId
2  - ManagerPassword
3  - ManagerSalary
4  - CourierSalary
5  - CompanyAddress
6  - MaximumDistanceKM
7  - MaxTimeWindow
8  - RiskRange
9  - CourierNotActive
10 - CarAvgSpeedKMH
11 - MotorcycleAvgSpeedKMH
12 - BicycleAvgSpeedKMH
13 - OnFootAvgSpeedKMH");
    }

    private static void OrderActions(OrderMenu orderChoice)
    {
        switch (orderChoice)
        {
            case BlTest.OrderMenu.MainMenu:
                break;
            case BlTest.OrderMenu.AddOrder:
                AddOrder();
                break;
            case BlTest.OrderMenu.Cancel:
                CancelOrder();
                break;
            case BlTest.OrderMenu.DeleteOrder:
                DeleteOrder();
                break;
            case BlTest.OrderMenu.GetOrder:
                GetOrder();
                break;
            case BlTest.OrderMenu.GetOpenOrders:
                GetOpenedOrders();
                break;
            case BlTest.OrderMenu.GetClosedOrder:
                GetClosedOrders();
                break;
            case BlTest.OrderMenu.GetOrdersInList:
                GetOrdersInList();
                break;
            case BlTest.OrderMenu.OrderAmountByStatus:
                OrderAmountByStatus();
                break;
            case BlTest.OrderMenu.OrderCompletion:
                OrderCompletion();
                break;
            case BlTest.OrderMenu.OrderToHandle:
                OrderToHandle();
                break;
            case BlTest.OrderMenu.UpdateOrder:
                UpdateOrder();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// updates an existing order
    /// </summary>
    private static void UpdateOrder()
    {
        Console.WriteLine("Enter Id of order to update");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine(@"Enter Order Type:
0 - Medicine
1 - Machines
2 - OperatingRoomEquipment
3 - MobilityEquipment
4 - SterilazationEquipment
5 - HomeTests ");
        if (!BO.OrderType.TryParse(Console.ReadLine(), out BO.OrderType orderType))
            throw new blUnSuccessfulCasting("BO.OrderType casting failed");

        Console.WriteLine("Enter 0 for incoming order and 1 for outgoing order");
        if (!BO.InOrOut.TryParse(Console.ReadLine(), out BO.InOrOut inOrOut))
            throw new blUnSuccessfulCasting("BO.InOrOut casting failed");

        Console.WriteLine("Enter description");
        string description = Console.ReadLine();

        Console.WriteLine("Enter address");
        string address = Console.ReadLine();

        Console.WriteLine("Enter Client Name");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Phone Number");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Enter the package volume");
        if (!double.TryParse(Console.ReadLine(), out double volume))
            throw new blUnSuccessfulCasting("double casting failed");

        Console.WriteLine("Enter 0 if package is fragile, 1 if not");
        if (!int.TryParse(Console.ReadLine(), out int fragileChoose))
            throw new blUnSuccessfulCasting("int casting failed");
        bool fragile = fragileChoose == 0 ? true : false;


        BO.Order orderToUpdate = new BO.Order()
        {
            OrderType = orderType,
            InOrOut = inOrOut,
            Description = description,
            Address = address,
            ClientName = name,
            ClientPhoneNumber = phoneNumber,
            Volume = volume,
            Fragile = fragile,
            OrderStatus = BO.OrderStatus.Pending,
            ScheduleStatus = ScheduleStatus.RunningOnTime,
            DeliveryPerOrderInList = null
        };
        s_bl.Order.Update(managerId, orderToUpdate);
    }

    /// <summary>
    /// handles an order
    /// </summary>
    private static void OrderToHandle()
    {
        Console.WriteLine("Enter courier id");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
            throw new blUnSuccessfulCasting("int casting failed");

        Console.WriteLine("Enter order id");
        if (!int.TryParse(Console.ReadLine(), out int deliveryId))
            throw new blUnSuccessfulCasting("int casting failed");

        s_bl.Order.OrderToHandle(managerId, courierId, deliveryId);
    }

    /// <summary>
    /// completes an order
    /// </summary>
    private static void OrderCompletion()
    {
        Console.WriteLine("Enter courier id");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
            throw new blUnSuccessfulCasting("int casting failed");

        Console.WriteLine("Enter order id");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
            throw new blUnSuccessfulCasting("int casting failed");

        s_bl.Order.OrderCompletion(managerId, courierId, orderId, BO.OrderStatus.Delivered);
    }

    /// <summary>
    /// returns the amount of orders with a certain status
    /// </summary>
    private static void OrderAmountByStatus()
    {
        Console.WriteLine("Enter Id to get amount");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("int casting failed");

        s_bl.Order.OrderAmountsByStatus(id);
    }

    /// <summary>
    /// gets orders with a certain filter and a property to sort by
    /// </summary>
    private static async Task GetOrdersInList()
    {
        Console.WriteLine(@"Enter order in list property to filter by
0 - DeliveryId
1 - OrderId
2 - OrderType
3 - InOrOut
4 - AirDistance
5 - ScheduleStatus
6 - RemainingTime
7 - ToatalTime
8 - NumberOfDeliveries");
        if (!OrderInListProperty.TryParse(Console.ReadLine(), out BO.OrderInListProperty orderInListPropertyToFilter))
            throw new blUnSuccessfulCasting("BO.OrderInListProperty  casting failed");

        object chosenValue;
        switch (orderInListPropertyToFilter)
        {
            case BO.OrderInListProperty.DeliveryId:
                Console.WriteLine("Enter delivery id to filter by");
                if (!int.TryParse(Console.ReadLine(), out int deliveryIdValue))
                    throw new blUnSuccessfulCasting("int  casting failed");
                chosenValue = deliveryIdValue;
                break;
            case BO.OrderInListProperty.OrderId:
                Console.WriteLine("Enter order id to filter by");
                if (!int.TryParse(Console.ReadLine(), out int orderIdValue))
                    throw new blUnSuccessfulCasting("int  casting failed");
                chosenValue = orderIdValue;
                break;
            case BO.OrderInListProperty.OrderType:
                Console.WriteLine(@"Enter order Type to filter by
0 - Medicine
1 - Machines
2 - OperatingRoomEquipment
3 - MobilityEquipment
4 - SterilazationEquipment
5 - HomeTests");
                if (!BO.OrderType.TryParse(Console.ReadLine(), out BO.OrderType orderTypeValue))
                    throw new blUnSuccessfulCasting("BO.OrderType casting failed");
                chosenValue = orderTypeValue;
                break;
            case BO.OrderInListProperty.InOrOut:
                Console.WriteLine("Enter 0 for incoming 1 for outgoing");
                if (!BO.InOrOut.TryParse(Console.ReadLine(), out BO.InOrOut inOrOutValue))
                    throw new blUnSuccessfulCasting("BO.InOrOut casting failed");
                chosenValue = inOrOutValue;
                break;
            case BO.OrderInListProperty.AirDistance:
                Console.WriteLine("Enter air distance");
                if (!double.TryParse(Console.ReadLine(), out double airDistanceValue))
                    throw new blUnSuccessfulCasting("double casting failed");
                chosenValue = airDistanceValue;
                break;
            case BO.OrderInListProperty.ScheduleStatus:
                Console.WriteLine(@"Enter schedule status
0 - RunningOnTime
1 - RunningLate
2 - InRisk ");
                if (!ScheduleStatus.TryParse(Console.ReadLine(), out ScheduleStatus scheduleStatusValue))
                    throw new blUnSuccessfulCasting("ScheduleStatus casting failed");
                chosenValue = scheduleStatusValue;
                break;
            case BO.OrderInListProperty.RemainingTime:
                Console.WriteLine("Enter remaining time in format dd/mm/yyyy hh:mm");
                if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan remainingTimeValue))
                    throw new blUnSuccessfulCasting("TimeSpan casting failed");
                chosenValue = remainingTimeValue;
                break;
            case BO.OrderInListProperty.TotalTime:
                Console.WriteLine("Enter total time dd/mm/yyyy hh:mm");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime toatalTimeValue))
                    throw new blUnSuccessfulCasting("DateTime casting failed");
                chosenValue = toatalTimeValue;
                break;
            case BO.OrderInListProperty.NumberOfDeliveries:
                Console.WriteLine("Enter number of deliveries");
                if (!int.TryParse(Console.ReadLine(), out int numberOfDeliveriesValue))
                    throw new blUnSuccessfulCasting("int casting failed");
                chosenValue = numberOfDeliveriesValue;
                break;
            default:
                chosenValue = null;
                break;
        }
        Console.WriteLine(@"Enter order in list property to sort by
0 - DeliveryId
1 - OrderId
2 - OrderType
3 - InOrOut
4 - AirDistance
5 - ScheduleStatus
6 - RemainingTime
7 - ToatalTime
8 - NumberOfDeliveries");
        if (!BO.OrderInListProperty.TryParse(Console.ReadLine(), out BO.OrderInListProperty orderInListPropertyToSort))
            throw new blUnSuccessfulCasting("int casting failed");
        var deliveries = (await s_bl.Order.GetOrdersInList(managerId, orderInListPropertyToFilter, chosenValue, orderInListPropertyToSort));
        foreach (var item in deliveries)
        {
            Console.WriteLine(item);
        }
    }

    /// <summary>
    /// gets closed orders with specific conditions
    /// </summary>
    private static void GetClosedOrders()
    {
        Console.WriteLine("Enter courier Id");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
            throw new blUnSuccessfulCasting("int casting failed");

        Console.WriteLine(@"Enter Order Type to filter by
0 - Medicine
1 - Machines
2 - OperatingRoomEquipment
3 - MobilityEquipment
4 - SterilazationEquipment
5 - HomeTests");
        if (!BO.OrderType.TryParse(Console.ReadLine(), out BO.OrderType orderType))
            throw new blUnSuccessfulCasting("BO.OrderType casting failed");
        Console.WriteLine(@"Enter closed order property to sort by:
0 - DeliveryId
1 - OrderId
2 - OrderType
3 - Address
4 - Vehicle
5 - ActualDistance
6 - MaxDeliveryTime
7 - OrderStatus ");
        if (!BO.ClosedDeliveryProperty.TryParse(Console.ReadLine(), out BO.ClosedDeliveryProperty closedDeliveryProperty))
            throw new blUnSuccessfulCasting("BO.ClosedDeliveryProperty casting failed");
        var result = s_bl.Order.GetClosedOrders(managerId, courierId, orderType, closedDeliveryProperty);
        result.ToList().ForEach(r => Console.WriteLine(r));
    }

    /// <summary>
    /// gets opened orders with specific conditions
    /// </summary>
    private static async Task GetOpenedOrders()
    {
        Console.WriteLine("Enter courier Id");
        if (!int.TryParse(Console.ReadLine(), out int courierId))
            throw new blUnSuccessfulCasting("int casting failed");
        Console.WriteLine(@"Enter Order Type to filter by
0 - Medicine
1 - Machines
2 - OperatingRoomEquipment
3 - MobilityEquipment
4 - SterilazationEquipment
5 - HomeTests");
        if (!BO.OrderType.TryParse(Console.ReadLine(), out BO.OrderType orderType))
            throw new blUnSuccessfulCasting("BO.OrderType casting failed");

        Console.WriteLine(@"Enter open order property to sort by:
0  - CourierId
1  - OrderId
2  - OrderType
3  - Volume
4  - Fragile
5  - Address
6  - AirDistance
7  - ActualDistance
8  - TimeEstimate
9  - ScheduleStatus
10 - RemainingTime
11 - MaxDeliveryTime");
        if (!BO.OpenOrderProperty.TryParse(Console.ReadLine(), out BO.OpenOrderProperty openOrderProperty))
            throw new blUnSuccessfulCasting("BO.OpenOrderProperty casting failed");
        PrintList(await s_bl.Order.GetOpenOrders(courierId, orderType, openOrderProperty));
    }

    /// <summary>
    /// gets an order with a specific id 
    /// </summary>
    private static void GetOrder()
    {
        Console.WriteLine("Enter Id of order");
        if (!int.TryParse(Console.ReadLine(), out int idToGet))
            throw new blUnSuccessfulCasting("int casting failed");

        Console.WriteLine(s_bl.Order.GetOrder(managerId, idToGet));
    }

    /// <summary>
    /// deletes an order
    /// </summary>
    private static void DeleteOrder()
    {
        Console.WriteLine("Enter Id of order to delete");
        if (!int.TryParse(Console.ReadLine(), out int idToDelete))
            throw new blUnSuccessfulCasting("int casting failed");

        s_bl.Order.Delete(managerId, idToDelete);
    }

    /// <summary>
    /// 
    /// els an order
    /// </summary>
    private static void CancelOrder()
    {
        Console.WriteLine("Enter Id of the order to cancel");
        if (!int.TryParse(Console.ReadLine(), out int idToCancel))
            throw new blUnSuccessfulCasting("int casting failed");

        s_bl.Order.Cancel(managerId, idToCancel);
    }

    /// <summary>
    /// adds an order
    /// </summary>
    private static void AddOrder()
    {
        Console.WriteLine(@"Enter Order Type:
0 -  Medicine
1 - Machines
2 - OperatingRoomEquipment
3 - MobilityEquipment
4 - SterilazationEquipment
5 - HomeTests ");
        if (!BO.OrderType.TryParse(Console.ReadLine(), out BO.OrderType orderType))
            throw new blUnSuccessfulCasting("OrderType casting failed");

        Console.WriteLine("Enter 0 for incoming order and 1 for outgoing order");
        if (!BO.InOrOut.TryParse(Console.ReadLine(), out BO.InOrOut inOrOut))
            throw new blUnSuccessfulCasting("BO.InOrOut casting failed");

        Console.WriteLine("Enter description");
        string description = Console.ReadLine();

        Console.WriteLine("Enter address");
        string address = Console.ReadLine();

        Console.WriteLine("Enter Client Name");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Phone Number");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Enter the package volume");
        if (!double.TryParse(Console.ReadLine(), out double volume))
            throw new blUnSuccessfulCasting("double casting failed");

        Console.WriteLine("Enter 0 if package is fragile, 1 if not");
        if (!int.TryParse(Console.ReadLine(), out int fragileChoose))
            throw new blUnSuccessfulCasting("int casting failed");
        bool fragile = fragileChoose == 0 ? true : false;
        BO.Order newOrder = new BO.Order(orderType, inOrOut, description, address, name, phoneNumber, volume, fragile);
        s_bl.Order.Add(managerId, newOrder);

    }

    /// <summary>
    /// prints the courier actions
    /// </summary>
    /// <param name="courierChoice"></param>
    private static void CourierActions(CourierMenu courierChoice)
    {
        switch (courierChoice)
        {
            case BlTest.CourierMenu.MainMenu:
                break;
            case BlTest.CourierMenu.AddCourier:
                AddCourier();
                break;
            case BlTest.CourierMenu.CourierEntrance:
                CourierEntrance();
                break;
            case BlTest.CourierMenu.DeleteCourier:
                DeleteCourier();
                break;
            case BlTest.CourierMenu.GetCourier:
                GetCourier();
                break;
            case BlTest.CourierMenu.GetCourierList:
                var list = GetCourierList();
                PrintList(list);
                break;
            case BlTest.CourierMenu.UpdateCourier:
                UpdateCourier();
                break;
            default:
                break;
        }
    }

    public static void PrintList<T>(IEnumerable<T> list)
    {
        list = list.ToList();
        foreach (var item in list.Take(3))
        {
            Console.WriteLine(item);
        }
    }

    /// <summary>
    /// updates a courier
    /// </summary>
    private static void UpdateCourier()
    {
        BO.Courier newCourier;
        Console.WriteLine("Enter Id");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("double casting failed");

        Console.WriteLine("Enter Courier Name");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Courier Phone Number");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Enter Courier Email");
        string email = Console.ReadLine();

        Console.WriteLine("Enter Courier Password (must include 8 characters, at least one capital letter, one lower case" +
         "letter, one number and one special character");
        string password = Console.ReadLine();
        Console.WriteLine("Enter Courier Max Personal Distance");

        if (!double.TryParse(Console.ReadLine(), out double maxPersonalDistance))
            throw new blUnSuccessfulCasting("double casting failed");

        Console.WriteLine(@"Enter Vehicle
0 - car
1 - motorcycle
2 - bicycle
3 - on foot");
        if (!BO.Vehicle.TryParse(Console.ReadLine(), out BO.Vehicle vehicle))
            throw new blUnSuccessfulCasting(" BO.Vehicle casting failed");

        BO.Courier courierToUpdate = new BO.Courier
        {
            Id = id,
            Name = name,
            PhoneNumber = phoneNumber,
            Email = email,
            Password = password,
            MaxPersonalDistance = maxPersonalDistance,
            Vehicle = vehicle
        };
        s_bl.Courier.UpdateCourier(managerId, courierToUpdate);
    }

    /// <summary>
    /// gets a list of courier with a property to sort by and a filter
    /// </summary>
    private static IEnumerable<CourierInList> GetCourierList()
    {
        Console.WriteLine(@"Enter 0 for non active couriers
Enter 1 for active couriers
Enter 2 for both");
        if (!int.TryParse(Console.ReadLine(), out int isActiveChoice))
            throw new blUnSuccessfulCasting("int casting failed");
        bool? isActive;
        switch (isActiveChoice)
        {
            case 0:
                isActive = false;
                break;
            case 1:
                isActive = true;
                break;
            default:
                isActive = null;
                break;
        }
        Console.WriteLine(@"enter criterion to sort by
0 - to sort by Id
1 - to sort by name
2 - to sort by employment date
3 - to sort by maximum personal distance
4 - to sort by number of provided on time"
);
        if (!BO.SortCourierByProperty.TryParse(Console.ReadLine(), out BO.SortCourierByProperty sortBy))
            throw new blUnSuccessfulCasting("BO.SortCourierByProperty casting failed");

        return s_bl.Courier.GetCourierList(managerId, isActive, sortBy);
    }

    /// <summary>
    /// gets a courier with the given id
    /// </summary>
    private static void GetCourier()
    {
        Console.WriteLine("Enter Id of courier");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("int casting failed");
        Console.WriteLine(s_bl.Courier.GetCourier(managerId, id));
    }

    private static void DeleteCourier()
    {
        Console.WriteLine("Enter id of courier to delete");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("int casting failed");
        s_bl.Courier.DeleteCourier(managerId, id);
    }

    /// <summary>
    /// checks courier entrance
    /// </summary>
    private static void CourierEntrance()
    {
        Console.WriteLine("Enter Id");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("int casting failed");
        Console.WriteLine("Enter Password");
        string password = Console.ReadLine();
        s_bl.Courier.CourierEntrance(id, password);
        Console.WriteLine("entrance succeded");
    }

    /// <summary>
    /// Adds a courier 
    /// </summary>
    private static void AddCourier()
    {
        BO.Courier newCourier;
        Console.WriteLine("Enter Id");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new blUnSuccessfulCasting("int casting failed");

        Console.WriteLine("Enter Courier Name");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Courier Phone Number");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Enter Courier Email");
        string email = Console.ReadLine();

        Console.WriteLine("Enter Courier Password (must include 8 characters, at least one capital letter, one lower case" +
         "letter, one number and one special character");
        string password = Console.ReadLine();

        Console.WriteLine("Enter Courier Max Personal Distance");
        if (!double.TryParse(Console.ReadLine(), out double maxPersonalDistance))
            throw new blUnSuccessfulCasting("double casting failed");

        Console.WriteLine(@"Enter Vehicle
0 - car
1 - Motorcycle
2 - Bicycle
3 - On foot ");
        if (!BO.Vehicle.TryParse(Console.ReadLine(), out BO.Vehicle vehicle))
            throw new blUnSuccessfulCasting("BO.Vehicle casting failed");

        BO.Courier courierToAdd = new BO.Courier
        {
            Id = id,
            Name = name,
            PhoneNumber = phoneNumber,
            Email = email,
            Password = password,
            MaxPersonalDistance = maxPersonalDistance,
            Vehicle = vehicle
        };
        s_bl.Courier.AddCourier(managerId, courierToAdd);
    }

    /// <summary>
    /// prints the admin menu
    /// </summary>
    /// <returns></returns>
    private static AdminMenu AdminMenu()
    {
        Console.WriteLine(@"***Admin Menu***
0 - Main Menu
1 - Forward Clock
2 - Get Clock
3 - Get Config
4 - Initialize DB
5 - Set Config"
);

        if (!BlTest.AdminMenu.TryParse(Console.ReadLine(), out AdminMenu choose))
            throw new blUnSuccessfulCasting("AdminMenu casting failed");
        return choose;
    }

    /// <summary>
    /// prints the order menu
    /// </summary>
    /// <returns></returns>
    private static OrderMenu OrderMenu()
    {
        Console.WriteLine(@"***Order Menu***
0  - Main Menu
1  - Add Order
2  - Cancel Order
3  - Delete Order
4  - Update Order
5  - Get Order
6  - Get Open Orders
7  - Get Closed Orders
8  - Get Orders in List
9  - Order Amount by Status
10 - Order Completion
11 - Order to Handle"
);


        if (!BlTest.OrderMenu.TryParse(Console.ReadLine(), out OrderMenu choose))
            throw new blUnSuccessfulCasting("OrderMenu casting failed");
        return choose;
    }

    /// <summary>
    /// prints the courier menu
    /// </summary>
    /// <returns></returns>
    private static CourierMenu CourierMenu()
    {
        Console.WriteLine(@" ***Courier Menu***
0 - Main Menu
1 - Add Courier
2 - Courier Entrance
3 - Delete Courier
4 - Get Courier
5 - Get Courier List
6 - Update Courier"
);
        if (!BlTest.CourierMenu.TryParse(Console.ReadLine(), out CourierMenu choose))
            throw new blUnSuccessfulCasting("CourierMenu casting failed");
        return choose;
    }

    /// <summary>
    /// Prints the Main main and returns the choice 
    /// </summary>
    /// <returns></returns>
    private static MainMenu PrintMainMenu()
    {
        Console.WriteLine(@"***Main Menu***
0 - Exit
1 - Courier Menu
2 - Admin Menu
3 - Order Menu
4 - Data Initialization
5 - Present Data
6 - Reset Data ");
        if (!BlTest.MainMenu.TryParse(Console.ReadLine(), out MainMenu choose))
            throw new blUnSuccessfulCasting("MainMenu casting failed");
        return choose;
    }


}