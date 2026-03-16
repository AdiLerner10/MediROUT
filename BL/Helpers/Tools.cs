using BO;
using DalApi;
using System;
using System.Reflection;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Helpers;
static class Tools
{
    public static string TOKEN = "pk.be7a3a2d7a587b7e3de08da593f31e71";

    /// <summary>
    /// BONUS
    /// A function that aims to encrypt an existing password and return the encrypted string
    /// </summary>
    /// <param name="password">the password sent in order to be encrypted</param>
    /// <returns></returns>
    private const string Key = "MySuperSecretKey1234567890"; // The encryption key (must be 32 bytes/256 bits after transformation)
    private const string IV = "MyInitVector1234";          // The initialization vector (must be 16 bytes/128 bits after transformation)

    /// <summary>
    /// Encrypts a plain-text password using AES-256 (CBC mode, PKCS7 Padding) and returns the result as a Base64 string.
    /// </summary>
    /// <param name="password">The plain-text password to encrypt.</param>
    /// <returns>The encrypted password as a Base64 string.</returns>
    public static string Encryption(string? password)
    {
        if (password is null) return null;
        // Source - https://stackoverflow.com/a
        // Posted by Oli, modified by community. See post 'Timeline' for change history
        // Retrieved 2025-12-04, License - CC BY-SA 3.0

        //byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
        //data = SHA256.HashData(data);
        //return System.Text.Encoding.ASCII.GetString(data);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
    }


