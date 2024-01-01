using IBApi;

namespace Bot.Runtime.Interfaces
{
    public interface ITechnicalIndicatorService
    {
        List<Bar> CalculateSuperTrend(List<Bar> data, int period = 7, double multiplier = 3);
    }
}
