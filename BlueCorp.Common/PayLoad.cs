namespace BlueCorp.Common
{
    public class PayLoad
    {
        public int ControlNumber { get; set; }
        public string SalesOrder { get; set; }
        public List<Container> Containers { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }
    }
}
