using AutoMapper;
using BlueCorp.Common;
using BlueCorp.D365.Contract;
using BlueCorp.D365.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlueCorp.D365.Controller
{
    [Route("api/dispatch")]
    [ApiController]
    public class DispatchController : ControllerBase
    {
        private readonly IDispatchService _srv;
        private readonly IMapper _mapper;

        public DispatchController(IDispatchService srv, IMapper mapper)
        {
            _srv = srv;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post(DispatchRequest request)
        {
            PayLoad payLoad;
            try
            {
                payLoad = _mapper.Map<PayLoad>(request);
            }
            catch
            {
                return BadRequest("Container Type is incorrect");
            }
            var (result, message) = await _srv.ProcessAsync(payLoad);
            return result ? Ok() : BadRequest(message);
        }
    }
}
