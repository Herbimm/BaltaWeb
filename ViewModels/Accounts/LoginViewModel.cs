using System.ComponentModel.DataAnnotations;

namespace BaltaWeb.ViewModels.Accounts
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
