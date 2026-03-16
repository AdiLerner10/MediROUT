using Dal;
using DalApi;
using DO;
using System.Collections.Specialized;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace DalTest
{
    public enum MainMenu { Exit = 0, CourierMenu, DeliveryMenu, OrderMenu, Initialization, Print, ConfigMenu, Reset };
    public enum EntityMenu { Exit = 0, AddEntity, ReadEntity, ReadAllEntitys, UpdateEntity, Delete, DeleteAllEntity };
    public enum ConfigMenu { Exit = 0, ForwardByMinute, ForwardByHour, ForwardByDay, ForwardByMonth, ForwardByYear, ViewClock, ChangeValue, ViewValue, Reset };
    public enum ConfigChangeMenu { Clock = 1, ManagerPassword,ManagerSalary, CompanyAddress, Latitude, Longitude, MaximumDistanceKM, MaxTimeWindow, RiskRange, CourierNotActive };

    internal class Program
    {
        // static readonly IDal? s_dal = new DalList();// Stage 2
        // static readonly IDal s_dal = new DalXml();//Stage3
        static readonly IDal? s_dal = Factory.Get;

        private static readonly Random s_rand = new();//a random veriabale. it will be used in every function that needs a random value

        /// <summary>
        /// a function that print the main menu
        /// </summary>
        public static void MainMenu()
        {
            Console.WriteLine(@"***Main Menu***
0 - Exit
1 - Courier Menu
2 - Delivery Menu
3 - Order Menu
4 - Data Initialization
5 - Present Data
6 - Config Menu
7 - Reset Data ");
        }

        /// <summary>
        /// a function that print the menu for the selected entity
        /// </summary>
        /// <typeparam name="T">a template value for any entity(Courier, Delivery, Order)</typeparam>
        /// <param name="entity">the selected entity</param>
        public static void EntityMenu<T>(T entity)
        {
            Console.WriteLine($@"***{GetEntityType<T>(entity)} Menu***
0 - Exit
1 - Add New {GetEntityType(entity)}
2 - Read {GetEntityType(entity)} by Id
3 - Read All {GetEntityType(entity)}s
4 - Update {GetEntityType(entity)} by Id
5 - Delete {GetEntityType(entity)} by Id 
6 - Delete all {GetEntityType(entity)}s
");
        }

        /// <summary>
        /// a function thst add a new entity to it's data base
        /// </summary>
        /// <typeparam name="T">a template value for any entity(Courier, Delivery, Order)</typeparam>
        /// <param name="entity">add a new entity into it's list according to the sent entity</param>
        public static void AddEntity<T>(T entity)
        {
            if (entity!.GetType() == typeof(Courier))
            {
                Courier courier = CreateCourier();
                s_dal.Courier.Create(courier);
            }
            if (entity!.GetType() == typeof(Delivery))
            {
                Delivery delivery = CreateDelivery();
                s_dal.Delivery.Create(delivery);
            }
            if (entity!.GetType() == typeof(Order))
            {
                Order order = CreateOrder();
                s_dal.Order.Create(order);
            }
        }

        /// <summary>
        /// a function that create a new order
        /// </summary>
        private static Order CreateOrder()
        {
            //get values for every field in the order
            Order order = new Order();
            Console.WriteLine("Enter order type (1-Medicine, 2-machines, 3-OperatingRoomEquipment, 4-MobilityEquipment, 5-SterilazationEquipment, 6-HomeTests):");

            if (!int.TryParse(Console.ReadLine(), out int input))
                throw new DalUnSuccessfulCasting("int casting failed");
            OrderType temporderType = (OrderType)input;

            Console.WriteLine("Enter in or out: (0-in, 1-out)");
            if (!int.TryParse(Console.ReadLine(), out int tempInOr0ut))
                throw new DalUnSuccessfulCasting("int casting failed");

            InOrOut inOriut = (InOrOut)tempInOr0ut;

            Console.WriteLine("Enter Description:");
            string tempdescription = Console.ReadLine();

            Console.WriteLine("Enter Adress:");
            string tempAdress = Console.ReadLine();

            Console.WriteLine("Enter Longitude:");
            if (!double.TryParse(Console.ReadLine(), out double tempLongitude))
                throw new DalUnSuccessfulCasting("double casting failed");

            Console.WriteLine("Enter Latitude:");
            if (!double.TryParse(Console.ReadLine(), out double tempLatitude))
                throw new DalUnSuccessfulCasting("double casting failed");

            Console.WriteLine("Enter Name:");
            string tempName = Console.ReadLine();

            Console.WriteLine("Enter Phone Number:");
            string tempPhoneNumber = Console.ReadLine();

            Console.WriteLine("Is your package fragile? if yes press 1 if not press 0");
            if (!int.TryParse(Console.ReadLine(), out int tempFragile))
                throw new DalUnSuccessfulCasting("int casting failed");
            bool isFragile = false;
            if (tempFragile == 0)
                isFragile = false;
            else isFragile = true;

            Console.WriteLine("What is the volume of the package?");
            if (!int.TryParse(Console.ReadLine(), out int tempVolume))
                throw new DalUnSuccessfulCasting("int casting failed");

            Console.WriteLine("Please Enter start date in format dd/mm/yy hh:mm:ss");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
                throw new DalUnSuccessfulCasting("DateTime casting failed");

            //create a new order with the given values
            return new Order(order.Id, temporderType, inOriut, tempdescription, tempAdress, tempLongitude, tempLatitude, tempName, tempPhoneNumber, isFragile, tempVolume, start);
        }

        /// <summary>
        /// a function that create a new Courier 
        /// </summary>
        private static Courier CreateCourier()
        {
            int tempId;
            do
            {
                Console.WriteLine("Enter courier Id");
                if (!int.TryParse(Console.ReadLine(), out tempId))
                    throw new DalUnSuccessfulCasting("int casting failed");
            }
            while (s_dal.Courier.Read(tempId) != null);
            //get values for every field in the courier
            Console.Write("Enter a number (1-Car, 2-Motorcycle, 3-Bicycle, 4-OnFoot): ");
            if (!int.TryParse(Console.ReadLine(), out int tempVehicle))
                throw new DalUnSuccessfulCasting("int casting failed");
            Vehicle vehicle = (Vehicle)tempVehicle;

            Console.WriteLine("Enter Phone Number:");
            string tempPhone = Console.ReadLine();

            Console.WriteLine("Enter Name:");
            string tempName = Console.ReadLine();

            Console.WriteLine("Enter Email:");
            string tempEmail = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            string tempPassword = Console.ReadLine();


            Console.WriteLine("Enter the maximum distances you will deliver :");
            if (!int.TryParse(Console.ReadLine(), out int tempDistance))
                throw new DalUnSuccessfulCasting("int casting failed");

            Console.WriteLine("Please Enter start date in format (dd/mm/yy hh:mm:ss):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
                throw new DalUnSuccessfulCasting("DateTime casting failed");//efrat - is it a good exception?


            //create a new courier with the given values
            return new Courier(tempId, vehicle, tempPhone, tempName, tempEmail, tempPassword, true, tempDistance,35, start);

        }

        /// <summary>
        /// a function that create a new Delivery 
        /// </summary>
        private static Delivery CreateDelivery()
        {
            //get values for every field in the delivery
            Delivery delivery;
            int orderId;
            do
            {
                Console.WriteLine("Enter order Id");
                if (!int.TryParse(Console.ReadLine(), out int temoOrderId))
                    throw new DalUnSuccessfulCasting("int casting failed");
                orderId = temoOrderId;
            }
            while (s_dal.Order.Read(orderId) == null);

            int courierId;
            do
            {
                Console.WriteLine("Enter courier Id");
                if (!int.TryParse(Console.ReadLine(), out int tempCourierId))
                    throw new DalUnSuccessfulCasting("int casting failed");
                courierId = tempCourierId;
            }
            while (s_dal.Courier.Read(courierId) == null);

            Console.WriteLine("Enter distance for the delivery");

            if (!double.TryParse(Console.ReadLine(), out double distance))
                throw new DalUnSuccessfulCasting("double casting failed");

            Console.Write("Enter a number (1 - Delivered ,2 - Refuses,3 - Canceled,4 - NoRespond,5 - Faild,6 - InProcess: ");

            if (!int.TryParse(Console.ReadLine(), out int input))
                throw new DalUnSuccessfulCasting("int casting failed");
            OrderStatus end = (OrderStatus)input;


            Console.WriteLine("Please Enter start date in format (dd/mm/yy hh:mm:ss):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime start))
                throw new DalUnSuccessfulCasting("DateTime casting failed");


            Console.WriteLine("Please Enter end date in format (dd/mm/yy hh:mm:ss):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                throw new DalUnSuccessfulCasting("DateTime casting failed");



            Vehicle vehicle = (Vehicle)s_dal.Courier.Read(courierId)!.VehicleType!;

            //create a new delivery with the given values
            return new Delivery(0, orderId, courierId, distance, end, start, endDate, vehicle);

        }

        /// <summary>
        /// a function that return the name of the sent value type
        /// </summary>
        /// <typeparam name="T">a template value for any entity(Courier, Delivery, Order)</typeparam>
        /// <param name="entity">return the string according to the sent entity value</param>
        /// <returns></returns>
        internal static string GetEntityType<T>(T entity)
        {
            if (entity!.GetType() == typeof(Courier))
                return "Courier";
            if (entity!.GetType() == typeof(Delivery))
                return "Delivery";
            else
                return "Order";
        }

        public static void PrintEntity<T>(int id, T entity)
        {
            if (typeof(T) == typeof(Courier))
                PrintCourier(id);
            if (typeof(T) == typeof(Order))
                PrintOrder(id);
            if (typeof(T) == typeof(Delivery))
                PrintDelivery(id);

        }
        /// <summary>
        /// prints a delivery 
        /// </summary>
        /// <param name="id">delivery id </param>
        private static void PrintDelivery(int id)
        {

            Delivery delivery = s_dal.Delivery.Read(id);
            if (delivery != null)
                Console.WriteLine(delivery);
        }

        /// <summary>
        /// prints an order
        /// </summary>
        /// <param name="id">order id</param>
        private static void PrintOrder(int id)
        {

            Order order = s_dal.Order.Read(id);
            if (order != null)
                Console.WriteLine(order);
        }

        /// <summary>
        /// prints a courier
        /// </summary>
        /// <param name="id">courier id</param>
        private static void PrintCourier(int id)
        {
            Courier courier = s_dal.Courier.Read(id);
            if (courier != null)
                Console.WriteLine(courier);
        }

        /// <summary>
        /// Print config menu
        /// </summary>
        public static void ConfigMenu()
        {
            Console.WriteLine(@"***Config Menu***
0 - Exit
1 - Forward clock by a minute
2 - Forward clock by an hour
3 - Forward clock by a day
4 - Forward clock by a month
5 - Forward clock by a year
6 - View present time
7 - Change config value
8 - View Config Value
9 - Reset all config values 
");
        }

        /// <summary>
        /// a function that prints the config values
        /// </summary>
        public static void ConfigVeriabalesMenu()
        {
            Console.WriteLine(@"
1 - Clock 
2 - Manager Password 
3 - Company Address 
4 - Latitude
5 - Longitude
6 - Maximun DistanceKM
7 - Car Average Speed KMH
8 - Max Time Window
9 - Risk Range
10 - Courier Not Active");
        }

        /// <summary>
        /// a function that do different actions on the config entitys
        /// </summary>
        /// <param name="tempConfigChoose">the chosen action</param>
        /// <exception cref="FormatException">throwing an exception if there is a wrong input value</exception>
        private static void configActions(ConfigMenu tempConfigChoose)
        {
            switch (tempConfigChoose)
            {
                case DalTest.ConfigMenu.Exit:
                    break;
                case DalTest.ConfigMenu.ForwardByMinute:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case DalTest.ConfigMenu.ForwardByHour:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case DalTest.ConfigMenu.ForwardByDay:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                    break;
                case DalTest.ConfigMenu.ForwardByMonth:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                    break;
                case DalTest.ConfigMenu.ForwardByYear:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                    break;
                case DalTest.ConfigMenu.ViewClock:
                    Console.WriteLine(s_dal.Config.Clock);
                    break;
                case DalTest.ConfigMenu.ChangeValue:
                    Console.WriteLine(@"Enter the Value you want to change:");
                    ConfigVeriabalesMenu();
                    if (!int.TryParse(Console.ReadLine(), out int config))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    ChangeConfigFunc((ConfigChangeMenu)config);
                    Console.WriteLine();
                    break;
                case DalTest.ConfigMenu.ViewValue:
                    Console.WriteLine("Choose the object you want to watch");
                    ConfigVeriabalesMenu();
                    if (!int.TryParse(Console.ReadLine(), out int configWatch))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    ViewConfig((ConfigChangeMenu)configWatch);
                    break;
                case DalTest.ConfigMenu.Reset:
                    s_dal.Config.Reset();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// a function that print a chosen config entity
        /// </summary>
        /// <param name="configWatch">the chosen entity</param>
        public static void ViewConfig(ConfigChangeMenu configWatch)
        {
            switch (configWatch)
            {
                case ConfigChangeMenu.Clock:
                    Console.WriteLine(s_dal.Config.Clock);
                    break;
                case ConfigChangeMenu.ManagerPassword:
                    Console.WriteLine(s_dal.Config.ManagerPassword);
                    break;
                case ConfigChangeMenu.ManagerSalary:
                    Console.WriteLine(s_dal.Config.ManagerSalary);
                    break;
                case ConfigChangeMenu.CompanyAddress:
                    Console.WriteLine(s_dal.Config.CompanyAddress);
                    break;
                case ConfigChangeMenu.Latitude:
                    Console.WriteLine(s_dal.Config.Latitude);
                    break;
                case ConfigChangeMenu.Longitude:
                    Console.WriteLine(s_dal.Config.Longitude);
                    break;
                case ConfigChangeMenu.MaximumDistanceKM:
                    Console.WriteLine(s_dal.Config.MaximumDistanceKM);
                    break;
                case ConfigChangeMenu.MaxTimeWindow:
                    Console.WriteLine(s_dal.Config.MaxTimeWindow);
                    break;
                case ConfigChangeMenu.RiskRange:
                    Console.WriteLine(s_dal.Config.RiskRange);
                    break;
                case ConfigChangeMenu.CourierNotActive:
                    Console.WriteLine(s_dal.Config.CourierNotActive);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// a function that change a chosen config entity
        /// </summary>
        /// <param name="config">the entity you want to change</param>
        /// <exception cref="FormatException">throwing an exception if there is a wrong input value</exception>
        private static void ChangeConfigFunc(ConfigChangeMenu config)
        {
            switch (config)
            {
                case ConfigChangeMenu.Clock:
                    Console.WriteLine("Enter new Clock date in format dd/mm/yy hh:mm:ss");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime clock))
                        throw new DalUnSuccessfulCasting("DateTime casting failed");
                    s_dal.Config.Clock = clock;
                    break;
                case ConfigChangeMenu.ManagerPassword:
                    Console.WriteLine("Enter new password");
                    s_dal.Config.ManagerPassword = Console.ReadLine();
                    break;
                case ConfigChangeMenu.CompanyAddress:
                    Console.WriteLine("Enter new address");
                    s_dal.Config.CompanyAddress = Console.ReadLine();
                    break;
                case ConfigChangeMenu.Latitude:
                    Console.WriteLine("Enter new latitude");
                    if (!int.TryParse(Console.ReadLine(), out int latitude))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    s_dal.Config.Latitude = latitude;
                    break;
                case ConfigChangeMenu.Longitude:
                    Console.WriteLine("Enter new latitude");
                    if (!int.TryParse(Console.ReadLine(), out int longitude))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    s_dal.Config.Longitude = longitude;
                    break;
                case ConfigChangeMenu.MaximumDistanceKM:
                    Console.WriteLine("Enter new maximum distance");
                    if (!double.TryParse(Console.ReadLine(), out double distance))
                        throw new DalUnSuccessfulCasting("double casting failed");
                    s_dal.Config.MaximumDistanceKM = distance;
                    break;
                case ConfigChangeMenu.MaxTimeWindow:
                    Console.WriteLine("Enter new maximum time woindow in days");
                    if (!int.TryParse(Console.ReadLine(), out int days))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    s_dal.Config.MaxTimeWindow = TimeSpan.FromDays(days);
                    break;
                case ConfigChangeMenu.RiskRange:
                    Console.WriteLine("Enter new risk range in days");
                    if (!int.TryParse(Console.ReadLine(), out int risk))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    s_dal.Config.RiskRange = TimeSpan.FromDays(risk);
                    break;
                case ConfigChangeMenu.CourierNotActive:
                    Console.WriteLine("Enter new not active time in days");
                    if (!int.TryParse(Console.ReadLine(), out int active))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    s_dal.Config.CourierNotActive = TimeSpan.FromDays(active);
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// sends to the entitys update function
        /// </summary>
        /// <typeparam name="T">generic</typeparam>
        /// <param name="id">id of entity to update</param>
        /// <param name="entity">entity type</param>
        public static void UpdateEntity<T>(int id, T entity)
        {

            PrintEntity(id, entity);
            if (typeof(T) == typeof(Courier))
                UpdateCourier(id);
            if (typeof(T) == typeof(Order))
                UpdateOrder(id);
            if (typeof(T) == typeof(Delivery))
                UpdateDelivery(id);
        }

        /// <summary>
        /// updates a courier
        /// </summary>
        /// <param name="id">id of the courier we want to update</param>
        public static void UpdateCourier(int id)
        {
            Courier newCourier, courier;
            courier = CreateCourier();

            if (courier != null)
            {
                newCourier = courier with { Id = id };
                s_dal.Courier.Update(newCourier);
            }
        }

        /// <summary>
        /// updates an order
        /// </summary>
        /// <param name="id">id of the order to update </param>
        public static void UpdateOrder(int id)
        {
            Order newOrder, order;
            order = CreateOrder();

            if (order != null)
            {
                newOrder = order with { Id = id };
                s_dal.Order.Update(newOrder);
            }
        }

        public static void UpdateDelivery(int id)
        {
            Delivery newDelivery, delivery;
            delivery = CreateDelivery();

            if (delivery != null)
            {
                newDelivery = delivery with { Id = id };
                s_dal.Delivery.Update(newDelivery);
            }
        }

        /// <summary>
        /// sends to the delete function of the right entity
        /// </summary>
        /// <typeparam name="T">generic </typeparam>
        /// <param name="id">id of the entity</param>
        /// <param name="entity">entity type</param>
        public static void DeleteEntity<T>(int id, T entity)
        {
            if (typeof(T) == typeof(Courier))
                s_dal.Courier.Delete(id);
            if (typeof(T) == typeof(Order))
                s_dal.Order.Delete(id);
            if (typeof(T) == typeof(Delivery))
                s_dal.Delivery.Delete(id);
        }

        /// <summary>
        /// sends to the readall function of the right entity
        /// </summary>
        /// <typeparam name="T">generic</typeparam>
        /// <param name="entity">entity type</param>
        private static void ReadAllEntitys<T>(T? entity)
        {
            if (typeof(T) == typeof(Courier))
                Console.WriteLine(string.Join("\n", s_dal.Courier.ReadAll(func => true)));
            if (typeof(T) == typeof(Order))
                Console.WriteLine(string.Join("\n", s_dal.Order.ReadAll(func => true)));
            if (typeof(T) == typeof(Delivery))
                Console.WriteLine(string.Join("\n", s_dal.Delivery.ReadAll(func => true)));
        }

        /// <summary>
        /// options of all teh actions on entitys
        /// </summary>
        /// <typeparam name="T">generic item</typeparam>
        /// <param name="entityChoose1">choise</param>
        /// <param name="entity">entity type</param>
        private static void entityActions<T>(EntityMenu entityChoose1, T entity)
        {

            switch (entityChoose1)
            {
                case DalTest.EntityMenu.Exit:
                    break;
                case DalTest.EntityMenu.AddEntity:
                    AddEntity(entity);
                    break;
                case DalTest.EntityMenu.ReadEntity:
                    Console.WriteLine("Enter id of the object you want to read");
                    if (!int.TryParse(Console.ReadLine(), out int id))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    PrintEntity(id, entity);
                    break;
                case DalTest.EntityMenu.ReadAllEntitys:
                    ReadAllEntitys(entity);
                    break;
                case DalTest.EntityMenu.UpdateEntity:
                    Console.WriteLine($"Enter the id of the object you want to update");
                    if (!int.TryParse(Console.ReadLine(), out int UpdatedObjectId))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    UpdateEntity(UpdatedObjectId, entity);
                    PrintEntity(UpdatedObjectId, entity);
                    break;
                case DalTest.EntityMenu.Delete:
                    Console.WriteLine($"Enter the id of the object you want to delete");
                    if (!int.TryParse(Console.ReadLine(), out int deletedObjectId))
                        throw new DalUnSuccessfulCasting("int casting failed");
                    DeleteEntity(deletedObjectId, entity);
                    break;
                case DalTest.EntityMenu.DeleteAllEntity:
                    DeleteAllEntitys(entity);
                    break;
                    // default:
            }
        }
        /// <summary>
        /// sends to the deleteall function of the right entity
        /// </summary>
        /// <typeparam name="T">generic item type</typeparam>
        /// <param name="entity"></param>
        private static void DeleteAllEntitys<T>(T? entity)
        {
            if (typeof(T) == typeof(Courier))
                s_dal.Courier.DeleteAll();
            if (typeof(T) == typeof(Order))
                s_dal.Order.DeleteAll(); ;
            if (typeof(T) == typeof(Delivery))
                s_dal.Delivery.DeleteAll();
        }

        static void Main(string[] args)
        {
            MainMenu Choose;

            MainMenu();
            if (!int.TryParse(Console.ReadLine(), out int tempChoose))
                throw new DalUnSuccessfulCasting("int casting failed");
            Choose = (MainMenu)tempChoose;
            while (Choose != DalTest.MainMenu.Exit)
            {
                try
                {
                    switch (Choose)
                    {
                        case DalTest.MainMenu.Exit:
                            break;
                        case DalTest.MainMenu.CourierMenu:
                            EntityMenu(new Courier());

                            if (!int.TryParse(Console.ReadLine(), out int tempEntityChoose1))
                                throw new DalUnSuccessfulCasting("int casting failed");

                            EntityMenu entityChoose1 = (EntityMenu)tempEntityChoose1;
                            entityActions(entityChoose1, new Courier());
                            break;
                        case DalTest.MainMenu.DeliveryMenu:
                            EntityMenu(new Delivery());


                            if (!int.TryParse(Console.ReadLine(), out int tempEntityChoose2))
                                throw new DalUnSuccessfulCasting("int casting failed");

                            EntityMenu entityChoose2 = (EntityMenu)tempEntityChoose2;
                            entityActions(entityChoose2, new Delivery());
                            break;
                        case DalTest.MainMenu.OrderMenu:
                            EntityMenu(new Order());

                            if (!int.TryParse(Console.ReadLine(), out int tempEntityChoose3))
                                throw new DalUnSuccessfulCasting("int casting failed");

                            EntityMenu entityChoose3 = (EntityMenu)tempEntityChoose3;
                            entityActions(entityChoose3, new Order());
                            break;
                        case DalTest.MainMenu.Initialization:
                            Initialization.Do();
                            break;
                        case DalTest.MainMenu.Print:
                            Console.WriteLine(string.Join("\n", s_dal.Courier.ReadAll(func => true)));
                            Console.WriteLine(string.Join("\n", s_dal.Delivery.ReadAll(func => true)));
                            Console.WriteLine(string.Join("\n", s_dal.Order.ReadAll(func => true)));
                            break;
                        case DalTest.MainMenu.ConfigMenu:
                            ConfigMenu();
                            if (!int.TryParse(Console.ReadLine(), out int tempConfigChoose))
                                throw new DalUnSuccessfulCasting("int casting failed");

                            ConfigMenu configChoose = (ConfigMenu)tempConfigChoose;
                            configActions(configChoose);
                            break;
                        case DalTest.MainMenu.Reset:
                            s_dal.Config.Reset();
                            s_dal.Courier.DeleteAll();
                            s_dal.Delivery.DeleteAll();
                            s_dal.Order.DeleteAll();

                            Initialization.CreateCouriers();
                            Initialization.CreateOrders();
                            Initialization.CreateDelivery();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


                MainMenu();
                if (!int.TryParse(Console.ReadLine(), out int MainTempChoose))
                    throw new DalUnSuccessfulCasting("int casting failed");
                Choose = (MainMenu)MainTempChoose;
            }
        }
    }




}


