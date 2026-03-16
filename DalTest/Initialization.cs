namespace DalTest;
using Dal;
using DalApi;
using DO;
using System;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;

/// <summary>
/// Class of initialization values for orders' data base
/// </summary>
public static class Initialization
{
    private static readonly Random s_random = new();
    static IDal? s_dal = Factory.Get;

    /// <summary>
    /// Creates 120 orders.
    /// </summary>
    public static void CreateOrders()
    {
        //arrays of adresses, longitude, latitude etc of the orders
        string[] factoriesAddresses = new string[15] { "15 Rothschild Boulevard, Tel Aviv 6688118, Israel", "26 Dr. Shlomo Tzvi Ren St., Beer Sheva 8489326, Israel", "16 Yafo Street, Jerusalem 9414219, Israel", "3 Ben Yehuda Alley, Tel Aviv 6347101, Israel", "24 Shachrai St., Jerusalem 9647021, Israel", "12 Eder St., Haifa 3475211, Israel", "41 Shlomo Hamelech St., Netanya 4226701, Israel", "27 Koreh HaDorot St., Jerusalem 9338725, Israel", "206 HaRav Uziel St., Netanya 4225211, Israel", "49 Ibn Gabirol St., Tel Aviv 6436101, Israel", "8 HaAlon St., Dimona 8625501, Israel", "31 Sderot Shazar, Beer Sheva 8437001, Israel", "66 Kanfei Nesharim St., Jerusalem 9546442, Israel", "125 Menachem Begin St., Tel Aviv 6701201, Israel", "15 HaPalyam St., Haifa 3309511, Israel" };
        string[] factories = new string[15] { "Teva Pharmaceutical Industries Ltd.", "Taro Pharmaceutical Industries Ltd.", "Kamada Ltd.", "Astellas Pharma Israel", "Abbott Medical Laboratories Ltd.", "Dyn Diagnostics Ltd.", "ISI Ltd. (Israel Scientific Instruments)", "Mapi Pharma", "Padagis Israel Pharmaceuticals Ltd.", "Savyon Diagnostics Ltd.", "BMT Diagnostics Ltd.", "TestInsight Ltd.", "GE Healthcare Israel", "Medtechnica Ltd.", "Enco Scientific Services and Diagnostics" };
        double[] factoriesLatitude = new double[15] { 32.1152, 32.8016, 31.9059, 32.1030, 32.1121, 32.5031, 31.4727, 31.9365, 32.0991, 31.8153, 32.4912, 32.0831, 32.7936, 31.9077, 32.4340 };
        double[] factoriesLongitude = new double[15] { 34.8329, 35.0347, 34.8080, 34.9392, 34.8378, 34.9317, 34.7634, 34.7937, 34.8258, 34.6596, 34.9171, 34.8055, 34.9612, 35.0084, 34.9197 };
        string[] factoriesPhoneNumber = new string[15] { "03", "04", "08", "04", "03", "08", "04", "09", "03", "08", "04", "03", "04", "09", "09" };
        string[] descriptionsForFactories = new string[15] { "Pharmaceutical supplies (Teva Pharmaceutical Industries)", "Generic medications (Taro Pharmaceutical Industries)", "Blood plasma products (Kamada)", "Recombinant therapeutic proteins (Protalix BioTherapeutics)", "Antibiotic coatings for implants (PolyPid)", "Personalized oncology testing kits (OncoHost)", "Gastrointestinal medications (RedHill Biopharma)", "Cell therapy components (Pluristem Therapeutics)", "Hematology treatment vials (Gamida Cell)", "Aesthetic laser systems (Alma Lasers)", "Surgical and ophthalmic laser equipment (Lumenis)", "Patient monitoring systems (EarlySense)", "Focused ultrasound surgery devices (Insightec)", "Capsule endoscopy devices (Given Imaging)", "Cartilage regeneration implants (CartiHeal)" };

        string[] patientAddresses = new string[35] { "12 HaNevi'im St., Tel Aviv", "45 Herzl St., Haifa", "23 HaPalmach St., Jerusalem", "18 Hamered St., Ramat Gan", "56 Hatzoref St., Petach Tikva", "7 Sderot HaShalom, Be'er Sheva", "14 Hagefen St., Herzliya", "9 Habarzel St., Tel Aviv", "21 Hanegev St., Modi'in", "33 HaYarkon St., Netanya", "5 HaShikma St., Ashdod", "11 Hatzoref St., Kfar Saba", "26 HaGefen St., Holon", "38 HaDekel St., Eilat", "17 HaTsorf St., Rehovot", "44 HaEmek St., Ra'anana", "2 HaOranim St., Hadera", "29 HaHagana St., Acre", "8 HaBanim St., Givatayim", "13 HaTavor St., Kiryat Shmona", "10 HaRimon St., Bat Yam", "27 HaZayit St., Lod", "35 HaKalanit St., Nahariya", "4 HaEla St., Afula", "19 HaShikma St., Safed", "30 HaTzabar St., Tiberias", "22 HaGefen St., Nes Ziona", "41 HaHaroshet St., Ashkelon", "12 HaRakefet St., Ramat Hasharon", "7 HaDekel St., Migdal HaEmek", "24 HaAvoda St., Beit Shemesh", "31 HaGefen St., Kiryat Gat", "18 HaSharon St., Carmiel", "9 HaZayit St., Ramat Yishai", "16 HaAlon St., Yehud" };
        string[] patientsNames = new string[35] { "David Cohen", "Sarah Levi", "Daniel Rosenberg", "Rachel Friedman", "Aaron Goldberg", "Miriam Katz", "Eli Weiss", "Hannah Stein", "Jacob Adler", "Naomi Rubin", "Benjamin Klein", "Esther Cohen", "Samuel Berger", "Leah Kaplan", "Isaac Rosen", "Rebecca Shapiro", "Nathan Weissman", "Deborah Gold", "Jonah Friedman", "Tamar Levy", "Noah Rosenfeld", "Ruth Adler", "Gabriel Cohen", "Abigail Bernstein", "Simon Katz", "Maya Goldstein", "Adam Rubin", "Chaya Rosen", "Ethan Levi", "Miriam Adler", "Joshua Weiss", "Dina Kaplan", "Michael Cohen", "Sarah Friedman", "Daniel Katz" };
        double[] patientsLatitude = new double[35] { 32.0792, 32.8121, 31.7773, 32.0838, 32.0911, 31.2504, 32.1724, 32.1091, 31.8895, 32.1901, 31.8415, 32.1764, 31.9863, 29.5577, 31.9062, 32.1834, 32.4357, 32.9234, 32.0712, 33.2078, 31.9873, 31.9515, 33.0117, 32.6101, 32.9646, 32.7018, 31.9287, 31.6667, 32.1444, 32.6074, 31.7470, 31.6034, 32.9199, 32.6841, 32.0031 };
        double[] patientsLongitude = new double[35] { 34.7806, 34.9962, 35.2098, 34.8011, 34.8725, 34.8019, 34.8436, 34.8412, 34.8624, 34.8601, 34.6541, 34.9048, 34.7895, 34.9519, 34.8016, 34.8714, 34.9134, 35.0805, 34.8125, 35.5811, 34.7578, 34.8941, 35.0933, 35.2871, 35.4962, 35.5352, 34.8361, 34.5715, 34.8385, 35.2407, 34.9881, 34.7633, 35.2901, 35.1642, 34.8856 };
        string[] descriptionHomeMedications = new string[15] { "Home delivery of chronic medication refill", "Patient order for antibiotic treatment pack", "Monthly prescription delivery for blood pressure medication", "Insulin pens home shipment for diabetic patient", "Pain relief medication delivery following surgery", "Home delivery of asthma inhalers", "Patient request for heart medication refill", "Antidepressant prescription delivery to residence", "Home shipment of thyroid medication", "Patient order for allergy treatment pills", "Home delivery of immunosuppressant therapy", "Anticoagulant prescription refill for home use", "Patient order for cholesterol-lowering tablets", "Chronic pain management medication home delivery", "Monthly shipment of" };
        string[] descriptionHomeTests = new string[20] { "Home blood pressure monitoring kit", "At-home glucose test kit", "Home cholesterol test kit", "Urine analysis home test", "COVID-19 rapid antigen test kit", "At-home pregnancy test kit", "Home DNA / genetic test kit", "Home allergy test kit", "Home stool test kit", "Vitamin D home test kit", "Thyroid function home test kit", "Iron levels home test kit", "Hormone balance home test kit", "Food sensitivity home test kit", "Kidney function home test kit", "Liver function home test kit", "Celiac disease home test kit", "Menopause hormone home test kit", "Prostate PSA home test kit", "Inflammation (CRP) home test kit", };
        string[] patientsPhoneNumber = new string[35] { "02", "03", "04", "08", "09", "050", "057", "058", "02", "052", "053", "054", "055", "056", "02", "03", "04", "08", "09", "050", "052", "02", "03", "04", "08", "09", "052", "053", "054", "055", "056", "08", "09", "050", "052" };

        for (int i = 0; i < factories.Length; i++)//creates deliveries coming in to the hospital
        {
            OrderType orderType = (OrderType)(s_random.Next() % 6);//picks an order type from the enum
            string number = factoriesPhoneNumber[i] + s_random.Next(10000000, 100000000).ToString();
            bool isFragile = (s_random.Next() % 2 == 0);
            double volume = s_random.NextDouble() * 20;
            DateTime start = s_dal!.Config.Clock.AddDays(-28);
            int range = (s_dal.Config.Clock - start).Days;//makes a range of dates
            DateTime randomOrderDate = start.AddDays(s_random.Next(range))
                                .AddHours(s_random.Next(0, 24))
                                .AddMinutes(s_random.Next(0, 60));

            s_dal!.Order.Create(new(i + 1, orderType, InOrOut.In, descriptionsForFactories[i % factories.Length], factoriesAddresses[i], factoriesLatitude[i], factoriesLongitude[i], factories[i], number, isFragile, volume, randomOrderDate));
        }

        for (int i = 0; i < patientsNames.Length; i++)//creates deliveries that are being delivered from the hospital
        {
            OrderType orderType = (OrderType)(i % 2 == 0 ? 0 : 5);
            string description;
            if (orderType == OrderType.Medicine)
                description = descriptionHomeMedications[s_random.Next(0, 15)];
            else
                description = descriptionHomeTests[s_random.Next(0, 10)];
            DateTime start = s_dal!.Config.Clock.AddDays(-28);
            int range = (s_dal.Config.Clock - start).Days;//makes a range of dates
            DateTime randomOrderDate = start.AddDays(s_random.Next(range))
                                .AddHours(s_random.Next(0, 24))
                                .AddMinutes(s_random.Next(0, 60));

            string number = patientsPhoneNumber[i] + s_random.Next(1000000, 10000000).ToString();
            bool isFragile = (s_random.Next() % 2 == 0);
            double volume = s_random.NextDouble() * 20;

            s_dal.Order!.Create(new(i + 1, orderType, InOrOut.Out, description, patientAddresses[i], patientsLatitude[i], patientsLongitude[i], patientsNames[i], number, isFragile, volume, randomOrderDate));
        }

    }

