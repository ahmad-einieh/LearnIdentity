using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(555)]
        public string name { get; set; }    

    }
}
