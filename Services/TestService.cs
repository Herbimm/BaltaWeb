using AutoMapper;
using BaltaWeb.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaltaWeb.Services
{
    public class TestService<T> : ITesteService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public TestService(HttpClient httpClient, IMapper mapper)
        {
            _mapper = mapper;
            httpClient.BaseAddress = new Uri("https://api.adviceslip.com");
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string address)
        {         
          
          var result = await _httpClient.GetAsync(address);
          var stringResult = result.Content.ReadAsStringAsync().Result;         
          var desc2 = JsonConvert.DeserializeObject<T>(stringResult);          

          return desc2;
        }


    }
}
