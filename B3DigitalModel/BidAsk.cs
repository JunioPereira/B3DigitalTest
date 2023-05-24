namespace B3DigitalModel
{
    public class BidAsk
    {
        public string Symbol { get; set; }
        public List<List<decimal>> Bids { get; set; }
        public List<List<decimal>> Asks { get; set; }
    }
}
