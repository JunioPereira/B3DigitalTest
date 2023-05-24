using B3DigitalModel;
using Newtonsoft.Json;
using Websocket.Client;

namespace B3DigitalService
{
    public interface IWebSocketService 
    {
        Task<bool> CheckIsRunning();
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }

    public class WebSocketService: IWebSocketService
    {
        IQueueService iQueueService { get; }
        string data_btc { get; }
        string data_eth { get; }
        Uri Url { get; }
        WebsocketClient WClient { get; set; }

        public WebSocketService(IQueueService queueService) 
        {
            iQueueService = queueService;

            Url = new Uri("wss://ws.bitstamp.net/");
            data_btc = "{\r\n    \"event\": \"bts:subscribe\",\r\n    \"data\": {\r\n        \"channel\": \"order_book_btcusd\"\r\n    }\r\n}";
            data_eth = "{\r\n    \"event\": \"bts:subscribe\",\r\n    \"data\": {\r\n        \"channel\": \"order_book_ethusd\"\r\n    }\r\n}";
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            WClient = new WebsocketClient(Url);


            WClient.ReconnectTimeout = TimeSpan.FromSeconds(30);


            WClient.ReconnectionHappened.Subscribe(info =>
            {
                Console.WriteLine("Reconnection happened, type: " + info.Type);

                SendInfoToListen();
            });


            WClient.MessageReceived.Subscribe(msg =>
            {
                var result = JsonConvert.DeserializeObject<Payload<object>>(msg.Text);

                if ("data".Equals(result.@event))
                {
                    var _data = JsonConvert.DeserializeObject<Payload<Data>>(msg.Text);

                    if (result.channel == "order_book_btcusd") 
                    {
                        iQueueService.AddItem(new KeyValuePair<CriptoType, Data>(CriptoType.BTC, _data.data));
                    }

                    if (result.channel == "order_book_ethusd")
                    {
                        iQueueService.AddItem(new KeyValuePair<CriptoType, Data>(CriptoType.ETH, _data.data));
                    }
                }
            });


            WClient.DisconnectionHappened.Subscribe(aa => 
            { 
                
            });


            return WClient.Start();
            
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await WClient.Stop(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Connection close");
        }

        private void SendInfoToListen() 
        {
            Task.Run(() =>
            {
                Task.Delay(500).GetAwaiter().GetResult();
                WClient.Send(data_btc);
                WClient.Send(data_eth);
            });
        }

        public Task<bool> CheckIsRunning()
        {
            if (WClient == null)
            {
                return Task.FromResult(false);
            }
            else 
            {
                return Task.FromResult(WClient.IsRunning);
            }
        }
    }
}