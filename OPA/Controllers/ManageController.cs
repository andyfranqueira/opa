// <copyright file="ManageController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OPA.BusinessLogic;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            Error
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

        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : string.Empty;

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                UserName = User.Identity.GetUserName()
            };

            var person = UserHelper.GetCurrentUser().Person;
            if (person != null)
            {
                model.PersonId = person.Id;
                model.PersonName = Utilities.FormatName(person);
            }

            return View(model);
        }

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            AddErrors(result);
            return View(model);
        }

        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }

                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Manage/RequestSupport
        public ActionResult RequestSupport(int? personId, string request)
        {
            if (personId == null || !UserHelper.UserCanEditPerson(User, personId))
            {
                return HttpNotFound();
            }

            var model = new SupportRequestViewModel { PersonId = personId.Value };
            ViewBag.Request = request;
            return PartialView("Request", model);
        }

        // POST: /Manage/RequestSupport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RequestSupport(SupportRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpNotFound();
            }

            var person = Database.People.SingleOrDefault(p => p.Id == model.PersonId);
            if (person == null)
            {
                return HttpNotFound();
            }

            var subject = "Please add family members";
            var emailBody = Utilities.FormatName(person)
                            + " has requested that family members be added to the system. <br/><br/>"
                            + "The family details are: <br/>" + model.Details + "<br/><br/>"
                            + "Person record: " + Url.Action("Edit", "People", new { id = person.Id }, protocol: Request.Url?.Scheme) + "<br/><br/>";

            await Utilities.AsyncSendEmail(UserHelper.GetAdminEmails(), subject, emailBody);
            return RedirectToAction("Edit", "People", new { id = person.Id, success = true });
        }

        // GET: /Manage/ManageUsers
        [Authorize(Roles = "Admin")]
        public ActionResult ManageUsers()
        {
            var users = Database.ApplicationUsers.ToList();
            var model = users
                .Select(u => new UserViewModel(u, UserHelper))
                .OrderBy(u => u.UserName).ToList();

            return View(model);
        }

        // GET: /Manage/EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            var user = Database.ApplicationUsers.Find(id);
            var model = new UserViewModel(user, UserHelper);
            return PartialView(model);
        }

        // POST: /Manage/EditUser/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "Id,UserName,EmailConfirmed,Admin,PersonId")] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }

            if (model.Admin)
            {
                UserManager.AddToRole(model.Id, "Admin");
            }
            else
            {
                UserManager.RemoveFromRole(model.Id, "Admin");
            }

            return RedirectToAction("ManageUsers");
        }

        // POST: /Manage/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string userId)
        {
            var user = UserManager.FindById(userId);
            UserManager.Delete(user);
            return RedirectToAction("ManageUsers");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }
    }
}