using AutoMapper;
using BaltaWeb.Interfaces;
using Newtonsoft.Json;

namespace BaltaWeb.Services
{
    public class TestService<T> : ITesteService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public TestService(HttpClient httpClient, IMapper mapper)
        {
            _mapper = mapper;
            httpClient.BaseAddress = new Uri("https://api.senior.com.br");
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string address)
        {         
          
          var result = await _httpClient.GetAsync(address);
          var stringResult = result.Content.ReadAsStringAsync().Result;         
          var deserializer = JsonConvert.DeserializeObject<T>(stringResult);          

          return deserializer;
        }


    }
}
