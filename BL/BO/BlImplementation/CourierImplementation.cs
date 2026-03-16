using BlApi;
using Helpers;
using DalApi;
namespace BO.BlImplementation;

internal class CourierImplementation : BlApi.ICourier
{
    /// <summary>
    /// adds a courier to the database
    /// </summary>
    /// <param name="id">manager id</param>
    /// <param name="newCourier">the new courier to add</param>
    public void AddCourier(int id, Courier newCourier)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        MaxDiststance(newCourier);
        CourierManager.AddCourier(id, newCourier);

    }

    /// <summary>
    /// adds an observer
    /// </summary>
    /// <param name="listObserver"></param>
    public void AddObserver(Action listObserver) => CourierManager.Observers.AddListObserver(listObserver);

    /// <summary>
    /// ///adds an observer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="observer"></param>
    public void AddObserver(int id, Action observer) => CourierManager.Observers.AddObserver(id, observer);

    /// <summary>
    /// checks if entrance details are correct
    /// </summary>
    /// <param name="name">username</param>
    /// <param name="password">users password</param>
    /// <exception cref="BlMemberIsNull"></exception>
    public string CourierEntrance(int id, string password)
    {
        if (id == null)
            throw new BlMemberIsNull("name");
        return CourierManager.Entrance(id, password);
    }

    /// <summary>
    /// deletes a courier
    /// </summary>
    /// <param name="id">manager id </param>
    /// <param name="courierId">id of the courier to delete</param>
    public void DeleteCourier(int id, int courierId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CourierManager.DeleteCourier(id, courierId);
    }

    /// <summary>
    /// gets a courier by an id
    /// </summary>
    /// <param name="id">manager id </param>
    /// <param name="courierId">courier id</param>
    /// <returns></returns>
    public BO.Courier GetCourier(int id, int courierId)
    {
        return CourierManager.GetCourier(courierId);
    }

    /// <summary>
    /// gets a courier list sorted by a specific filter
    /// </summary>
    /// <param name="id">manager id</param>
    /// <param name="active">active courier?</param>
    /// <param name="sortType">the type of filter to sort by</param>
    /// <returns></returns>
    public IEnumerable<CourierInList> GetCourierList(int id, bool? active, BO.SortCourierByProperty? sortType)
    {
        return CourierManager.GetCourierList(active, sortType);
    }

    public void RemoveObserver(Action listObserver) => CourierManager.Observers.RemoveListObserver(listObserver);

    public void RemoveObserver(int id, Action observer) => CourierManager.Observers.RemoveObserver(id, observer);



    /// <summary>
    /// updates a courier 
    /// </summary>
    /// <param name="id">manager id</param>
    /// <param name="courier">updated courier</param>
    public void UpdateCourier(int id, Courier courier)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        MaxDiststance(courier);
        CourierManager.UpdateCourier(courier);
    }

    /// <summary>
    /// checks if id is valid
    /// </summary>
    /// <param name="id"></param>
    public bool ValidateCourierId(int id)
    {
        return Tools.IsValidId(id);
    }
    public void ValidateCourierPhoneNumber(string phoneNumber)
    {
        Tools.IsValidPhoneNumber(phoneNumber);
    }

    /// <summary>
    /// checks if courier email is valid 
    /// </summary>
    /// <param name="email"></param>
    public void ValidateCourierEmail(string email)
    {
        Tools.IsValidEmail(email);
    }

    /// <summary>
    /// checks if the couriers password 
    /// </summary>
    /// <param name="password"></param>

    public bool ValidateCourierPassword(string password)
    {
        return Tools.IsStrongPassword(password);
    }

    /// <summary>
    /// checks if courier can be deleated
    /// </summary>
    /// <param name="CourierId"></param>
    /// <returns></returns>
    public bool CheckIfCanDelete(int CourierId)
    {
        return Tools.CanBeDeleted(CourierId);
    }

    /// <summary>
    /// converts a order in to an order in progress
    /// </summary>
    /// <param name="order"></param>
    /// <param name="courier"></param>
    /// <returns></returns>
    public BO.OrderInProgress ConvertToOrderInProgress(BO.Order order, BO.Courier courier)
    {
        return CourierManager.ConverToOrderInProgress(order, courier);
    }

    /// <summary>
    /// makes sure the couriers max distaance is smaller oir equal to the manager
    /// </summary>
    /// <param name="courier"></param>
    public void MaxDiststance(BO.Courier courier)
     {
        if (courier.MaxPersonalDistance > AdminManager.GetConfig().MaximumDistanceKM)
            courier.MaxPersonalDistance = AdminManager.GetConfig().MaximumDistanceKM;
    }

}
