using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class RoleFormViewModel
    {
        [Required,StringLength(256)]
        public string RoleName { get; set; }
    }
}
