using MessengerWeb.Server.Services;
using MessengerWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MessengerWeb.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly ApiRequestsService _apiRequestsService;

        public HomeController(ILogger<HomeController> logger, ApiRequestsService apiRequestsService)
        {
            _logger = logger;
            _apiRequestsService = apiRequestsService;
        }

        [HttpPost("liveness")]
        public async Task<IActionResult> PostFrameGetLiveness([FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");
            string content = "Liveness validation error. ";
            using (Stream stream = file.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var bytes = ms.ToArray();

                    var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                    var livenessTaskResponse = await _apiRequestsService.GetExternalApiLivenessTaskId(fileHashResponse.Hash, "fea041df-4e7e-4e59-ae9a-68a4500a1754");
                    await Task.Delay(500);
                    var livenessTaskResult = await _apiRequestsService.GetLivenessTaskResult(livenessTaskResponse.TaskId);
                    if (livenessTaskResult is LivenessTaskResult)
                    {
                        var livenessResult = (LivenessTaskResult)livenessTaskResult;
                        content = livenessResult.Result.Score.ToString();
                    }
                }
            }
            return Ok(content);
        }

        [HttpPost("match")]
        public async Task<IActionResult> Post([FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");

           /* string content = String.Empty;
            using (Stream stream = file.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var bytes = ms.ToArray();

                    string fileHash = await _apiRequestsService.GetExternalApiFileHash(bytes);
                    var livenessTaskResponse = await _apiRequestsService.GetExternalApiLivenessTaskId(fileHash, "fea041df-4e7e-4e59-ae9a-68a4500a1754");
                    var livenessTaskResult = await _apiRequestsService.GetLivenessTaskResult(livenessTaskResponse.TaskId);
                    if (livenessTaskResult is FaceApiTaskResult)
                    {
                        var livenessResult = (FaceApiTaskResult)livenessTaskResult;
                        content = livenessResult.Result.FaceId;
                    }
                    else
                        message += "Liveness validation error. ";
                }
            }*/
            return Ok();
        }
    }
}
