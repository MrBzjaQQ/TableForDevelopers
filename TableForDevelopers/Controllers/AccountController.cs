using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TableForDevelopers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using TableForDevelopers.App_Start;

namespace TableForDevelopers.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (UserManager.Users.Where(i => i.Email == model.Email).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Данная почта уже используется");
                return PartialView(model);
            }
            if (ModelState.IsValid)
            {
                UserType rights = UserType.Customer;
                switch (model.UserType)
                {
                    case "Developer": { rights = UserType.Developer; break; }
                    case "Customer": { rights = UserType.Customer; break; }
                    case "TeamLeader": { rights = UserType.TeamLeader; break; }

                }
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Rights = rights
                };
                
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var provider = new DpapiDataProtectionProvider("TableForDevelopers");
                    //UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                    // генерируем токен для подтверждения регистрации
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // создаем ссылку для подтверждения
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                               protocol: Request.Url.Scheme);
                    // отправка письма
                    await UserManager.SendEmailAsync(user.Id, "Подтверждение электронной почты",
                               "Для завершения регистрации перейдите по ссылке:: <a href=\""
                                                               + callbackUrl + "\">завершить регистрацию</a>" +
                                                               "<p>Это письмо сгенерировано в учебных целях(!), автоматически и отвечать на него не нужно.</p>");
                    return RedirectToAction("DisplayEmail", "Account");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return PartialView(model);
        }
        public ActionResult DisplayEmail()
        {
            return PartialView();
        }
        public ActionResult ConfirmEmail(string UserId, string code)
        {
            using (var context = ApplicationContext.Create())
            {
                var user = context.Users.Where(i => i.Id == UserId).ToList().FirstOrDefault();
                user.EmailConfirmed = true;
                context.SaveChanges();
            }
            return PartialView();
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Login(string UserId)
        {
            if (UserId != null)
            {
                var user = UserManager.FindById(UserId);
                ClaimsIdentity ident = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);  //  DefaultAuthenticationTypes.ApplicationCookie is used whenn working with individual accounts
                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, ident);
                HttpContext.Cache.Insert("UserType", user.Rights.ToString());
                HttpContext.Cache.Insert("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Поле E-Mail пустое.");
                return PartialView(model);
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Поле Password пустое.");
                return PartialView(model);
            }
            if (ModelState.IsValid)
            {
                PasswordVerificationResult result = PasswordVerificationResult.Failed;
                ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный email или пароль.");
                }
                else
                {
                    result = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);

                    if (!(result == PasswordVerificationResult.Success))
                    {
                        ModelState.AddModelError("", "Неверный email или пароль.");
                    }
                    else
                    {
                        if(user.PhoneNumberConfirmed)
                        {
                            if (await SendSmsVerificationCode(user.Id))
                                return RedirectToAction("ConfirmTwoFactorAuth", "Account", new {UserId = user.Id, ParentAction = "Login"});
                        }
                        ClaimsIdentity ident = await UserManager
                                                         .CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);  //  DefaultAuthenticationTypes.ApplicationCookie is used whenn working with individual accounts
                        AuthenticationManager.SignOut();

                        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, ident);
                        HttpContext.Cache.Insert("UserType", user.Rights.ToString());
                        HttpContext.Cache.Insert("UserId", user.Id);
                        if (returnUrl == null)
                            return RedirectToAction("Index", "Home");
                        return Redirect(returnUrl);
                    }
                }

            }
            ViewBag.returnUrl = returnUrl;
            return PartialView(model);
        }
        public ActionResult ForgotPassword()
        {
            return PartialView();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordConfirmationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }
                //var provider = new DpapiDataProtectionProvider("TableForDevelopers");
                //UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("PasswordRecovery", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                    "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>" +
                    "<p> Это письмо сгенерировано в учебных целях(!), автоматически и отвечать на него не нужно.</p>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return PartialView(model);
        }
        public ActionResult ForgotPasswordConfirmation()
        {
            return PartialView();
        }
        public ActionResult PasswordRecovery()
        {
            return PartialView();
        }
        public async Task<ActionResult> PasswordReset(string UserId)
        {
            CheckAuth();
            var user = await UserManager.FindByIdAsync(UserId);
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("PasswordRecovery", "Account",
                new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Сброс пароля",
                "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>" +
                "<p> Это письмо сгенерировано в учебных целях(!), автоматически и отвечать на него не нужно.</p>");
            AuthenticationManager.SignOut();
            return RedirectToAction("ForgotPasswordConfirmation", "Account");
        }
        public async Task<ActionResult> EnableTwoFactorAuth(string UserId)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            if (await SendSmsVerificationCode(UserId))
            {
                return RedirectToAction("ConfirmTwoFactorAuth", "Account", new { UserId = UserId, ParentAction = "AccountSettings" });
            }
            return PartialView();
        }
        public ActionResult ConfirmTwoFactorAuth(string UserId, string ParentAction)
        {
            ViewBag.UserId = UserId;
            ViewBag.ParentAction = ParentAction;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmTwoFactorAuth(string UserId, string ParentAction, TwoFactorAuthModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(UserId);

            var result = await UserManager.ChangePhoneNumberAsync(UserId, user.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                return RedirectToAction(ParentAction, "Account", new { UserId = user.Id });
            }
            ModelState.AddModelError("", "Failed to verify phone");
            return PartialView();
        }
        public ActionResult DisableTwoFactorAuth(string UserId)
        {
            using (var context = ApplicationContext.Create())
            {
                var user = context.Users.Where(i => i.Id == UserId).First();
                user.PhoneNumberConfirmed = false;
                context.SaveChanges();
            }
            return RedirectToAction("AccountSettings", "Account", new { UserId = UserId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordRecovery(string UserId, PasswordRecoveryModel model)
        {
            if(ModelState.IsValid)
            {
                using (var context = ApplicationContext.Create())
                {
                    var user = context.Users.Where(i => i.Id == UserId).FirstOrDefault();
                    if (user == null)
                        return RedirectToAction("Index", "Home");
                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("PasswordChanged", "Account");
        }
        public ActionResult PasswordChanged()
        {
            return PartialView();
        }
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }
        //TODO
        public ActionResult TwoFactorAuth()
        {
            return PartialView();
        }
        public ActionResult AccountSettings(string UserId)
        {
            CheckAuth();
            var user = UserManager.FindById(UserId);
            ViewBag.IsTwoFactorEnabled = user.PhoneNumberConfirmed;
            return PartialView();
        }
        public async Task<bool> SendSmsVerificationCode(string UserId)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(UserId, user.PhoneNumber);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = user.PhoneNumber,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
                return true;
            }
            return false;
        }
        private ApplicationUserManager UserManager
        {
            get
            {
                var context = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                context.EmailService = new EmailService();
                context.SmsService = new SmsService();
                return context;
            }
        }
        private void CheckAuth()
        {
            ViewBag.IsAuth = HttpContext.User.Identity.IsAuthenticated; // аутентифицирован ли пользователь
            ViewBag.Login = HttpContext.User.Identity.Name; // логин авторизованного пользователя
            ViewBag.UserType = HttpContext.Cache["UserType"];
            ViewBag.UserId = HttpContext.Cache["UserId"];
        }
    }
}