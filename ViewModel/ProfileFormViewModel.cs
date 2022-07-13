using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel
{
    public class ProfileFormViewModel
    {

        public string Id { get; set; }  

        [Required]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /*[Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }*/

       // public List<RoleViewModel> roles { get; set; }
    }
}
