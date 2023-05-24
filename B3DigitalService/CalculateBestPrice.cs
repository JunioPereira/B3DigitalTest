using B3DigitalModel;
using System.Collections.Concurrent;

namespace B3DigitalService
{
    public interface ICalculateBestPrice 
    {
        void AddValues(CriptoType type, BidAsk ba);
        BestPricePayload CalculatePrice(CriptoType type, Side side, int quantity);
    }

    public class CalculateBestPrice : ICalculateBestPrice
    {
        ConcurrentDictionary<CriptoType, BidAsk> dicBidAsk { get; set; }
        public CalculateBestPrice() 
        {
            dicBidAsk = new ConcurrentDictionary<CriptoType, BidAsk>();
        }

        public void AddValues(CriptoType type, BidAsk ba)
        {
            dicBidAsk[type] = ba;
        }

        public BestPricePayload CalculatePrice(CriptoType type, Side side, int quantity)
        {
            var id = Guid.NewGuid();

            decimal price = 0;
            decimal qtd = quantity;

            var payload = new BestPricePayload();
            payload.Id = id;
            payload.Side = side;
            payload.Quantity = qtd;
            payload.Collection = new List<List<decimal>>();

            if (dicBidAsk.ContainsKey(type)) 
            {
                if (side == Side.Buy) 
                {
                    var bidAsk = dicBidAsk[type];

                    //Ordenando pelo preço
                    var ask = bidAsk.Asks.OrderBy(x => x[0]);

                    foreach (var item in ask)
                    {
                        //Posição 0 - Price
                        //Posição 1 - Quantity

                        if (item[1] >= qtd)
                        {
                            price += qtd * item[0];

                            payload.Collection.Add(item);

                            break;
                        }
                        else 
                        {
                            price += item[1] * item[0];

                            qtd = qtd - item[1];

                            payload.Collection.Add(item);
                        }
                    }

                    payload.BestPrice = price;
                }

                if (side == Side.Sell)
                {
                    var bidAsk = dicBidAsk[type];

                    //Ordenando pelo preço
                    var bid = bidAsk.Asks.OrderByDescending(x => x[0]);

                    foreach (var item in bid)
                    {
                        //Posição 0 - Price
                        //Posição 1 - Quantity

                        if (item[1] >= qtd)
                        {
                            price += qtd * item[0];

                            payload.Collection.Add(item);

                            break;
                        }
                        else
                        {
                            price += item[1] * item[0];

                            qtd = qtd - item[1];

                            payload.Collection.Add(item);
                        }
                    }

                    payload.BestPrice = price;
                }
            }

            return payload;
        }
    }
}
