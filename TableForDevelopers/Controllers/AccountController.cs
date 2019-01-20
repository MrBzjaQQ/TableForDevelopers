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
            if (ModelState.IsValid)
            {
                UserType type = UserType.Customer;
                switch (model.UserType)
                {
                    case "Developer": { type = UserType.Developer; break; }
                    case "Customer": { type = UserType.Customer; break; }
                    case "TeamLeader": { type = UserType.TeamLeader; break; }

                }
                ApplicationUser user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    Rights = type
                };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var provider = new DpapiDataProtectionProvider("TableForDevelopers");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
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

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("", "Поле E-Mail пустое.");
            if (string.IsNullOrEmpty(model.Password))
                ModelState.AddModelError("", "Поле Password пустое.");
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

                        ClaimsIdentity ident = await UserManager
                                                         .CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);  //  DefaultAuthenticationTypes.ApplicationCookie is used whenn working with individual accounts

                        AuthenticationManager.SignOut();

                        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, ident);
                        HttpContext.Cache.Insert("UserType", user.Rights.ToString());
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
                var provider = new DpapiDataProtectionProvider("TableForDevelopers");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
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

        private ApplicationUserManager UserManager
        {
            get
            {
                var context = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                context.EmailService = new EmailService();
                return context;
            }
        }
    }
}