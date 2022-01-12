using MessengerWeb.Server.Services;
using MessengerWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApiRequestsService apiRequestsService, IConfiguration configuration)
        {
            _logger = logger;
            _apiRequestsService = apiRequestsService;
            _configuration = configuration;
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
                    var livenessTask = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetLivenessTask"], 
                                                                             fileHashResponse.Hash, 
                                                                             "fea041df-4e7e-4e59-ae9a-68a4500a1754");
                    await Task.Delay(500);
                    var livenessTaskResult = await _apiRequestsService.GetTaskResult(livenessTask.TaskId, Operation.Liveness);
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
        public async Task<IActionResult> PostFrameGetMatch([FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");

            string content = "Match validation error. ";
            using (Stream stream = file.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var bytes = ms.ToArray();

                    var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                    var matchTask = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetBestMatchTask"], 
                                                                          fileHashResponse.Hash, 
                                                                          "fea041df-4e7e-4e59-ae9a-68a4500a1754");
                    await Task.Delay(500);
                    var matchTaskResult = await _apiRequestsService.GetTaskResult(matchTask.TaskId, Operation.Match);
                    if (matchTaskResult is CommonTaskResult)
                    {
                        var matchResult = (CommonTaskResult)matchTaskResult;
                        content = matchResult.Result?.FaceId;
                    }
                }
            }
            return Ok(content);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post([FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");

            using (Stream stream = file.OpenReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();

                var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                var livenessTaskResponse = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetRegisterTask"], 
                                                                                    fileHashResponse.Hash, 
                                                                                    "fea041df-4e7e-4e59-ae9a-68a4500a1754");
                await Task.Delay(500);
                var matchTaskResult = await _apiRequestsService.GetTaskResult(livenessTaskResponse.TaskId, Operation.Register);
                var matchResult = (CommonTaskResult)matchTaskResult;
                return matchResult.Status == "failed" ? StatusCode(409, matchResult.Status) 
                                                      : Ok(matchResult.Result.FaceId);
            }
        }
    }
}
