using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerWeb.Client.Services
{
    public class LivenessService
    {
        public async Task<double> Get(string imageBytes, HttpClient Http)
        {
            var data = Convert.FromBase64String(imageBytes); // get the image as byte array
            var contents = new StreamContent(new MemoryStream(data));
            var form = new MultipartFormDataContent();
            form.Add(contents, "data", "image");
            var response = await Http.PostAsync("Home/liveness", form);

            string responseContent = await response.Content.ReadAsStringAsync();
            return Convert.ToDouble(responseContent);
        }
    }
}
