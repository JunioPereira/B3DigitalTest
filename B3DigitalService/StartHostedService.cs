using B3DigitalModel;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace B3DigitalService
{
    public class StartHostedService : IHostedService
    {
        IQueueService iQueueService { get; }
        IWebSocketService iWebSocketService { get; }
        IObservableQuotesInfo iObservableQuotesInfo { get; }
        ICalculateBestPrice iCalculateBestPrice { get; }
        ConcurrentDictionary<string, QuoteInfo> DicQuoteInfo { get; }
        ConcurrentDictionary<string, BidAsk> DicBidAsk { get; set; }

        public StartHostedService(IQueueService queueService, IWebSocketService webSocketService, IObservableQuotesInfo observableQuotesInfo, ICalculateBestPrice calculateBestPrice) 
        {
            iQueueService = queueService;
            iWebSocketService = webSocketService;
            iObservableQuotesInfo = observableQuotesInfo;
            iCalculateBestPrice = calculateBestPrice;

            DicQuoteInfo = new ConcurrentDictionary<string, QuoteInfo>();
            DicBidAsk = new ConcurrentDictionary<string, BidAsk>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                while (!iQueueService.IsCompleted())
                {
                    try
                    {
                        var item = iQueueService.Take();


                        iCalculateBestPrice.AddValues(item.Key, new BidAsk() { Asks = item.Value.asks, Bids = item.Value.bids, Symbol = item.Key.ToString() });

                        //Posição 0 - Price
                        //Posição 1 - Quantity

                        //venda
                        var biggestPrice = item.Value.asks.OrderByDescending(x => x[0]).First()[0];
                        
                        var lengthPriceAsk = item.Value.asks.Select(x => x[0]).Count();
                        
                        var averagePriceAsk = (item.Value.asks.Select(x => x[0])).Sum() / lengthPriceAsk;

                        //Compra
                        var lowestPrice = item.Value.bids.OrderBy(x => x[0]).First()[0];

                        var lengthPriceBid = item.Value.bids.Select(x => x[0]).Count();

                        var averagePriceBid = (item.Value.bids.Select(x => x[0])).Sum() / lengthPriceBid;

                        //Average Price
                        var averagePrice = (averagePriceAsk + averagePriceBid) / 2;



                        var lengthQtdAsk = item.Value.asks.Select(x => x[1]).Count();
                        var averageQtdAsk = (item.Value.asks.Select(x => x[1])).Sum() / lengthQtdAsk;

                        var lengthQtdBid = item.Value.bids.Select(x => x[1]).Count();
                        var averageQtdBid = (item.Value.bids.Select(x => x[1])).Sum() / lengthQtdBid;

                        //Average quantity
                        var averageQtd = (averageQtdAsk + averageQtdBid) / 2;

                        var quoteInfo = new QuoteInfo();
                        quoteInfo.Symbol = item.Key.ToString();
                        quoteInfo.BiggestPrice = biggestPrice;
                        quoteInfo.LowestPrice = lowestPrice;
                        quoteInfo.AveragePrice = averagePrice;
                        quoteInfo.averageQtd = averageQtd;

                        DicQuoteInfo[quoteInfo.Symbol] = quoteInfo;

                        if (DicBidAsk.ContainsKey(quoteInfo.Symbol))
                        {
                            DicBidAsk[quoteInfo.Symbol].Bids.AddRange(item.Value.bids);
                            DicBidAsk[quoteInfo.Symbol].Asks.AddRange(item.Value.asks);
                        }
                        else
                        {
                            var bidAsk = new BidAsk();
                            bidAsk.Symbol = quoteInfo.Symbol;
                            bidAsk.Bids = item.Value.bids;
                            bidAsk.Asks = item.Value.asks;

                            DicBidAsk[quoteInfo.Symbol] = bidAsk;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });

            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var dic = new ConcurrentDictionary<string, BidAsk>(DicBidAsk);
                        
                        DicBidAsk = new ConcurrentDictionary<string, BidAsk>();

                        foreach ( var item in dic.Values ) 
                        {
                            //venda
                            var biggestPrice = item.Asks.OrderByDescending(x => x[0]).First()[0];

                            var lengthPriceAsk = item.Asks.Select(x => x[0]).Count();

                            var averagePriceAsk = (item.Asks.Select(x => x[0])).Sum() / lengthPriceAsk;

                            //Compra
                            var lowestPrice = item.Bids.OrderBy(x => x[0]).First()[0];

                            var lengthPriceBid = item.Bids.Select(x => x[0]).Count();

                            var averagePriceBid = (item.Bids.Select(x => x[0])).Sum() / lengthPriceBid;

                            //Average Price
                            var averagePrice = (averagePriceAsk + averagePriceBid) / 2;

                            if (DicQuoteInfo.ContainsKey(item.Symbol)) 
                            {
                                DicQuoteInfo[item.Symbol].AveragePrice5Seconds = averagePrice;

                                iObservableQuotesInfo.OnNext(DicQuoteInfo[item.Symbol]);
                            }
                        }

                        Task.Delay(5000).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            iQueueService.Stop();
            return iWebSocketService.StopAsync(cancellationToken);
        }
    }
}
