using BlueCorp.Common;
using BlueCorp.ThirdPL.Contract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlueCorp.ThirdPL.Controller
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileHandlerService _srv;

        public UploadController(IFileHandlerService srv)
        {
            _srv = srv;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
                return BadRequest("No file uploaded");

            string apiKey = Request.Headers["api-key"];
            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            if (!_srv.CheckAuthorized(apiKey, ip))
                return Unauthorized();
            
            using var reader = new StreamReader(jsonFile.OpenReadStream());
            var json = await reader.ReadToEndAsync();
            
            var payload = JsonConvert.DeserializeObject<PayLoad>(json);

            var result = await _srv.UploadToInComingFolder(payload);

            return result ? Ok() : BadRequest();
        }
    }
}
