using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HourglassAzure.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "O {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Código de autenticação")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Lembrar essa maquina")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
