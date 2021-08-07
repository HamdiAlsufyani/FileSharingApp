using AutoMapper;
using FileSharingPractice.Data;
using FileSharingPractice.Helpers.Mail;
using FileSharingPractice.Models;
using FileSharingPractice.Resources;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileSharingPractice.Controllers
{
    public class AccountController : Controller
    {
        private SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SharedResource> stringLocalizer;
        private readonly IMailService mailService;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IMapper mapper, IStringLocalizer<SharedResource> stringLocalizer, IMailService mailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.mapper = mapper;
            this.stringLocalizer = stringLocalizer;
            this.mailService = mailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModels models, string returnurl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(models.Email, models.Password, true, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnurl))
                    {
                        return LocalRedirect(returnurl);
                    }
                    return RedirectToAction("Create", "Uploads");
                }
                else if (result.IsNotAllowed)
                {
                    TempData["error"] = stringLocalizer["RequireEmailConfirmation"]?.Value;
                }
            }

            return View(models);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModels models)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = models.Email,
                    Email = models.Email,
                    FirstName = models.FirstName,
                    LastName = models.LastName
                };
                var result = await userManager.CreateAsync(user, models.Password);
                if (result.Succeeded)
                {
                    //Create Link
                    var token = userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Account", new { token = token, userid = user.Id }, Request.Scheme);
                    //Send Email
                    StringBuilder body = new StringBuilder();
                    body.AppendLine("File Sharng Appplication :Email Confirmation");
                    body.AppendLine("We are sending this email");
                    body.AppendFormat("to confirm your Email , you should <a href'{0}'> click here</a>", url);

                    mailService.SendEmail(new InputEmailMessage
                    {
                        Body = body.ToString(),
                        Email = models.Email,
                        Subject = "Email Confirmation"
                    });
                    return RedirectToAction("RequirEmailConfirm");
                    //await signInManager.SignInAsync(user, false);
                    //return RedirectToAction("Create", "Uploads");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(models);
        }
        [HttpGet]
        public IActionResult RequirEmailConfirm()
        {
            return View();
        }
        //[HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
            //return View();
        }

        public IActionResult ExternalLogin(string provider) //Provider ="Facebook","Google"
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, "/Account/ExternalResponse");
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalResponse()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Message"] = "Login Field";
                return RedirectToAction("Login");
            }
            var loginresult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (!loginresult.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                //Create
                var userToCreate = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var CreateResult = await userManager.CreateAsync(userToCreate);//AspNetUseras
                if (CreateResult.Succeeded)
                {
                    var exLoginResult = await userManager.AddLoginAsync(userToCreate, info); //AspNetUserLogin Table
                    if (exLoginResult.Succeeded)
                    {
                        await signInManager.SignInAsync(userToCreate, false, info.LoginProvider);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await userManager.DeleteAsync(userToCreate);
                    }
                }
                return RedirectToAction("Login");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Info()
        {
            var curentuser = await userManager.GetUserAsync(User);
            if (curentuser != null)
            {
                var model = mapper.Map<UserViewModel>(curentuser);
                return View(model);
            }
            return NotFound();

        }
        [HttpPost]
        public async Task<IActionResult> Info(UserViewModel model)
        {
            if (ModelState.IsValid)
            {


                var curentuser = await userManager.GetUserAsync(User);
                if (curentuser != null)
                {
                    curentuser.FirstName = model.FirstName;

                    curentuser.LastName = model.LastName;

                    var result = await userManager.UpdateAsync(curentuser);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = stringLocalizer["SuccessMessage"]?.Value;
                        return RedirectToAction("Info");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);

                }
                else
                {
                    return NotFound();
                }

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var curentuser = await userManager.GetUserAsync(User);
            if (curentuser != null)
            {

                if (ModelState.IsValid)
                {
                    var result = await userManager.ChangePasswordAsync(curentuser, model.CurentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await signInManager.SignOutAsync();
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                return NotFound();
            }
            return View("Info", mapper.Map<UserViewModel>(curentuser));
        }

        [HttpPost]
        public async Task<IActionResult> AssPassword(AddPasswordViewModel model)
        {
            var curentuser = await userManager.GetUserAsync(User);
            if (curentuser != null)
            {

                if (ModelState.IsValid)
                {
                    var result = await userManager.AddPasswordAsync(curentuser, model.Password);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = stringLocalizer["AddPasswordMessage"]?.Value;
                        return RedirectToAction("Info");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                return NotFound();
            }
            return View("Info", mapper.Map<UserViewModel>(curentuser));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        var result = await userManager.ConfirmEmailAsync(user, model.Token);
                        if (result.Succeeded)
                        {
                            TempData["Success"] = "Your Email confirm Successfully";
                            return RedirectToAction("Login");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    else
                    {
                        TempData["Success"] = "Your Email already confirm";
                    }

                }

            }
            return View();
        }
        [HttpGet]
        public IActionResult ForgetPssword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPssword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existedUser = await userManager.FindByEmailAsync(model.Email);
                if (existedUser != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(existedUser);
                    var url = Url.Action("ResetPassword", "Account", new { token, model.Email }, Request.Scheme);
                    StringBuilder body = new StringBuilder();
                    body.AppendLine("File Sharng Appplication :Email Confirmation");
                    body.AppendLine("We are sending this email, Becouse we have a reset password request to your account");
                    body.AppendFormat("to reset a new password , you should <a href'{0}'> click here</a>", url);

                    mailService.SendEmail(new InputEmailMessage
                    {
                        Email = model.Email,
                        Subject = "Resset Password",
                        Body = body.ToString()
                    });

                }
                TempData["Success"] = "If your email matches  an exist account in our system,you should receive an email";
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(veriefyPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeduser = await userManager.FindByEmailAsync(model.Email);
                if (existeduser != null)
                {
                    var isvalid = await userManager.VerifyUserTokenAsync(existeduser, TokenOptions.DefaultProvider, "ReserPassword", model.Token);
                    if (isvalid)
                    {
                        return View();
                    }
                    else
                    {
                        TempData["error"] = "Token is invalid";
                    }
                }
            }
            return RedirectToAction("Login");

        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeuser = await userManager.FindByEmailAsync(model.Email);
                if (existeuser != null)
                {
                    var result = await userManager.ResetPasswordAsync(existeuser, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Reset password completed successfully!";
                        return RedirectToAction("Login");

                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
