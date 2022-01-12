using MessengerWeb.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerWeb.Server.Services
{
    public class ApiRequestsService
    {
        private readonly HttpClient _httplClient;
        private readonly ILogger<ApiRequestsService> _logger;
        private readonly IConfiguration _configuration;

        public ApiRequestsService(ILogger<ApiRequestsService> logger, IConfiguration configuration)
        {
            _httplClient = new HttpClient();
            _logger = logger;
            _configuration = configuration;
        }

        internal async Task<FileHashEntity> GetExternalApiFileHash(byte[] data)
        {
            var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(data, 0, data.Length), "data", "image");
            var fileUploadResponse = await _httplClient.PostAsync(_configuration["ApiGates:UploadFile"], form);
            string content = await fileUploadResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FileHashEntity>(content);
        }

        internal async Task<FaceApiTaskResponse> ProcessTask(string url, string fileHash, string engineId)
        {
            HttpResponseMessage response = new();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
            {
                var livenessBodyRequest = new StringContent(JsonSerializer.Serialize(new FaceApiTaskRequest()
                {
                    EngineId = engineId,
                    FileHash = fileHash
                }));
                request.Content = livenessBodyRequest;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                response = await _httplClient.SendAsync(request);
            }
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FaceApiTaskResponse>(content);
        }


        internal async Task<IFaceApiTaskResult> GetTaskResult(string taskId, Operation operation)
        {
            HttpResponseMessage response = new();
            string url = $"{_configuration["ApiGates:GetTaskResult"]}?uuid={taskId}";
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
            {
                request.Headers.TryAddWithoutValidation("accept", "application/json");
                response = await _httplClient.SendAsync(request);
            }
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                switch(operation)
                {
                    case Operation.Liveness:
                        return JsonSerializer.Deserialize<LivenessTaskResult>(content);
                    case Operation.Register:
                    case Operation.Match:
                        return JsonSerializer.Deserialize<CommonTaskResult>(content);
                    default:
                        return null;
                }
            }
            else
                return JsonSerializer.Deserialize<FaceApiTaskResultError>(content);

        }

    }
}