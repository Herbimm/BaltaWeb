using BaltaWeb.Interfaces;
using BaltaWeb.ViewModels.SeniorApi;
using Newtonsoft.Json;
using System.Text;

namespace BaltaWeb.Services
{
    public class SeniorAuthenticationService : ISeniorAuthenticationService
    {
        private readonly HttpClient _client;

        public SeniorAuthenticationService(HttpClient client)
        {
            _client = client;
        }

        public async Task DetectGoogleTranslatorAsync()
        {

            var parameters = new SeniorParameters
            {
                Q = "Bom dia"
            };            

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://google-translate1.p.rapidapi.com/language/translate/v2/detect"),
                Headers =
                {
                    { "X-RapidAPI-Key", "e2316ecae7msh80e47e89cbf9638p1c0cf8jsnbacd1141848b" },
                    { "X-RapidAPI-Host", "google-translate1.p.rapidapi.com" }
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { $"{nameof(parameters.Q).ToLower()}", $"{parameters.Q}" },
                })
            };            

            using (var response = await _client.SendAsync(request))
            {
                var readResponse = response.Content.ReadAsStringAsync();
            }
        }
        public async Task PostUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}
