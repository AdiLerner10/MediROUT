namespace BlApi;

/// <summary>
/// The courier functions definition in the BL
/// </summary>
public interface ICourier:IObservable
{
    public string CourierEntrance(int id, string password);
    public IEnumerable<BO.CourierInList> GetCourierList(int id, bool? active, BO.SortCourierByProperty? sortType );
    public BO.Courier GetCourier(int id, int courierId);
    public void UpdateCourier(int id, BO.Courier courier);  
    public void DeleteCourier(int id, int courierId);
    public void AddCourier(int id, BO.Courier newCourier);
    bool ValidateCourierId(int id);
    void ValidateCourierPhoneNumber(string phoneNumber);
    void ValidateCourierEmail(string email);
    bool ValidateCourierPassword(string password);
    bool CheckIfCanDelete(int CourierId);
    BO.OrderInProgress ConvertToOrderInProgress(BO.Order order, BO.Courier courierId);
    public void MaxDiststance(BO.Courier courier);
}
