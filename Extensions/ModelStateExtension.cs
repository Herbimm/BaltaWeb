using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace BaltaWeb.Extensions
{
    public static class ModelStateExtension
    {
        public static List<string> GetError(this ModelStateDictionary modelState)
        {

            var result = (from itemError in modelState.Values
                          from item in itemError.Errors
                          select item.ErrorMessage).ToList();

            return result;
        }
    }
}
