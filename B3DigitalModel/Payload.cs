namespace B3DigitalModel
{
    public class Payload<T>
    {
        public T data { get; set; }
        public string channel { get; set; }
        public string @event { get; set; }
    }
}
