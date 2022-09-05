using BusinessLogic;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Models;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileCrypto.Controllers
{
    public class UserAccountController : BaseController
    {
        private readonly UserAccountService Service;
        private readonly IEmailSender Sender;
        public IConfiguration Configuration { get; }
        public UserAccountController(ControllerDependencies dependencies, UserAccountService service, IEmailSender sender, IConfiguration configuration)
           : base(dependencies)
        {
            Sender = sender;
            Service = service;
            Configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterModel();
            return View("Register", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var emailAvailable = await Service.CheckEmailAvailability(model.Email);
            var userAvailable = await Service.CheckUsernameAvailability(model.UserName);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var OK = 1;
            if (!emailAvailable)
            {
                OK = 0;
                ModelState.AddModelError("Email", "Email is already in base");
            }
            if (!userAvailable)
            {
                OK = 0;
                ModelState.AddModelError("UserName", "UserName is already in base");
            }
            if (OK == 0)
            {
                return View(model);
            }

            await Service.RegisterNewUser(model);
            var token = Service.GenerateEmailConfirmationToken(model);
            var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = model.Email }, Request.Scheme);
            Sender.SendEmail(model.Email, "Hello There", confirmationLink);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string returnURL)
        {
            var model = new LoginModel
            {
                ReturnURL = returnURL
            };
            return View(model);
        }
        public IActionResult Preview(string act, string ctl, string obj)
        {
            TempData["Data"] = obj;
            return RedirectToAction(act, ctl);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var checkEmail = Service.CheckEmail(model.Email);

            if (!await checkEmail)
            {
                ModelState.AddModelError("Email", "This Email Doesnt Exist!");
                return View(model);
            }
            var checkCredentials = Service.CheckUserCredentials(model.Email, model.Password);
            if (model.Password == String.Empty)
            {
                ModelState.AddModelError("Password", "Please type your password");
                return View(model);
            }
            if (!await checkCredentials)
            {
                ModelState.AddModelError("Password", "Your password is incorect");
                return View(model);
            }
            if (!(await Service.CheckValidity(model.Email)))
            {
                ModelState.AddModelError("Email", "This account is not yet activated");
                ModelState.AddModelError("Password", "This account is not yet activated");
                return View(model);
            }
            return RedirectToAction("Preview", "UserAccount", new { act = "LoginWith2Factor", ctl = "UserAccount", obj = JsonSerializer.Serialize(model) });
        }
        [HttpGet]
        public IActionResult LoginWith2Factor()
        {
            var loginModel = JsonSerializer.Deserialize<LoginModel>(TempData["Data"].ToString());
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var TwoFactorSecretCode = Configuration["CommonSettings:SecretCode"];
            var accountSecretKey = $"{TwoFactorSecretCode}-{loginModel.Email}";
            loginModel.Key = accountSecretKey;
            var setupCode = twoFactorAuthenticator.GenerateSetupCode("FileCrypto", "FileCrypto IP",
                Encoding.ASCII.GetBytes(accountSecretKey));
            ViewBag.BarcodeImageUrl = setupCode.QrCodeSetupImageUrl; ;
            ViewBag.SetupCode = setupCode.ManualEntryKey;
            return View(loginModel);
        }
        [HttpGet]
        public async Task<List<SelectListItem>> GetUsers(string search)
        {
            if (search != null)
            {
                return await Service.ReturnUsersForFileUpload(search);
            }
            return new List<SelectListItem>();
        }
        [HttpPost]
        public async Task<IActionResult> LoginWith2Factor(LoginModel loginModel)
        {
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var checkValidity = twoFactorAuthenticator.ValidateTwoFactorPIN(loginModel.Key, loginModel.InputCode);
            var setupCode = twoFactorAuthenticator.GenerateSetupCode("FileCrypto", "FileCrypto IP",
                Encoding.ASCII.GetBytes(loginModel.Key));
            ViewBag.BarcodeImageUrl = setupCode.QrCodeSetupImageUrl; ;
            ViewBag.SetupCode = setupCode.ManualEntryKey;

            if (!checkValidity)
            {
                ModelState.AddModelError("InputCode", "Your input code is incorect");
                return View();
            }
            var user = await Service.LoginAsync(loginModel.Email, loginModel.Password);

            if (!user.IsAuthenticated)
            {
                loginModel.AreCredentialsInvalid = true;
                return View();
            }

            await LogIn(user);
            if (!String.IsNullOrEmpty(loginModel.ReturnURL))
            {
                return Redirect(loginModel.ReturnURL);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await LogOut();

            return RedirectToAction("Index", "Home");
        }
        private async Task LogIn(CurrentUserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.UserName)
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                    scheme: "FileCryptoCookies",
                    principal: principal);
        }
        [HttpGet]
        public async Task<ActionResult> UserProfile(Guid id)
        {
            var model = await Service.GetUserProfileModel(id);
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> EditUser(Guid id)
        {
            if (CurrentUser.UserId == id)
            {
                var model = await Service.GetUserProfileModel(id);
                return View("EditUser", model);
            }
            return View("AccessDenied");
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserProfileModel model)
        {
            if (CurrentUser.UserId == model.UserId)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var OK = 1;
                if (!await Service.CheckEmailAvailability(model.Email, CurrentUser.Email))
                {
                    OK = 0;
                    ModelState.AddModelError("Email", "Email is already in base");
                }
                if (!await Service.CheckUsernameAvailability(model.UserName, CurrentUser.UserName))
                {
                    OK = 0;
                    ModelState.AddModelError("UserName", "UserName is already in base");
                }
                if (OK == 0)
                {
                    return View(model);
                }

                await Service.EditUser(model);

                return RedirectToAction("UserProfile", "UserAccount", new { id = model.UserId.ToString() });
            }
            else
            {
                return View("AccessDenied");
            }
        }
        private async Task LogOut() => await HttpContext.SignOutAsync(scheme: "FileCryptoCookies");
    }
}
