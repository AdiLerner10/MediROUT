using BO;
using DalApi;
using DalTest;
using System.Runtime.CompilerServices;

namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Configuration Variables and Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4-7
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    internal static
        Action? ConfigUpdatedObservers; //stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //stage 5 - for clock update observers

    private static Task? _periodicTask = null; //stage 7

    /// <summary>
    /// Method to update application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        DateTime oldClock;
        lock (AdminManager.BlMutex)
        {
            oldClock = s_dal.Config.Clock; //stage 4
            s_dal.Config.Clock = newClock; //stage 4
        }

        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic students' updates:
        // - Go through all students to update properties that are affected by the clock update
        // - (students become not active after 5 years etc.)

        //TO_DO: //stage 4
        //CourierManager.PeriodicCouriersUpdate(oldClock, newClock); //stage 4. to be removed in stage 7 and replaced as below
        //OrderManager.PeriodicOrdersUpdates(oldClock, newClock);
        //DeliveryManager.PeriodicDeliveriesUpdates(oldClock, newClock);

        //lock (AdminManager.BlMutex)
        //{
        //    _ = Task.Run(() => CourierManager.PeriodicCouriersUpdate(oldClock, newClock));
        //}
        //TO_DO: //stage 7
        if (_periodicTask is null || _periodicTask.IsCompleted) //stage 7
            _periodicTask = Task.Run(() => CourierManager.PeriodicCouriersUpdate(oldClock, newClock));
        //..

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }


    /// <summary>
    /// Method for providing current configuration variables values for any BL class that may need it
    /// </summary>
    //[MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static BO.Config GetConfig() //stage 4
    {
        IConfig cnfg;
        lock (AdminManager.BlMutex)
            cnfg = s_dal.Config;

        return new BO.Config()
        {
            //TO_DO: //stage 4
            //add an assignment for each configuration property
            Clock = cnfg.Clock,
            ManagerId = cnfg.ManagerId,
            ManagerPassword = cnfg.ManagerPassword,
            ManagerSalary = cnfg.ManagerSalary,
            CourierSalary = cnfg.CourierSalary,
            CompanyAddress = cnfg.CompanyAddress,
            MaximumDistanceKM = cnfg.MaximumDistanceKM,
            MaxTimeWindow = cnfg.MaxTimeWindow,
            RiskRange = cnfg.RiskRange,
            CourierNotActive = cnfg.CourierNotActive,
            BicycleAvgSpeedKMH = (double)cnfg.BicycleAvgSpeedKMH,
            CarAvgSpeedKMH = cnfg.CarAvgSpeedKMH,
            MotorcycleAvgSpeedKMH = (double)cnfg.MotorcycleAvgSpeedKMH,
            OnFootAvgSpeedKMH = (double)cnfg.OnFootAvgSpeedKMH
        };

    }

    /// <summary>
    /// Method for setting current configuration variables values for any BL class that may need it
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void SetConfig(BO.Config configuration) //stage 4
    {
        bool configChanged = false;

        IConfig cnfg;
        lock (AdminManager.BlMutex)
        {
            cnfg = s_dal.Config;

            if (cnfg.RiskRange != configuration.RiskRange) //stage 4
            {
                s_dal.Config.RiskRange = configuration.RiskRange;
                configChanged = true;
            }

            if (cnfg.MaximumDistanceKM != configuration.MaximumDistanceKM) //stage 4
            {
                s_dal.Config.MaximumDistanceKM = configuration.MaximumDistanceKM;
                configChanged = true;
            }

            if (cnfg.MaxTimeWindow != configuration.MaxTimeWindow) //stage 4
            {
                s_dal.Config.MaxTimeWindow = configuration.MaxTimeWindow;
                configChanged = true;
            }

            if (configuration.CompanyAddress is not null && (cnfg.CompanyAddress != configuration.CompanyAddress)) //stage 4
            {
                s_dal.Config.CompanyAddress = configuration.CompanyAddress;
                configChanged = true;
            }

            if (cnfg.CourierNotActive != configuration.CourierNotActive) //stage 4
            {
                s_dal.Config.CourierNotActive = configuration.CourierNotActive;
                configChanged = true;
            }

            string? encryptedPassword = Tools.Encryption(configuration.ManagerPassword);
            if (cnfg.ManagerPassword != configuration.ManagerPassword) //stage 4
            {
                Tools.IsStrongPassword(configuration.ManagerPassword);
                try
                {
                    s_dal.Config.ManagerPassword = encryptedPassword;
                    configChanged = true;
                }
                catch { }
            }

            if (cnfg.ManagerSalary != configuration.ManagerSalary) //stage 4
            {
                s_dal.Config.ManagerSalary = configuration.ManagerSalary;
                configChanged = true;
            }

            if (cnfg.CourierSalary != configuration.CourierSalary) //stage 4
            {
                s_dal.Config.CourierSalary = configuration.CourierSalary;
                configChanged = true;
            }
        }

        //Calling all the observers of configuration update
        if (configChanged)// stage 5
            ConfigUpdatedObservers?.Invoke(); // stage 5

    }

    /// <summary>
    /// a finction that reset the DB values.
    /// </summary>
    internal static void ResetDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            s_dal.ResetDB(); //stage 4
            s_dal.Config.ManagerPassword = Tools.Encryption(s_dal.Config.ManagerPassword);
            AdminManager.UpdateClock(AdminManager.Now); //stage 5 - needed since we want the label on Pl to be updated
        }
        ConfigUpdatedObservers?.Invoke();
        OrderManager.Observers.NotifyListUpdated();
        CourierManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// a function that initialize the DB values with beggining values.
    /// </summary>
    internal static void InitializeDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            DalTest.Initialization.Do(); //stage 4
            s_dal.Config.ManagerPassword = Tools.Encryption(s_dal.Config.ManagerPassword);
            AdminManager.UpdateClock(AdminManager.Now);  //stage 5 - needed since we want the label on Pl to be updated           
        }
        ConfigUpdatedObservers?.Invoke();//stage 5 - needed for update the PL
        OrderManager.Observers.NotifyListUpdated();
        CourierManager.Observers.NotifyListUpdated();
    }

    #endregion Stage 4-7

    #region Stage 7 base

    /// <summary>    
    /// Mutex to use from BL methods to get mutual exclusion while the simulator is running
    /// </summary>
    internal static readonly object BlMutex = new(); // BlMutex = s_dal; // This field is actually the same as s_dal - it is defined for readability of locks

    /// <summary>    
    /// Mutex to use
    /// </summary>
    internal static readonly object SimulatorDistanceMutex = new();

    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static volatile Thread? s_thread;
    internal static bool IsSimulatorRunning { get => s_thread is not null; }
    /// <summary>
    /// The Interval for clock updating
    /// in minutes by second (default value is 1, will be set on Start())    
    /// </summary>
    private static int s_interval = 1;
    /// <summary>
    /// The flag that signs whether simulator is running
    /// 
    private static volatile bool s_stop = false;

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
            throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Stop()
    {

        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt(); //awake a sleeping thread
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }

    }

    private static Task? _simulateTask = null;
    private static volatile bool _isSimulating = false;

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));
            //TO_DO: //stage 7
            //Add calls here to any logic simulation that was required in stage 7
            //for example: course registration simulation
            //etc…
            _ = Task.Run(async () => await CourierManager.SimulateCourierCollectAndDeliverAsync());

            try
            {
                Thread.Sleep(1000); // 1 second
            }

            catch (ThreadInterruptedException) { }
        }



        //while (!s_stop)
        //{
        //    UpdateClock(Now.AddMinutes(s_interval));

        //    // Spawn simulation task only if the previous one finished
        //    if (!_isSimulating)
        //    {
        //        _isSimulating = true;
        //        _ = Task.Run(async () =>
        //        {
        //            try
        //            {
        //                await CourierManager.SimulateCourierCollectAndDeliverAsync();
        //            }
        //            finally
        //            {
        //                _isSimulating = false;
        //            }
        //        });
        //    }

        //    try
        //    {
        //        Thread.Sleep(000); // 1 second
        //    }
        //    catch (ThreadInterruptedException) { }
        //}
    }

    #endregion Stage 7 base


}
