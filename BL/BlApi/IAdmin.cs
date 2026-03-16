using BO;
namespace BlApi;

/// <summary>
/// The Config functions definition in the BL 
/// </summary>
public interface IAdmin
{
    public void ResetDB();
    public void InitializeDB();
    public DateTime GetClock();
    public void ForwardClock(TimeUnit timeUnit);
    public BO.Config GetConfig();
    public void SetConfig(BO.Config config);
    public bool IsManager(int id, string password);
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

    #region Stage 7
    void StartSimulator(int interval);
    void StopSimulator();
    #endregion

}
