using DalApi;
using DO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Dal;

internal class CourierImplementation : ICourier
{

    static Courier getCourier(XElement s)
    {
        return new DO.Courier()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            PhoneNumber=(string ?)s.Element("PhoneNumber") ?? "",
            Name = (string?)s.Element("Name") ?? "",
            Active = (bool?)s.Element("Active") ?? false,
            Email = (string?)s.Element("Email") ?? "",
            Password = (string?)s.Element("Password") ?? "",
            EmploymentDate = s.ToDateTimeNullable("EmploymentDate"),
            VehicleType = (Vehicle?)(s.ToIntNullable("VehicleType") ?? 1),
            MaxPersonalDistance = s.ToIntNullable("MaxPersonalDistance"),
        };
    }

    static XElement toXElement(DO.Courier c)
    {
        return new XElement("Courier",
            new XElement("Id", c.Id),
            new XElement("PhoneNumber", c.PhoneNumber ?? ""),
            new XElement("Name", c.Name ?? ""),
            new XElement("Active", c.Active),
            new XElement("Email", c.Email ?? ""),
            new XElement("Password", c.Password ?? ""),
            new XElement("EmploymentDate", c.EmploymentDate?.ToString("o") ?? ""), // ISO 8601 format
            new XElement("VehicleType", (int?)c.VehicleType ?? 1), // ממירים ל-int בשביל XML
            new XElement("MaxPersonalDistance", c.MaxPersonalDistance ?? 0)
        );
    }

    /// <summary>
    /// Reads a courier with a specific Id from the xml file
    /// </summary>
    /// <param name="id">id of the courier to read</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id)
    {
        XElement? courierElem =
    XMLTools.LoadListFromXMLElement(Config.s_couriers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return courierElem is null ? null : getCourier(courierElem);
    }

    /// <summary>
    /// reads the first courier witha  a specific filter from the xml file 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(Func<Courier, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_couriers_xml).Elements().Select(s => getCourier(s)).FirstOrDefault(filter);
    }

    /// <summary>
    /// updates a courier from the xml file
    /// </summary>
    /// <param name="item">item to update</param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        XElement courierRootElement = XMLTools.LoadListFromXMLElement(Config.s_couriers_xml);

        (courierRootElement.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Courier with ID={item.Id} does Not exist"))
                .Remove();

        courierRootElement.Add(toXElement(item));

        XMLTools.SaveListToXMLElement(courierRootElement, Config.s_couriers_xml);
    }

    /// <summary>
    /// Craetes a courier and adds it to the xml file
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DO.DalAlreadyExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        XElement courierRootElement = XMLTools.LoadListFromXMLElement(Config.s_couriers_xml);

        if (courierRootElement.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id) != null)
            throw new DO.DalAlreadyExistException($"Courier with ID={item.Id} already exist");
        courierRootElement.Add(toXElement(item));

        XMLTools.SaveListToXMLElement(courierRootElement, Config.s_couriers_xml);
    }

    /// <summary>
    /// deletes a courier with a specific id from the xml file
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DO.DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement courierRootElement = XMLTools.LoadListFromXMLElement(Config.s_couriers_xml);
        (courierRootElement.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id)
        ?? throw new DO.DalDoesNotExistException($"Courier with ID={id} does Not exist"))
                .Remove();
        XMLTools.SaveListToXMLElement(courierRootElement, Config.s_couriers_xml);
    }

    /// <summary>
    /// Deletes all the couriers from the xml file
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement courierRootElement = XMLTools.LoadListFromXMLElement(Config.s_couriers_xml);
        courierRootElement.RemoveAll();
        XMLTools.SaveListToXMLElement(courierRootElement, Config.s_couriers_xml);

    }

    /// <summary>
    /// reads all the courier with a specific filter from the xml file
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        IEnumerable<Courier> couriers = XMLTools.LoadListFromXMLElement(Config.s_couriers_xml)
                                         .Elements()
                                         .Select(s => getCourier(s));
        if (filter != null)
            couriers = couriers.Where(filter);
        return couriers;
    }


}
