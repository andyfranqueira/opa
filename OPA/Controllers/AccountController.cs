// <copyright file="AccountController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OPA.BusinessLogic;
using OPA.Entities;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var user = await UserManager.FindByNameAsync(model.Email.ToLower());
            if (!ModelState.IsValid || user == null)
            {
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            {
                await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");
                return Error("You must have a confirmed email to log on. " + "The confirmation token has been resent to your email account.");
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email.ToLower(), model.Password, model.RememberMe, true);
            Logger.Info("Log in " + result + ": " + model.Email);

            switch (result)
            {
                case SignInStatus.Success:
                    if (user.PersonId == null)
                    {
                        user.PersonId = UserHelper.FindUserPerson(user);
                    }

                    user.LastLogin = DateTime.Now;
                    user.LoginCount = (user.LoginCount ?? 0) + 1;
                    await UserManager.UpdateAsync(user);

                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                default:
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email.ToLower(), Email = model.Email.ToLower() };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send a confirmation email to validate the user account
                    await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");
                    ViewBag.DonateEmail = model.Email.ToLower();
                    ViewBag.Message = "<p>A confirmation email has been sent to you at <strong><em>" + model.Email.ToLower() + "</em></strong> to confirm your account. "
                                      + "Your account must be confirmed before you can log in.</p>";

                    // Look for a matching person record and link to the user account if one is found
                    user.PersonId = UserHelper.FindUserPerson(user);
                    await UserManager.UpdateAsync(user);

                    // Inform the admins of the new user account
                    var emailBody = "A new OPA account has been created for: " + model.Email.ToLower() + "<br/>"
                                    + "Name: " + model.FirstName + " " + model.LastName + "<br/><br/>";

                    if (user.PersonId != null)
                    {
                        var person = Database.People.Find(user.PersonId);
                        emailBody = emailBody
                                    + "A matching person record was found and assigned.<br/>"
                                    + "Person: " + Utilities.FormatName(person);
                    }
                    else
                    {
                        emailBody = emailBody
                                    + "No matching person record was found.<br/><br/>"
                                    + Url.Action("Create", "People", null, protocol: Request.Url?.Scheme);
                    }

                    await Utilities.AsyncSendEmail(UserHelper.GetAdminEmails(), "New user account", emailBody);
                    return View("Info");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email.ToLower());
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var subject = "Reset your password";
                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url?.Scheme);
                var message = "Please reset your password by clicking the following link:<br/>" + callbackUrl + "<br/><br/>";

                await UserManager.SendEmailAsync(user.Id, subject, message);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByNameAsync(model.Email.ToLower());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userId, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url?.Scheme);
            var message = "Thank you for registering for an account with " + Utilities.AppName + ".<br/>"
                          + "Please confirm your email account by clicking the following link:<br/>"
                          + callbackUrl + "<br/><br/>"
                          + "Your account must be confirmed before you can log in.";

            await UserManager.SendEmailAsync(userId, subject, message);
            return callbackUrl;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}