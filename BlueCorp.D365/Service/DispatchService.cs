using BlueCorp.Common;
using BlueCorp.D365.Client;
using BlueCorp.D365.Contract;
using BlueCorp.D365.DataRepository;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;
using System.Text;

namespace BlueCorp.D365.Service
{
    public class DispatchService : IDispatchService
    {
        private readonly IPayLoadRepo _repo;
        private readonly IDispatchClient _client;
        private readonly ILogger<DispatchService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        public DispatchService(IPayLoadRepo repo, IDispatchClient client, ILogger<DispatchService> logger)
        {
            _repo = repo;
            _client = client;
            _logger = logger;
            //handle http error(e.g. 500)
            //request time out(e.g. 504)
            //retry int.MaxValue times, 2^n seconds waiting time for each retry
            _retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.GatewayTimeout).WaitAndRetryAsync(int.MaxValue, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)));

        }

        private bool IsProcessed(string payLoadJson)
        {
            var (controlNum, payLaod) = _repo.SelectByPayLoad(payLoadJson);

            return controlNum != 0 && payLaod != null;
        }

        public async Task<(bool, string)> ProcessAsync(PayLoad payLoad)
        {
            string json = JsonConvert.SerializeObject(payLoad);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            if (stream.Length > 800 * 1000)
                return (false, "The PayLaod size can't be more than 800KB");

            bool isProcessed = IsProcessed(json);
            if (isProcessed)
                return (false, "The PayLaod is already dispatched");

            int controlNum = _repo.SelectMaxControlNum() + 1;
            try
            {
                _repo.Insert(controlNum, json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dispatch PayLoad into Database failed, SalesOrder:{payLoad.SalesOrder}", ex);
                return (false, "OOPS! Something wrong happend! Try again later!");
            }

            payLoad.ControlNumber= controlNum;

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _client.UploadJsonFileTo3PL(payLoad);
            });
            if (response.IsSuccessStatusCode)
                _logger.LogInformation($"Dispatch PayLoad from D365 to 3PL successfully, Control Number{controlNum},SalesOrder:{payLoad.SalesOrder}");
            else
                _logger.LogError($"Dispatch PayLoad from D365 to 3PL failed, Control Number{controlNum},SalesOrder:{payLoad.SalesOrder},StatusCode:{ response.StatusCode}. Result: {response.Content.ReadAsStringAsync().Result}");

            return (true, "");
        }
    }
}
