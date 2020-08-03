using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DemoApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IOrderRepository orderRepository;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          IOrderRepository orderRepository,
            ISmsSender smsSender, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.orderRepository = orderRepository;
            _emailSender = emailSender;
            _smsSender = smsSender;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl)
        {
            RegisterViewModel model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {

                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                       $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");

                    //  await signInManager.SignInAsync(user, isPersistent: false);
                    //                    await _emailSender.SendEmailAsync(model.Email, "Thank you from Prince Digital", $"<h1>Prince Digital</h1> <h2>Thank you for register <br/> Happy shopping </h2><a>link</a>");
                    //return RedirectToAction("index", "home");

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                        "email, by clicking on the confirmation link we have emailed you";
                    return View("Error");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });
            var properties = signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {

                var user = await userManager.FindByEmailAsync(model.Email);



                if (user != null && !user.EmailConfirmed &&
                                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    ModelState.AddModelError(string.Empty, "New Link is sent to email. Please click this link.");
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                       $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(
                  model.Email, model.Password, model.RememberMe, false);



                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("index", "Administration");
                        }
                        return RedirectToAction("index", "home");
                    }


                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? ""
                        };

                        var result = await userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {

                            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = Url.Action("AddPassword", "Account",
                            new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                            await _emailSender.SendEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email), "Confirm your account",
                               $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                        }
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    if (user.EmailConfirmed)
                    {
                        await userManager.AddLoginAsync(user, info);
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        ViewBag.ErrorTitle = "Your Registration is successfully. Please check your mail and comfirm your account.";
                        ViewBag.ErrorTitle1 = " Please check spam as well";
                        LoginViewModel model = new LoginViewModel
                        {
                            ReturnUrl = returnUrl,
                            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                        };

                        return View("Login", model);

                    }


                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            ViewBag.ErrorTitle = "Registration successful";
            ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                "email, by clicking on the confirmation link we have emailed you";
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                        $"Please confirm your account by clicking this link: <a href='{passwordResetLink}'>link</a>");

                    return View("ForgotPasswordConfirmation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email id not found. Please register");
                    ModelState.AddModelError(string.Empty, "or Email is not comfirm");
                    return View(model);
                }


            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]

        public async Task<IActionResult> AddPassword(string userId, string code)
        {

            bool isSuccessLogin = false;
            if (userId == null && code == null)
            {

                if (HttpContext.User != null)
                {
                    var user = await userManager.GetUserAsync(HttpContext.User);
                    if (user != null)
                    {
                        return View();
                    }
                }
                return await LoadLoginPage();
            }
            else
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ViewBag.ErrorTitle = "Please Register or Login";
                    LoginViewModel model = new LoginViewModel
                    {
                        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                    };

                    return View("Login", model);
                }
                var result = await userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    isSuccessLogin = true;
                }
            }

            if (isSuccessLogin)
            {
                return View();
            }

            return await LoadLoginPage();


        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return RedirectToAction("Index", "Home", new { msg = "sucess" });

            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        public async Task<IActionResult> LoadLoginPage()
        {
            ViewBag.ErrorTitle = "Please Register or Login";
            LoginViewModel model = new LoginViewModel
            {
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View("Login", model);
        }


        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }

        }

        [Authorize]
        public async Task<IActionResult> YourOrders()
        {
            ApplicationUser user = await userManager.GetUserAsync(HttpContext.User);
            List<Order> orderList = orderRepository.UserorderList(user.Id);
            return View(orderList);
        }  
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int orderid)
        {
            int IsSuccess = orderRepository.CancelOrder(orderid);
            if(IsSuccess > 0)
            {
                ViewBag.msg = "Order Cancel Successful";
            }
            else
            {
                ViewBag.msg = "Order Cancel is not cancelled";
            }
            ApplicationUser user = await userManager.GetUserAsync(HttpContext.User);
            List<Order> orderList = orderRepository.UserorderList(user.Id);
            return Redirect(Url.ActionLink("YourOrders", "Account"));
          //  LocalRedirect(Url.ActionLink("Account", "YourOrders"));
            //return View("YourOrders", orderList);
        }
    }
}
    