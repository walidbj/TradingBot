using IBApi;

namespace Bot.Runtime.Events
{
    public class HistoricalDataEventArgs : EventArgs
    {
        public int RequestId { get;  }
        public Bar Bar { get; }

        public HistoricalDataEventArgs(int requestId, Bar bar)
        {
            RequestId = requestId;
            Bar = bar;
        }
    }
}
