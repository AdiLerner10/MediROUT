using BlApi;
using Helpers;
using System.Net.Security;

namespace BO.BlImplementation;

internal class AdminImplementation : IAdmin
{
    /// <summary>
    /// Forwars clock by a certain time unit
    /// </summary>
    /// <param name="timeUnit">the time unit to forward teh clock by</param>
    public void ForwardClock(TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        switch (timeUnit)
        {
            case TimeUnit.Second:
                AdminManager.UpdateClock(AdminManager.Now.AddSeconds(1));
                break;
            case TimeUnit.Minute:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case TimeUnit.Hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case TimeUnit.Day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case TimeUnit.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case TimeUnit.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// gets the present time 
    /// </summary>
    /// <returns></returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public BO.Config GetConfig() => AdminManager.GetConfig();

    /// <summary>
    /// intizialized the data base
    /// </summary>
    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.InitializeDB();
    }

    /// <summary>
    /// resets the data base
    /// </summary>
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.ResetDB();
    }

    /// <summary>
    /// sets the configuration
    /// </summary>
    /// <param name="config"></param>
    public void SetConfig(BO.Config config)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.SetConfig(config);
    }

    /// <summary>
    /// a function that replace the courier password only if the sent password is the same as the existing password.
    /// </summary>
    /// <param name="managerPassword"></param>
    /// <param name="oldPassword"></param>
    /// <param name="courierId"></param>
    /// <param name="newPassword"></param>
    /// <exception cref="BlInvalidCredentials"></exception>
    public static void ReplaceCourierPassword(string managerPassword, string oldPassword, int courierId, string newPassword)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        if (AdminManager.GetConfig().ManagerPassword != Encrypt(managerPassword))
            throw new BlInvalidCredentials("Manager password incorrect.");
        BO.Courier courier = CourierManager.GetCourier(courierId);
        if (Encrypt(managerPassword) != Encrypt(courier.Password))
            throw new BlInvalidCredentials("your password incorrect.");
        courier.Password = newPassword;

        CourierManager.UpdateCourier(courier);
    }

    /// <summary>
    /// checks if this user is the manager
    /// </summary>
    /// <param name="id"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public bool IsManager(int id, string password)
    {
        if (id != AdminManager.GetConfig().ManagerId)
            return false;
        if (password != AdminManager.GetConfig().ManagerPassword)
            return false;
        return true;
    }

    /// <summary>
    /// encrypts the password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string Encrypt(string password)
    {
        return Tools.Encryption(password);
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;

    public void StartSimulator(int interval)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.Start(interval);
    }

    public void StopSimulator() => AdminManager.Stop();

    #endregion Stage 5
}
