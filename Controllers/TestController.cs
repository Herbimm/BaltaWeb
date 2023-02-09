using BaltaWeb.Interfaces;
using BaltaWeb.Services;
using BaltaWeb.ViewModels.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaltaWeb.Controllers
{   

    [Route("v1/test/")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITesteService _testeService;

        public TestController(ITesteService testeService)
        {
            _testeService = testeService;
        }


        [HttpGet("test")]
        public async Task<IActionResult> GetAsync()
        {           

           var result = await _testeService.GetAsync<AdviceViewModel>("advice");

            return Ok(result);
        }
    }
}
