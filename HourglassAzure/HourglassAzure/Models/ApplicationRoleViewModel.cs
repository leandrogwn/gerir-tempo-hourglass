using System.ComponentModel.DataAnnotations;

namespace HourglassAzure.Models
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }
        [Display(Name ="Role Name")]
        public string RoleName { get; set; }
    }
}
