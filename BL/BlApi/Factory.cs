namespace BlApi;
using BO;

public static class Factory
{
    public static IBl Get() => new BO.BlImplementation.Bl(); 

}
