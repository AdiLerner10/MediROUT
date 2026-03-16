namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// a class that implement the IDelivery interface
/// </summary>
internal class DeliveryImplementation : IDelivery
{
    /// <summary>
    /// a function that create new Delivery object in the data base, if the id doesn't exist in the data base it will throw an exception.
    /// </summary>
    /// <param name="item">get a new Delivery object to add in the data base</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Delivery item)
    {
        int newId = Config.NextDeliveryId;//running id number
        Delivery newDelivery = item with { Id = newId };
        DataSource.Deliveries.Add(newDelivery);
    }

    /// <summary>
    ///a function the deleted the object with the given id, if the id doesn't exist in the data base it will throw an exception.
    /// </summary>
    /// <param name="id">get the Delivery id number you want to delete.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Delivery? foundItem = Read(id);
        if (foundItem is null)
            throw new DalDoesNotExistException($"Delivery with ID={id} doesn't exists");
        DataSource.Deliveries.Remove(foundItem);
    }

    /// <summary>
    /// a function that deletes the whole Delivery objects 
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        var copy = DataSource.Deliveries.ToList();
        foreach (var item in copy)
        {
            Delete(item.Id);
        }
    }

    /// <summary>
    /// a function that get an id number and returns the object with the match id, if it isn't exist returns null.
    /// </summary>
    /// <param name="id">gets an id of the object we want to search for.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(int id)
    {

        Delivery delivery = DataSource.Deliveries.FirstOrDefault(item => item.Id == id);
        return delivery;
    }

    /// <summary>
    /// a function that returns an object by a sent filter function
    /// </summary>
    /// <param name="filter">the sent boolian function</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Delivery? Read(Func<Delivery, bool> filter)
        => filter != null
    ? DataSource.Deliveries.FirstOrDefault(item => filter(item))
    : throw new DalFunctionIsNull($"The sent filter is null");

    /// <summary>
    /// a function that makes a copy of the delivery list and returns it's reference.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
              => filter != null
            ? from item in DataSource.Deliveries
              where filter(item)
              select item
              : throw new DalFunctionIsNull($"The sent filter is null");


    /// <summary>
    /// a function that get an updated item, and update it in the data base. if it's not exist it will throw an exception.
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Delivery item)
    {
        Delivery? foundDelivery = Read(item.Id);
        if (foundDelivery is null)
            throw new DalDoesNotExistException($"Delivery with ID={item.Id} doesn't exists");
        Delete(item.Id);
        Delivery updatedDelivery = item with {Id=item.Id };

        DataSource.Deliveries.Add(updatedDelivery);
    }
}