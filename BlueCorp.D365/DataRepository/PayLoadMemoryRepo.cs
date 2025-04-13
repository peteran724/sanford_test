namespace BlueCorp.D365.DataRepository
{
    /// <summary>
    /// mock data in database side
    /// </summary>
    public class PayLoadMemoryRepo : IPayLoadRepo
    {
        public static IDictionary<int, string> PayLoads = new Dictionary<int, string>();

        public void Insert(int controlNum, string payLaod)
        {
            PayLoads.Add(controlNum, payLaod);
        }

        public int SelectMaxControlNum()
        {
            return PayLoads.Count != 0 ? PayLoads.Keys.Max() : 0;
        }

        public (int, string) SelectByPayLoad(string payload)
        {
            var x = PayLoads.SingleOrDefault(kv => kv.Value == payload);
            return (x.Key, x.Value);
        }
    }
}