    /// <summary>
    /// Creates deliveries.
    /// </summary>
    /// <exception cref="System.Exception">if there is a null time exception will be thrown</exception>
    public static void CreateDelivery()
    {
        int i = 0;
        double[] distances = new double[50] { 51.614, 116.019, 41.774, 52.811, 51.614, 63.676, 116.019, 32.326, 53.749, 55.485, 127.771, 54.918, 119.314, 32.326, 32.326, 55.221, 116.085, 1.853, 50.991, 47.543, 71.153, 57.192, 47.366, 24.915, 71.032, 55.788, 53.542, 50.606, 247.211, 41.924, 58.270, 80.459, 129.200, 52.011, 163.356, 53.154, 37.544, 138.255, 93.418, 135.118, 53.564, 56.250, 11.313, 27.719, 71.032, 55.788, 53.542, 50.606, 247.211, 41.924 };
        var couriers = s_dal.Courier.ReadAll(func => true).ToList();
        var orders = s_dal.Order.ReadAll(func => true).ToList();
        OrderStatus OrderStatus;
        HashSet<int> visited = new();
        foreach (var order in orders.Take(45))//matches orders to couriesr
        {
            DateTime start = s_dal.Config.Clock.AddDays(-28);
            DateTime orderCreated = (DateTime)order.OrderStartTime;
            int daysPassedSinceOrder = (s_dal.Config.Clock - orderCreated).Days;
            DateTime deliveryStarted = orderCreated.AddDays(s_random.Next(Math.Max(1, daysPassedSinceOrder)));

            int daysLeft = (s_dal.Config.Clock - deliveryStarted).Days;
            DateTime endDate = deliveryStarted.AddDays(s_random.Next(Math.Max(1, daysLeft)))
                                  .AddHours(s_random.Next(1, 11));

            OrderStatus = (OrderStatus)s_random.Next(Enum.GetValues(typeof(OrderStatus)).Length);
            if (OrderStatus != OrderStatus.InProcess)
            {
                // חישוב כמה ימים נשארו מתחילת המשלוח ועד עכשיו
                int remainingDays = (s_dal.Config.Clock - deliveryStarted).Days;

                // זמן הסיום יהיה אחרי תחילת המשלוח ובתוך הטווח שנותר
                endDate = deliveryStarted.AddDays(s_random.Next(Math.Max(1, remainingDays)));
            }

            int randomCourier=-1;
            Courier? courier = null;
            do
            {
                randomCourier = s_random.Next(couriers.Count);
                while (visited.Count < couriers.Count&& randomCourier==-1)
                {
                    randomCourier = s_random.Next(couriers.Count);
                }
                if (visited.Count == couriers.Count)
                {
                    randomCourier = -1;
                    break;
                }
                courier = couriers[randomCourier];
            }
            while (distances[i] > courier.MaxPersonalDistance);//checks if the distance is smaller than the courier agreed to.
            if (courier is not null)
            {
                s_dal.Delivery!.Create(new(i, order.Id, courier.Id, distances[i], OrderStatus, endDate, start, courier.VehicleType));
                visited.Add(randomCourier);
            }
            i++;
        }
        visited.Clear();
    }

