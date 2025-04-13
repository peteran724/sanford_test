namespace BlueCorp.ThirdPL.Model
{
    public class CsvRecord
    {
        public string CustomerReference { get; set; }
        public string LoadId { get; set; }
        public string ContainerType { get; set; }
        public string ItemCode { get; set; }
        public int ItemQuantity { get; set; }
        public double ItemWeight { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
