using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerWeb.Client.Services
{
    public class EnginesHttpRepository
    {
        public async Task<string> Post(string imageBytes, HttpClient Http, string url, string engineId)
        {
            var data = Convert.FromBase64String(imageBytes);
            var contents = new StreamContent(new MemoryStream(data));
            var form = new MultipartFormDataContent();
            form.Add(contents, "data", "image");
            var response = await Http.PostAsync($"{url}/{engineId}", form);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
