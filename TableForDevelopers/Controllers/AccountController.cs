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

namespace TableForDevelopers.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
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
                switch(model.UserType)
                {
                    case "Developer": { type = UserType.Developer; break; }
                    case "Customer": { type = UserType.Customer; break; }
                    case "TeamLeader": { type = UserType.TeamLeader; break; }

                }
                ApplicationUser user = new ApplicationUser { UserName = model.Name,
                    Email = model.Email, Rights = type};
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
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
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult TwoFactorAuth()
        {
            return PartialView();
        }
    }
}