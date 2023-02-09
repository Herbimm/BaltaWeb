using BaltaWeb.Data;
using BaltaWeb.ViewModels.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace BaltaWeb.Controllers
{
    [Route("v1/post/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("get")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context, [FromQuery]int page, [FromQuery]int pageSize = 25)
        {
            var posts = await context.Posts.AsNoTracking().Include(x => x.Author).Include(x=>x.Category).Select( x => new ListPostsViewModel
            {
                Id = x.Id,                
                Category = x.Category.Name,
                Author = $"{x.Author.Name} {x.Author.Email}",
                Slug = x.Slug,
                Title = x.Title 
            })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("postWithFilters")]
        public async Task<IActionResult> GetAsyncWithFilters([FromServices] BlogDataContext context, [FromQuery] int page, [FromQuery] int pageSize = 25)
        {
            var posts = await context.Posts.AsNoTracking().Include(x => x.Author).ThenInclude(x => x.Posts).Include(x => x.Category).Select(x => new ListPostsViewModel
            {
                Id = x.Id,
                Category = x.Category.Name,
                Author = $"{x.Author.Name} {x.Author.Email}",
                Slug = x.Slug,
                Title = x.Title
            })
                .Skip(page * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("teste")]
        public async Task<IActionResult> GetTeste([FromServices] BlogDataContext context)
        {
            var posts = await context.Categories.ToListAsync();
            return Ok(posts);
        }
    }
}
