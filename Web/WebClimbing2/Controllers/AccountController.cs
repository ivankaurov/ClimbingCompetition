using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebClimbing2.Models.Account;

namespace WebClimbing2.Controllers
{
    [Authorize]
    public class AccountController : __BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? string.Empty;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dbUserByLogin = this.Context.Users.FirstOrDefault(u => u.UserName == model.UserName);

            if (dbUserByLogin == null && model.UserName.Contains("@"))
            {
                if (this.Context.Users.Count(u => u.Email == model.UserName && !u.LogicallyDeleted) > 1)
                {
                    ModelState.AddModelError(string.Empty, "Вход в систему по данному E-Mail невозможен. К адресу привязано более одного активного пользователя.");
                    return View(model);
                }
            }

            if(!WebMatrix.WebData.WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                ModelState.AddModelError(String.Empty, "Неправильное имя пользователя или пароль");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebMatrix.WebData.WebSecurity.Logout();
            return this.RedirectToAction("Index", "Home");
        }
    }
}