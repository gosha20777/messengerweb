using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MessengerWeb.Client.Services
{
    public class Register
    {
        public async Task<string> Get(string imageBytes, HttpClient Http)
        {
            var data = Convert.FromBase64String(imageBytes);
            var content = new StreamContent(new MemoryStream(data));
            var form = new MultipartFormDataContent();
            form.Add(content, "data", "image");
            var response = await Http.PostAsync("Home/register", form);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
