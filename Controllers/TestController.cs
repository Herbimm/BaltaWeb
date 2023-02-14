using BaltaWeb.Interfaces;
using BaltaWeb.Services;
using BaltaWeb.ViewModels.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BaltaWeb.Controllers
{   

    [Route("v1/test/")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISeniorAuthenticationService _seniorAuthenticationService;
        private readonly ITesteService _testeService;

        public TestController(ITesteService testeService, ISeniorAuthenticationService seniorAuthenticationService)
        {
            _seniorAuthenticationService = seniorAuthenticationService;
            _testeService = testeService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("test")]
        public async Task<IActionResult> GetAsync()
        {           

           var result = await _testeService.GetAsync<AdviceViewModel>("advice");

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("login")]
        public async Task<IActionResult> LoginAsync()
        {
            await _seniorAuthenticationService.DetectGoogleTranslatorAsync();
            return Ok();
        }


    }
}
