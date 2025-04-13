using BlueCorp.Common;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BlueCorp.D365.Client;

public class DispatchClient : IDispatchClient
{

    private readonly IHttpClientFactory _httpClientFactory;
    public DispatchClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> UploadJsonFileTo3PL(PayLoad payLoad)
    {
        var json = JsonConvert.SerializeObject(payLoad);
       
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
       
        var formData = new MultipartFormDataContent();
        formData.Add(fileContent, "jsonFile", "payload.json");

        var client = _httpClientFactory.CreateClient("ThirdPL");
       
        return await client.PostAsync($"api/upload/", formData);
    }
}
