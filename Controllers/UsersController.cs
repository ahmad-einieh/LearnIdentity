using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{

    [Authorize(Roles="Admin")]
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Select(user => new UserViewModel { 
                Id = user.Id,
                name = user.name,
                Email = user.Email,
                UserName = user.UserName,
                Roles = _userManager.GetRolesAsync(user).Result


            }).ToListAsync();

            return View(users);
        }


        public async Task<IActionResult> ManageRoles(string userId) {
            var usre = await _userManager.FindByIdAsync(userId);
            if (usre == null) return NotFound();
            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel {

                userId = usre.Id,
                userName = usre.UserName,
                roles = roles.Select(role => new RoleViewModel { 
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelecte = _userManager.IsInRoleAsync(usre,role.Name).Result
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.userId);
            if (user == null) return NotFound();
            var userRole = await _userManager.GetRolesAsync(user);
            foreach(var role in model.roles)
            {
                if(userRole.Any(r => r == role.RoleName) && !role.IsSelecte) await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                if(!userRole.Any(r => r == role.RoleName) && role.IsSelecte) await _userManager.AddToRoleAsync(user, role.RoleName);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
