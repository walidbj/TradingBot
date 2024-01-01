using Bot.Runtime.Events;
using Bot.Runtime.Helpers;
using Bot.Runtime.Wrappers;
using IBApi;

namespace Bot.Runtime
{
    public class Program
    {
        public static List<IBApi.Bar> HistoricalBars { get; set; } = new List<IBApi.Bar>();
        private static async Task Main(string[] args)
        {
            try
            {


                // Input parameters
                string stockCode = "RILY"; // Replace with your desired stock code
                string duration = "1 D"; // Replace with your desired duration

                // IBKR connection details
                string host = "127.0.0.1"; // TWS or IB Gateway host
                int port = 7497; // TWS or IB Gateway port
                int clientId = 1; // Unique client ID for your application

                // Instantiate and connect to IBKR
                var ibkrWrapper = new MarketHistoryWrapper();
                var readerSignal = new EReaderMonitorSignal();
                var ibkrClient = new EClientSocket(ibkrWrapper, readerSignal);
                ibkrClient.eConnect(host, port, clientId);

                if (ibkrClient.IsConnected())
                {
                    Console.WriteLine("Connected to IBKR");

                    // Create a reader to process messages from TWS or IB Gateway
                    var reader = new EReader(ibkrClient, readerSignal);
                    reader.Start();

                    // Start processing messages from the reader
                    new Task(() =>
                    {
                        while (ibkrClient.IsConnected())
                        {
                            readerSignal.waitForSignal();
                            reader.processMsgs();
                        }
                    }).Start();

                    // Request market data for the specified stock
                    int requestId = 1;
                    IBApi.Contract contract = new IBApi.Contract
                    {
                        Symbol = stockCode,
                        SecType = "STK",
                        Exchange = "SMART",
                        Currency = "USD"
                    };
                    // Specify the end date and time for the historical data request
                    string endDateTime = DateTime.UtcNow.ToString("yyyyMMdd-hh:mm:ss");
                    string genericTickList = "233"; // Super Trend indicator

        

                    DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0);

                    ibkrWrapper.HistoricalDataReceived += HistoricalDataHandler;
                    ibkrWrapper.HistoryReceived += HistoryReceivedDatahandler;
                    ibkrClient.reqHistoricalData(requestId, contract, DateTime.Now.ToString("yyyyMMdd-hh:mm:ss"), duration, "5 secs", "TRADES", 1, 1, false, null);



                    // Wait for market data to arrive (you may need to adjust the timing based on your needs)
                    await Task.Delay(5000);

                    // Implement your Super Trend strategy and place orders here

                    // Disconnect from IBKR
                    ibkrClient.eDisconnect();
                    Console.WriteLine("Disconnected from IBKR");
                }
                else
                {
                    Console.WriteLine("Failed to connect to IBKR");
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        private  static void HistoryReceivedDatahandler(object? sender, bool e)
        {
            HistoricalBars =  TechnicalIndicatorHelper.CalculateSuperTrend(HistoricalBars);
        }

        private  static void HistoricalDataHandler(object sender, HistoricalDataEventArgs e)
        {
            // Access and process the historical data in the Main function
            var historicalData = e.Bar;
            HistoricalBars.Add(historicalData);
            Console.WriteLine($"Time: {historicalData.Time}, Open: {historicalData.Open}, High: {historicalData.High}, Low: {historicalData.Low}, Close: {historicalData.Close}");
        }

    }
}