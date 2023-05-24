namespace B3DigitalModel
{
    public class BestPricePayload
    {
        public Guid Id { get; set; }
        public List<List<decimal>> Collection { get; set; }
        public decimal Quantity { get; set; }
        public Side Side { get; set; }
        public decimal BestPrice { get; set; }
    }
}
