namespace BlueCorp.Common
{
    /// <summary>
    /// should use azure key vault
    /// </summary>
    public class KeyVaults
    {
        public static IDictionary<string, string> APIKeys = new Dictionary<string, string>();

        static KeyVaults()
        {
            APIKeys.Add("D365-3PL", "2G2X9qbsqBBwl4xuxbE7bg==");
        }
    }
}
