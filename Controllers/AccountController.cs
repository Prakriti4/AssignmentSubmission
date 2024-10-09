using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SchoolManagementSystem.Helper;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.ViewModel;

public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserStore<User> userStore,
        ILogger<AccountController> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _logger = logger;
        _emailSender = emailSender;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            string uniquefile = HandleImageUpload.UploadedFile(model.Image);
            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber = model?.PhoneNumber,
                Image = model.Image,
                ImageUrl = uniquefile,
                DateOfBirth = model?.DateOfBirth,
            };
            await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, model.Password);
            string roleName = model.Role;

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);

                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)));

                if (roleName == "Teacher") { return RedirectToAction(nameof(Index), "Teacher"); }
                else { return RedirectToAction(nameof(Index), "Student"); }

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

        }
        return View(model);
    }
    private IUserEmailStore<User> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<User>)_userStore;
    }
}