    /// <summary>
    /// Converts a string to a byte[] of a fixed length
    /// If the input is too short, pads with zeros at the end
    /// If the input is too long, truncates to the desired length
    /// </summary>
    private static byte[] GetFixedLengthBytes(string input, int length)
    {
        byte[] bytes = new byte[length];                          // Initialize an array with the desired length
        if (!string.IsNullOrEmpty(input))                         // If input is not null or empty
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);    // Convert the string to a byte[]
            Array.Copy(inputBytes, bytes, Math.Min(inputBytes.Length, length)); // Copy as much as possible
        }
        return bytes;                                             // Return the array with the exact length
    }

    /// <summary>
    /// to string property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ToStringProperty<T>(this T t)
    {
        string objectProperties = "";

        foreach (var prop in t.GetType().GetRuntimeProperties())
        {
            if (!prop.CanRead) continue;

            var type = prop.PropertyType;

            if (type.IsPrimitive || type == typeof(string) || type.IsEnum || type == typeof(TimeSpan))
            {
                var value = prop.GetValue(t); // עדיין צריך try/catch קטן
                objectProperties += $"{prop.Name}: {value}\n";
            }
        }
        /*
                if (t == null)
                    throw new ArgumentNullException("t");
                string objectProperties = "";
                foreach (var item in t.GetType().GetRuntimeProperties())
                {

                    objectProperties += item.Name + ": " + item.GetValue(t) + "\n";
                }*/
        return objectProperties;
    }



    /// <summary>
    /// A function that aims to sort the OrderInList list by a sent attribute.
    /// </summary>
    /// <param name="property">the given attribute</param>
    /// <param name="order">the given order to be sorted</param>
    /// <returns></returns>
    public static List<BO.OrderInList> GetOrdersInList(
    int id,
    OrderInListProperty? filter,
    object? filterValue,
    OrderInListProperty? sort)
    {
        var list = OrderManager.GetOrders()
                               .Select(OrderManager.ConvertToOrderInList)
                               .ToList();

        // Apply filter if exists
        if (filter != null && filterValue != null)
        {
            list = list.Where(o => GetPropertyValue(o, filter).Equals(filterValue))
                       .ToList();
        }

        // Apply sorting if exists
        if (sort != null)
        {
            list = list.OrderBy(o => GetPropertyValue(o, sort)).ToList();
        }

        return list;
    }

    // Helper to get property value dynamically
    private static object? GetPropertyValue(OrderInList order, OrderInListProperty? property)
    {
        return property switch
        {
            OrderInListProperty.OrderType => order.OrderType,
            OrderInListProperty.InOrOut => order.InOrOut,
            OrderInListProperty.AirDistance => order.AirDistance,
            OrderInListProperty.ScheduleStatus => order.ScheduleStatus,
            OrderInListProperty.RemainingTime => order.RemainingTime,
            OrderInListProperty.TotalTime => order.ToatalTime,
            OrderInListProperty.NumberOfDeliveries => order.NumberOfDeliveries,
            _ => null
        };
    }

    /// <summary>
    /// a function that checks if the sent password is correct
    /// </summary>
    /// <param name="username">the username</param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool CheckManagerPassword(int id, string password)
    {
        var cfg = new BO.Config();
        var encryptedPassword = Tools.Encryption(password);
        return encryptedPassword == Tools.Encryption(cfg.ManagerPassword) && id == cfg.ManagerId;
    }

    /// <summary>
    /// checks if the password is strong- 8 characters at least one big and one small letter 
    /// one number and one special character
    /// </summary>
    /// <param name="password">the password we are checking</param>
    /// <returns></returns>

    public static Object GetDeliveryProp(ClosedDeliveryInList delivery, ClosedDeliveryProperty? filter)
    {
        switch (filter)
        {
            case ClosedDeliveryProperty.DeliveryId:
                return delivery.DeliveryId;
            case ClosedDeliveryProperty.DeliveryTime:
                return delivery.OrderType;
            case ClosedDeliveryProperty.OrderId:
                return delivery.OrderId;
            case ClosedDeliveryProperty.Address:
                return delivery.Address;
            case ClosedDeliveryProperty.OrderStatus:
                return delivery.OrderStatus;
            case ClosedDeliveryProperty.ActualDistance:
                return delivery.VehicleType;
            default:
                return null;
        }
    }

    /// <summary>
    /// checkss if an orders status is currently in process or pending
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    internal static bool IsOpen(BO.Order order)
    {
        var deliveries = order.DeliveryPerOrderInList
            .FirstOrDefault(delivery => (delivery.OrderStatus == BO.OrderStatus.InProcess || delivery.OrderStatus == BO.OrderStatus.Delivered|| delivery.OrderStatus == BO.OrderStatus.Refuses|| delivery.OrderStatus == BO.OrderStatus.Canceled));
        if (deliveries != null) return false;
        return true;
    }

    /// <summary>
    /// changes a number in to radians
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    private static double ToRadians(double angle)
    {
        return angle * Math.PI / 180.0;
    }

    /// <summary>
    /// a function that activates a courier
    /// </summary>
    /// <param name="courier"></param>
    public static void UpdateActivation(CourierInList courier)
    {
        Config config = new Config();
        var deliveries = DeliveryManager.GetDeliveries().Where(delivery => delivery.CourierId == courier.Id);
        foreach (var delivery in deliveries)
        {
            if ((TimeSpan)(delivery.DeliveryEndTime - AdminManager.Now) > config.CourierNotActive)
            {
                var courierFromDO = CourierManager.GetCourier(courier.Id);
                courierFromDO.Active = false;
                CourierManager.UpdateCourier(courierFromDO);
            }
        }
    }

    /// <summary>
    /// return the speed depending on the vehicle type
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns></returns>
    internal static double SpeedCalculation(BO.Vehicle vehicle)
    {
        switch (vehicle)
        {
            case BO.Vehicle.Car:
                return (double)AdminManager.GetConfig().CarAvgSpeedKMH;
            case BO.Vehicle.Motorcycle:
                return (double)AdminManager.GetConfig().MotorcycleAvgSpeedKMH;
            case BO.Vehicle.Bicycle:
                return (double)AdminManager.GetConfig().BicycleAvgSpeedKMH;
            case BO.Vehicle.OnFoot:
                return (double)AdminManager.GetConfig().OnFootAvgSpeedKMH;
            default:
                return 50;
        }
    }

    /// <summary>
    /// checks if the password is strong enough
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="BlNotStrongPasswordException"></exception>
    public static bool IsStrongPassword(string password)
    {
        if (password is null || password.Length < 8)
            throw new BlNotStrongPasswordException("the password isn't strong");

        bool flag = false;
        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] >= 'A' && password[i] <= 'Z')
                flag = true;
        }
        if (!flag)
            throw new BlNotStrongPasswordException("the password isn't strong");

        flag = false;
        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] >= 'a' && password[i] <= 'z')
                flag = true;
        }
        if (!flag)
            throw new BlNotStrongPasswordException("the password isn't strong");

        flag = false;
        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] >= '0' && password[i] <= '9')
                flag = true;
        }
        if (!flag)
            throw new BlNotStrongPasswordException("the password isn't strong");

        flag = false;
        for (int i = 0; i < password.Length; i++)
        {
            if (password[i] >= 33 && password[i] <= 64)
                flag = true;
        }
        if (!flag)
            throw new BlNotStrongPasswordException("the password isn't strong");
        return true;
    }

    /// <summary>
    /// checks if id is valid
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="blInvalidInputException"></exception>
    internal static bool IsValidId(int id)
    {
        if (id < 100000000 || id > 999999999)
            throw new blInvalidInputException("Invalid Id");
        return true;
    }

    /// <summary>
    /// checks if email is valid
    /// </summary>
    /// <param name="email"></param>
    /// <exception cref="blInvalidInputException"></exception>
    internal static void IsValidEmail(string email)
    {
        bool found = false;
        for (int i = 0; i < email.Length; i++)
        {
            if (email[i] == '@')
                found = true;
        }
        if (!found)
            throw new blInvalidInputException("email must contain contain @");
    }

    /// <summary>
    /// checks if phone number is valid 
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <exception cref="blInvalidInputException"></exception>
    internal static void IsValidPhoneNumber(string phoneNumber)
    {
        for (int i = 0; i < phoneNumber.Length; i++)
            if (phoneNumber[i] < 48 || phoneNumber[i] > 57)
                throw new blInvalidInputException("Phone number must only contain numbers");
        if (phoneNumber.Length < 10 || phoneNumber.Length > 10)
            throw new blInvalidInputException("Phone number must have 10 digits");
        if (phoneNumber[0] != '0')
            throw new blInvalidInputException("Phone number must start with the number 0");
    }

    /// <summary>
    /// return the property to sort the list by
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    internal static Object SortByProperty(OrderInListProperty? filter, OrderInList order)
    {
        switch (filter)
        {
            case BO.OrderInListProperty.OrderType:
                return order.OrderType;
            case BO.OrderInListProperty.InOrOut:
                return order.InOrOut;
            case BO.OrderInListProperty.AirDistance:
                return order.AirDistance;
            case BO.OrderInListProperty.ScheduleStatus:
                return order.ScheduleStatus;
            case BO.OrderInListProperty.RemainingTime:
                return order.RemainingTime;
            case BO.OrderInListProperty.TotalTime:
                return order.ToatalTime;
            case BO.OrderInListProperty.NumberOfDeliveries:
                return order.NumberOfDeliveries;
            default:
                return order.OrderId;
        }
    }

    /// <summary>
    /// checks if the couriers max distance is bigger than the distance of the order
    /// </summary>
    /// <param name="courierId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    internal static async Task<bool> CheckDistance(int courierId, int orderId)
    {

        Courier courier = CourierManager.GetCourier(courierId);
        Order order = OrderManager.GetOrder(orderId);


        OrderManager.Location compLoc = await OrderManager.GetLocationCalcValues(AdminManager.GetConfig().CompanyAddress);
        OrderManager.Location orderLoc = await OrderManager.GetLocationCalcValues(order.Address);
        double speed = SpeedCalculation(courier.Vehicle);

        var distance = await OrderManager.CalculateRoute(orderLoc, compLoc);
        double distancKm = distance;
        return (courier.MaxPersonalDistance >= distancKm);
    }

    /// <summary>
    /// sends an email
    /// </summary>
    internal static class EmailService
    {
        private const string FromEmail = "adi.chana.project@gmail.com";//have to change to real email and password 
        private const string AppPassword = "eeme eavi agwi pxgh";

        internal static async Task Send(string to, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(FromEmail, AppPassword),
                EnableSsl = true
            };

            var mail = new MailMessage(FromEmail, to, subject, body);
            await smtp.SendMailAsync(mail);
        }
    }

    /// <summary>
    /// checks if a courier can be deleted
    /// </summary>
    /// <param name="courierId"></param>
    /// <returns></returns>
    internal static bool CanBeDeleted(int courierId)
    {
        return !DeliveryManager.GetDeliveries().Any(d => d.CourierId == courierId);

    }
}

