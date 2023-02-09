using System.ComponentModel.DataAnnotations;

namespace BaltaWeb.ViewModels.Categories
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Slug é obrigatório")]
        [MinLength(10, ErrorMessage = "Quantidade de caracteres minimo 10")]
        public string Slug { get; set; }
    }
}
