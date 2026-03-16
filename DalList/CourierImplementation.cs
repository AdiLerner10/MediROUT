namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// courier implemintation-create,delete,update etc
/// </summary>


internal class CourierImplementation : ICourier
{
    /// <summary>
    /// creates a new courier
    /// </summary>
    /// <param name="item">gets a Courier</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Courier item)
    {
        Courier? foundItem = Read(item.Id);
        if ((foundItem != null))
            throw new DalAlreadyExistException($"Courier with ID={item.Id} already exists");
        DataSource.Couriers.Add(item);
    }

    /// <summary>
    /// deletes a courier with the given id 
    /// </summary>
    /// <param name="id">gets an id of a courier</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Courier? foundItem = Read(id);
        if (foundItem is null)
            throw new DalDoesNotExistException($"Courier with ID={id} doesn't exist");
        DataSource.Couriers.Remove(foundItem);
    }

    /// <summary>
    /// deletes all the couriers in then list
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        var copy = DataSource.Couriers.ToList();
        foreach (var item in copy)
        {
            Delete(item.Id);
        }
    }

    /// <summary>
    /// reads a courier
    /// </summary>
    /// <param name="id">gtes an id of a courier</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(int id)
    {
        Courier courier = DataSource.Couriers.FirstOrDefault(item => item.Id == id);
        return courier;
    }

    /// <summary>
    /// a function that returns an object by a sent filter function
    /// </summary>
    /// <param name="filter">the sent boolian function</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Courier? Read(Func<Courier, bool> filter)
        =>filter !=null
    ? DataSource.Couriers.FirstOrDefault(item => filter(item))
    : throw new DalFunctionIsNull($"The sent filter is null");

    /// <summary>
    /// returns a list with all the couriers
    /// </summary>
    /// <returns>returns a list of couriers</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
        => filter != null
            ? from item in DataSource.Couriers
              where filter(item)
              select item
              : throw new DalFunctionIsNull($"The sent filter is null");


    /// <summary>
    /// updates an existing courier
    /// </summary>
    /// <param name="item">gets a courier</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Courier item)
    {
        Courier? foundCourier = Read(item.Id);
        if (foundCourier is null)
            throw new 
                ($"Courier with ID={item.Id} doesn't exists");
        Courier updatedCourier = item with { Id = item.Id };
        Delete(item.Id);
        Create(updatedCourier);
    }
}
