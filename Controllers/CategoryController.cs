using BaltaWeb.Data;
using BaltaWeb.Extensions;
using BaltaWeb.Models;
using BaltaWeb.ViewModels;
using BaltaWeb.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata.Ecma335;

namespace BaltaWeb.Controllers
{

    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context, [FromServices] IMemoryCache cache)
        {
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });

                return Ok(new ResultViewModel<Task<List<Category>>>(categories));
                
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<List<Category>>(error: "05x4 - Falha interna no servidor"));
            }
            
        }

        private async Task<List<Category>> GetCategories(BlogDataContext context)
        {
            var categories = await context.Categories.ToListAsync();
            return categories;
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {                
                var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>(error: "05x5 - Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>(error: "05x6 - Erro no Sistema"));
            }            
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel categoryModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(error: new ResultViewModel<Category>(ModelState.GetError()));
            

            try
            {
                var category = new Category()
                {
                    Name = categoryModel.Name,
                    Slug = categoryModel.Slug
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x7 - Não foi possivel incluir"));                
            }
            catch
            {
                return BadRequest(new ResultViewModel<Category>("05x8 - Não foi possível incluir a categoria"));
            }

        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel category, [FromRoute] int id)
        {
            var categories = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categories == null)
                return NotFound(new ResultViewModel<Category>("05x9 - Não encontrado!"));

            categories.Slug = category.Slug;  
            categories.Name = category.Name;

            context.Categories.Update(categories);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{categories.Name}", new ResultViewModel<Category>(categories));
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromBody] Category category, [FromRoute] int id)
        {
            var categories = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (categories == null)
                return NotFound();

            context.Categories.Remove(categories);
            await context.SaveChangesAsync();

            return Ok(categories);
        }

    }


}
