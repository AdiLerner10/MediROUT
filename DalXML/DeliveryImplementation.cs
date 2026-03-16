namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class DeliveryImplementation : IDelivery
{
    /// <summary>
    /// Creates a delivery and adds it the xml file
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        List<Delivery> Deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        int newId = Config.NextDeliveryId;//running id number
        Delivery newDelivery = item with { Id = newId };
        Deliveries.Add(newDelivery);

        XMLTools.SaveListToXMLSerializer(Deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Deletes a delivery with a specific Id from the xml file
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Delivery> Deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if (Deliveries.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Delivery with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Deletes all the deliveries from the xml file
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Delivery>(), Config.s_deliveries_xml);
    }

    /// <summary>
    /// reads a delivery with a specific Id from the xml file
    /// </summary>
    /// <param name="id">Id of delivery to read</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {
        List<Delivery> Deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery delivery = Deliveries.FirstOrDefault(item => item.Id == id);
        return delivery;
    }

    /// <summary>
    /// reads the first delivery with a specific filter from the xml file
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    /// <exception cref="
    /// "></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery found;
        if (filter != null)
        {
            found = deliveries.FirstOrDefault(item => filter(item));
        }
        else
            throw new DalFunctionIsNull($"The sent filter is null");
        return found;
    }

    /// <summary>
    ///  reads all the deliveries with the filter
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        IEnumerable<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if (filter != null)
            deliveries = deliveries.Where(filter);
        return deliveries;
    }

    /// <summary>
    ///  updates a delivery in the xml file
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        if (deliveries.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        deliveries.Add(item);
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_deliveries_xml);
    }
}
