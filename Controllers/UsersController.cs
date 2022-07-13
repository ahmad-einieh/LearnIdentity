using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using WebApplication1.Models;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{

    [Authorize(Roles="Admin")]
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;


        public UsersController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
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

        public async Task<IActionResult> Add() {

            var roles = await _roleManager.Roles.Select(r=> new RoleViewModel { RoleId = r.Id,RoleName = r.Name}).ToListAsync();
            var viewModel = new AddUserViewModel
            {
               roles = roles,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            //Console.Write("here => "+ModelState.Values.ToString());
            Console.Write("here => " + ModelState.IsValid);
            if(!ModelState.IsValid) return View(model);
            if (!model.roles.Any(r => r.IsSelecte)) {
                ModelState.AddModelError("Roles", "please select at least one role");
                return View(model); 
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null) {
                ModelState.AddModelError("Email", "email already exist");
                return View(model);
            
            }
            if (await _userManager.FindByNameAsync(new MailAddress(model.Email).User) != null)
            {
                ModelState.AddModelError("UserName", "username already exist");
                return View(model);
            }
            var user = CreateUser();
            user.name = model.name;
            await _userStore.SetUserNameAsync(user, new MailAddress(model.Email).User, CancellationToken.None);
            await GetEmailStore().SetEmailAsync(user, model.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
                return View(model);
            }

            await _userManager.AddToRolesAsync(user,model.roles.Where(r=>r.IsSelecte).Select(r=>r.RoleName));
            return RedirectToAction(nameof(Index));
        }


        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }


        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new ProfileFormViewModel
            {
                Id = userId,
                name = user.name,
                Email = user.Email,
                //UserName = user.UserName,
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormViewModel model)
        {
            if(!ModelState.IsValid) return View(model); 
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();
           
            var userWithSameEmail = await _userManager.FindByEmailAsync(user.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id !=model.Id)
            {
                ModelState.AddModelError("Email", "this email already used");
                return View(model);
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(user.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != model.Id)
            {
                ModelState.AddModelError("UserNmae", "this UserName already used");
                return View(model);
            }

            user.name = model.name;
            user.Email = model.Email;
            user.UserName = new MailAddress(model.Email).User;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }



    }
}
