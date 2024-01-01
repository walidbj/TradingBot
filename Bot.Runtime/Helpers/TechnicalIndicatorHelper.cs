using IBApi;

namespace Bot.Runtime.Helpers
{
    public  static class TechnicalIndicatorHelper 
    {

        public static List<Bar> CalculateSuperTrend(List<Bar> data, int period = 4600, double multiplier = 3)
        {
            for (int i = period; i < data.Count; i++)
            {
                // Calculate ATR
                var tr = Math.Max(data[i].High - data[i].Low, Math.Max(Math.Abs(data[i].High - data[i - 1].Close), Math.Abs(data[i].Low - data[i - 1].Close)));
                var atr = data.Skip(i - period + 1).Take(period - 1).Sum(d => Math.Max(d.High - d.Low, Math.Max(Math.Abs(d.High - d.Close), Math.Abs(d.Low - d.Close)))) / period;

                // Calculate SuperTrend
                var upperBand = data[i].Close + multiplier * atr;
                var lowerBand = data[i].Close - multiplier * atr;

                data[i].InUpTrend = data[i - 1].Close > data[i - 1].SuperTrend;

                data[i].SuperTrend = data[i].InUpTrend ? upperBand : lowerBand;
                //if (data[i - 1].Close > data[i - 1].High)
                //{
                //    data[i].InUpTrend = false;
                //}
                //else if (data[i - 1].Close < data[i - 1].Low)
                //{
                //    data[i].InUpTrend = true;
                //}
                //else
                //{
                //    data[i].InUpTrend = data[i - 1].InUpTrend;
                //}

                //data[i].SuperTrend = data[i].InUpTrend ? upperBand : lowerBand;
            }

            return data.Skip(period).ToList();
        }
    
}
}
