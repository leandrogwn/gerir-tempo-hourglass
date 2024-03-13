using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HourglassAzure.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "Senha e confirmação de senha são diferentes.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Perfis de usuário : ")]
        [UIHint("List")]
        public List<SelectListItem> Roles { get; set; }

        public string Role { get; set; }

        public RegisterViewModel()
        {
            Roles = new List<SelectListItem>();
            Roles.Add(new SelectListItem() { Value = "1", Text = "Dev/An/Qa" });
            Roles.Add(new SelectListItem() { Value = "2", Text = "Po" });
            Roles.Add(new SelectListItem() { Value = "3", Text = "Pmo/Scrum" });
        }
    }
}
