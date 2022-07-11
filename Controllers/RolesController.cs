using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager) { 
        _roleManager = roleManager;
        }



        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel role) {
            if (!ModelState.IsValid) {
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            var roleIsExist = await _roleManager.RoleExistsAsync(role.RoleName);

            if (roleIsExist) {
                ModelState.AddModelError("Name", "Role is exist");
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new IdentityRole(role.RoleName.Trim()));
            return RedirectToAction(nameof(Index));
        }


    }

}
