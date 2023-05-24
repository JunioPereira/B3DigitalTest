namespace B3DigitalModel
{
    public class Data
    {
        public string timestamp { get; set; }
        public string microtimestamp { get; set; }
        public List<List<decimal>> bids { get; set; }
        public List<List<decimal>> asks { get; set; }
    }
}