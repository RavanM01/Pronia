using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaTask.Abstractions;
using ProniaTask.DAL;
using ProniaTask.Helpers.Enums;
using ProniaTask.Models;
using ProniaTask.ViewModels.Account;
using IMailService = ProniaTask.Abstractions.IMailService;

namespace ProniaTask.Controllers
{
    public class AccountController : Controller
    {
        AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService mailService;

        public AccountController(AppDBContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IMailService mailService
)
        {
            _context = context;
            this._userManager = userManager;
            _signInManager = signInManager;
            this._roleManager = roleManager;
            this.mailService = mailService;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser();
            {
                user.Email = vm.Email;
                user.Name = vm.Name;
                user.Surname = vm.Surname;
                user.UserName = vm.Name + vm.Surname;

            }
            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, HttpContext.Request.Scheme);
            await mailService.SendEmailAsync(new MailRequest()
            {
                Subject = "Confirm Email",
                ToEmail = vm.Email,
                Body = $"<p>{user.Name} Confirmliyin emaili </p><br><a href='{link}'>Confirm Email</a>"
            });

            await _userManager.AddToRoleAsync(user, UserRoles.Member.ToString());

            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await _userManager.FindByEmailAsync(vm.EmailOrUsername) ?? await _userManager.FindByNameAsync(vm.EmailOrUsername);
            if (user == null)
            {
                ModelState.AddModelError("", "melumatlari duzgun daxil edin");
                return View();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, vm.Password, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Yeniden yoxlayin");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "melumatlari duzgun daxil edin");
                return View();
            }

            await _signInManager.SignInAsync(user, vm.RememberMe);
            if (ReturnUrl != null)
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                await _roleManager.CreateAsync(new IdentityRole()
                {
                    Name = item.ToString(),
                });

            }

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm vm)
        {
            if (!ModelState.IsValid) { return View(); }
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null) { return NotFound(); }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string link = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, HttpContext.Request.Scheme);
            await mailService.SendEmailAsync(new MailRequest()
            {
                Subject = "Reset Password",
                ToEmail = vm.Email,
                Body = $"<p>{user.Name} Resetliyin kodu </p><br><a href='{link}'>Reset Password</a>"
            });
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm vm, string userId, string token)
        {
            if (!ModelState.IsValid) { return View(); }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }
            var result = await _userManager.ResetPasswordAsync(user, token, vm.Password);
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }
            var user = await _userManager.FindByIdAsync(userId); 
            if (user == null) { return NotFound(); };
            var result=await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
    }
}  
