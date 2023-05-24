namespace B3DigitalModel
{
    public class QuoteInfo
    {
        public string Symbol { get; set; }
        public decimal BiggestPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AveragePrice5Seconds { get; set; }
        public decimal averageQtd { get; set; }
    }
}
