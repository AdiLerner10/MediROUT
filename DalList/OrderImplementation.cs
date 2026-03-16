namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Order implemintation-create, delete,read etc.
/// </summary>

internal class OrderImplementation : IOrder
{
    /// <summary>
    /// creates a new order and adds it to the list 
    /// </summary>
    /// <param name="item">gets an existing item</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Order item)
    {
        int newid = Config.NextOrderId;
        Order newOrder = item with { Id = newid };
        DataSource.Orders.Add(newOrder);
    }

    /// <summary>
    /// deletes the item with the same id
    /// </summary>
    /// <param name="id">gets an order id</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {

        Order? foundItem = Read(id);
        if (foundItem == null)
        {
            throw new DalDoesNotExistException($"Order with ID={id} doesn't exists");
        }
        DataSource.Orders.Remove(foundItem);
    }

    /// <summary>
    /// deletes all the items 
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        var copy = DataSource.Orders;
        foreach (var item in copy)
        {
            Delete(item.Id);
        }
    }

    /// <summary>
    /// checks if the id matches the order
    /// </summary>
    /// <param name="id">gets an order id</param>
    /// <returns>returns an item if found otherwise returns null</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(int id)
    {

        Order order = DataSource.Orders.FirstOrDefault(item => item.Id == id);
        return order;
    }

    /// <summary>
    /// a function that returns an object by a sent filter function
    /// </summary>
    /// <param name="filter">the sent boolian function</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Order? Read(Func<Order, bool> filter)
        => filter != null
    ? DataSource.Orders.FirstOrDefault(item => filter(item))
    : throw new DalFunctionIsNull($"The sent filter is null");

    /// <summary>
    /// returns a list with all of the orders
    /// </summary>
    /// <returns>returns a list with all the orders</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    => filter != null
            ? from item in DataSource.Orders
              where filter(item)
              select item
              : throw new DalFunctionIsNull($"The sent filter is null");


    /// <summary>
    /// updates an existing item 
    /// </summary>
    /// <param name="item">gets an order</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Order item)
    {
        Order? foundOrder = Read(item.Id);
        if ((foundOrder == null))
        {
            throw new DalAlreadyExistException($"Order with ID{item.Id} already exists.");
        }
        Order updatedOrder = item with { Id = item.Id };
        Delete(item.Id);
        DataSource.Orders.Add(updatedOrder);
    }
}
