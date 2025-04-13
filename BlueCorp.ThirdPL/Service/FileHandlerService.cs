using BlueCorp.Common;
using BlueCorp.ThirdPL.Contract;
using BlueCorp.ThirdPL.Model;
using CsvHelper;
using System.Globalization;

namespace BlueCorp.ThirdPL.Service
{
    public class FileHandlerService : IFileHandlerService
    {
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _host;
        private readonly ILogger<FileHandlerService> _logger;
        public FileHandlerService(IConfiguration config, IHostEnvironment host, ILogger<FileHandlerService> logger)
        {
            _config = config;
            _host = host;
            _logger = logger;
        }

        public bool CheckAuthorized(string apiKey, string ip)
        {
            string[] ips = _config["ip_white_listing"].Split(",");
            return apiKey == KeyVaults.APIKeys["D365-3PL"] && ips.Contains(ip);
        }

        public async Task<bool> UploadToInComingFolder(PayLoad payLoad)
        {
            int controlNum = payLoad.ControlNumber;
            var records = new List<CsvRecord>();
            payLoad.Containers.ForEach(c =>
            {
                c.Items.ForEach(i =>
                {
                    records.Add(new CsvRecord
                    {
                        CustomerReference = payLoad.SalesOrder,
                        LoadId = c.LoadId,
                        ContainerType = c.ContainerType.GetDescription(),
                        ItemCode = i.ItemCode,
                        ItemQuantity = i.Quantity,
                        ItemWeight = i.CartonWeight,
                        Street = payLoad.DeliveryAddress.Street,
                        City = payLoad.DeliveryAddress.City,
                        State = payLoad.DeliveryAddress.State,
                        PostalCode = payLoad.DeliveryAddress.PostalCode,
                        Country = payLoad.DeliveryAddress.Country
                    });
                });
            });
            try
            {
                var path = Path.Combine(_host.ContentRootPath, "bluecorp-incoming");
                var fileName = $"bluecorp-3pl_controlno_{controlNum}_{DateTime.Now:yyyyMMddHHmmssfff}.csv";
                var fullPath = Path.Combine(path, fileName);
                using (var writer = new StreamWriter(fullPath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(records);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Upload csv file to bluecorp-incoming folder failed, Control Number{controlNum}", ex);
                return false;
            }
            return true;

        }

        public void MoveFiles()
        {
            string incomingPath = Path.Combine(_host.ContentRootPath, "bluecorp-incoming");
            string processedPath = Path.Combine(_host.ContentRootPath, "bluecorp-processed");
            string failedPath = Path.Combine(_host.ContentRootPath, "bluecorp-failed");


            string[] csvFiles = Directory.GetFiles(incomingPath, "*.csv");

            foreach (var filePath in csvFiles)
            {
                string fileName = Path.GetFileName(filePath);
                string processedFile = Path.Combine(processedPath, fileName);

                try
                {
                    File.Move(filePath, processedFile);
                    _logger.LogInformation($"{fileName} has been moved to bluecorp-processed folder successfully", fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{fileName} has been moved to bluecorp-processed folder failed, try to move to bluecorp-failed folder", fileName);
                    string failedFile = Path.Combine(failedPath, fileName);

                    try
                    {
                        File.Move(filePath, failedFile);
                        _logger.LogInformation($"{fileName} has been moved to bluecorp-failed folder successfully", fileName);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"{fileName} has been moved to bluecorp-failed folder failed,{fileName} is still keep in the bluecorp-incoming folder", fileName);
                    }
                }
            }

        }
    }
}
