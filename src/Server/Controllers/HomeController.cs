using MessengerWeb.Server.Services;
using MessengerWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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

        [HttpPost("liveness/{engineId?}")]
        public async Task<IActionResult> PostFrameGetLiveness(string engineId, [FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");
            string content = "Liveness error. ";
            using (Stream stream = file.OpenReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();

                var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                var livenessTask = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetLivenessTask"], 
                                                                            fileHashResponse.Hash, 
                                                                            engineId);
                await Task.Delay(500);
                var livenessTaskResult = await _apiRequestsService.GetTaskResult(livenessTask.TaskId, Operation.Liveness);
                if (livenessTaskResult is LivenessTaskResult)
                {
                    var livenessResult = (LivenessTaskResult)livenessTaskResult;
                    content = livenessResult.Result.Score.ToString();
                    return Ok(content);
                }
            }
            return BadRequest(content);
        }

        [HttpPost("match/{engineId?}")]
        public async Task<IActionResult> PostFrameGetMatch(string engineId, [FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");
            string content = "Match error. ";

            using (Stream stream = file.OpenReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();

                var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                var matchTask = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetBestMatchTask"], 
                                                                        fileHashResponse.Hash,
                                                                        engineId);
                await Task.Delay(500);
                var matchTaskResult = await _apiRequestsService.GetTaskResult(matchTask.TaskId, Operation.Match);
                if (matchTaskResult is CommonTaskResult)
                {
                    var matchResult = (CommonTaskResult)matchTaskResult;
                    if (matchResult.Result?.FaceId is not null)
                    {
                        content = matchResult.Result?.FaceId;
                        return Ok(content);
                    }
                }
            }
            return BadRequest(content);
        }

        [HttpPost("register/{engineId?}")]
        public async Task<IActionResult> Post(string engineId, [FromForm(Name = "data")] IFormFile file)
        {
            if (file is null)
                return StatusCode(400, "No photo, formFile is null");
            string content = "Match failed. ";

            using (Stream stream = file.OpenReadStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();

                var fileHashResponse = await _apiRequestsService.GetExternalApiFileHash(bytes);
                var registerTaskResponse = await _apiRequestsService.ProcessTask(_configuration["ApiGates:GetRegisterTask"], 
                                                                                    fileHashResponse.Hash,
                                                                                    engineId);
                await Task.Delay(500);
                var registerTaskResult = await _apiRequestsService.GetTaskResult(registerTaskResponse.TaskId, Operation.Register);
                if (registerTaskResult is CommonTaskResult)
                {
                    var matchResult = (CommonTaskResult)registerTaskResult;
                    if (matchResult.Result.FaceId is not null)
                    {
                        content = matchResult.Result.FaceId;
                        return Ok(content);
                    }

                }
            }
            return Ok(content);
        }
    }
}
