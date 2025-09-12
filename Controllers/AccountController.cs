using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskManagementMvc.Models;
using TaskManagementMvc.Models.ViewModels;

namespace TaskManagementMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Update last login time
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        user.LastLoginAt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه است.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewData["UserRoles"] = roles;

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                UserId = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["SuccessMessage"] = "رمز عبور با موفقیت تغییر یافت.";
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام کامل الزامی است")]
        [Display(Name = "نام کامل")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "فرمت شماره تلفن صحیح نیست")]
        [Display(Name = "شماره تلفن")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "تأیید رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام شرکت الزامی است")]
        [Display(Name = "نام شرکت")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "توضیحات شرکت")]
        public string? CompanyDescription { get; set; }

        [Display(Name = "یادداشت‌ها")]
        public string? Notes { get; set; }

        // Properties expected by RegisterPipeline
        [Display(Name = "توضیحات")]
        public string? Description => CompanyDescription;

        [Display(Name = "تلفن")]
        public string? Phone => PhoneNumber;
    }
}