    /// <summary>
    ///  Creates 120 Couriers.
    /// </summary>
    public static void CreateCouriers()//courier intialization
    {
        //array of names
        string[] courierNames = new string[20] { "Adi Lerner", "Chana Scheiner", "Isaac Rosen", "Sarah Kaplan", "Aaron Goldstein", "Miriam Katz", "Eli Friedman", "Leah Weiss", "Benjamin Adler", "Naomi Rosenberg", "Samuel Rubin", "Hannah Klein", "Jacob Stein", "Ruth Shapiro", "Daniel Weissman", "Esther Rosenfeld", "Jonah Cohen", "Rebecca Levine", "Simon Bernstein", "Talia Weiss" };      //array of passwords
        string[] courierPasswords = new string[20] { "QWxlcm5lcjMyMDYh", "Q3MxMjM0JCQ=", "fE7@vR9b12", "N1d$zT5u43", "B9n!W2xk76", "hK6#yP4a32", "Z5q%44tM3r", "yR8!gN1j56", "V3m$F7eH12", "kS2!wB9p76", "X7n@vR3d55", "cJ35!t4K1y", "pL4#s44T9", "mQ9@hC332v", "T6p#4zL8qs", "wE3!rN457k", "J1d$gB522x", "fR4@t5H6y6", "K83m!vD24p", "sN733#qF9a" };        //array of emails
        string[] phoneNumbers = new string[20] { "0501234567", "0522345678", "0533456789", "0544567890", "0555678901", "0566789012", "0577890123", "0588901234", "0599012345", "0500123456", "0512345678", "0523456789", "0534567890", "0545678901", "0556789012", "0567890123", "0578901234", "0589012345", "0590123456", "0501239876" };
        int[] idNumbers = new int[20] { 215868002, 328949722, 123456782, 234567895, 345678907, 456789012, 567890124, 678901236, 789012348, 890123450, 901234562, 112233445, 223344556, 334455668, 445566770, 556677882, 667788994, 778899106, 889910218, 991122330 };
        //creats a range of random dates untill todays date 
        Random random = new Random();
        DateTime start = s_dal.Config.Clock.AddDays(-28);
        DateTime end = s_dal.Config.Clock;

        for (int i = 0; i < courierNames.Length; i++)
        {
            string name = courierNames[i];//matches a name email and password
            string email = "adi.chana.project@gmail.com";
            string password = courierPasswords[i];
            string phoneNumber = phoneNumbers[i];
           
            double maxDistance = 0;
            Vehicle vehicleType;
            int id = idNumbers[i];
            vehicleType = (Vehicle)s_random.Next(Enum.GetValues(typeof(Vehicle)).Length);//picks a random vehicle

            switch (vehicleType)
            {
                case Vehicle.Car:
                    maxDistance = s_random.Next(5, 320);//picks a random number in the range for distance 
                    break;
                case Vehicle.Motorcycle:
                    maxDistance = s_random.Next(5, 320);//picks a random number in the range for distance 
                    break;
                case Vehicle.Bicycle:
                    maxDistance = s_random.Next(1, 5);//picks a random number in the range for distance 
                    break;
                case Vehicle.OnFoot:
                    maxDistance = (double)(s_random.NextDouble() * 2);//picks a random number in the range for distance 
                    break;
                default:
                    break;
            }

            double salary = (double)s_random.Next(35, 70);
            int range = (end - start).Days;
            DateTime randomDate = start.AddDays(random.Next(range))
                           .AddHours(random.Next(0, 24))
                           .AddMinutes(random.Next(0, 60))
                           .AddSeconds(random.Next(0, 60)); Courier courier1 = new Courier(id, vehicleType, phoneNumber, name, email, password, true, maxDistance, salary, randomDate);
            s_dal.Courier!.Create(courier1);
        }
    }

    public static void Do()
    {
        s_dal = DalApi.Factory.Get;

        Console.WriteLine("Reset Configuration values and List values..");
        s_dal.ResetDB();

        Console.WriteLine("Initializing couriers list..");
        CreateCouriers();

        Console.WriteLine("Initializing orders list..");
        CreateOrders();

        Console.WriteLine("Initializing deliveries list..");
        CreateDelivery();

    }

}
