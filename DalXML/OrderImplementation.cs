namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

///
internal class OrderImplementation : IOrder
{
    /// <summary>
    /// Creates an order and adds it the xml file
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        int newId = Config.NextOrderId;//running id number
        Order newOrder = item with { Id = newId };
        Orders.Add(newOrder);
        XMLTools.SaveListToXMLSerializer(Orders, Config.s_orders_xml);
    }

    /// <summary>
    /// Deletes an order with a specific Id from the xml file
    /// </summary>
    /// <param name="id">Id of the order to delete</param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if (Orders.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Order with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Orders, Config.s_orders_xml);
    }

    /// <summary>
    /// Deletes all the orders from the xml file
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Order>(), Config.s_orders_xml);
    }

    /// <summary>
    /// reads an order with a specific Id from the xml file
    /// </summary>
    /// <param name="id">Id of the order to read</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml );
        Order order = Orders.FirstOrDefault(item => item.Id == id);
        return order;
    }

    /// <summary>
    /// reads the first order with a specific filter from the xml file
    /// </summary>
    /// <param name="filter">function of filter</param>
    /// <returns></returns>
    /// <exception cref="DalFunctionIsNull"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        Order found;
        if (filter != null)
        {
            found = Orders.FirstOrDefault(item => filter(item));
        }
        else
            throw new DalFunctionIsNull($"The sent filter is null");
        return found;
    }

    /// <summary>
    /// reads all the orders with the filter
    /// </summary>
    /// <param name="filter">function to filter by</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        IEnumerable<Order> orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        //List<Order> orders = (List<Order>)ordersXML;
        if (filter != null)
            orders = orders.Where(filter);
        return orders;
    }

    /// <summary>
    /// updates an order in the xml file
    /// </summary>
    /// <param name="item">wich order to update</param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        List<Order> Orders = XMLTools.LoadListFromXMLSerializer<Order>(Config.s_orders_xml);
        if (Orders.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Order with ID={item.Id} does Not exist");
        Orders.Add(item);
        XMLTools.SaveListToXMLSerializer(Orders, Config.s_orders_xml);
    }
}
