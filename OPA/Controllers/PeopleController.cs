// <copyright file="PeopleController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPA.BusinessLogic;
using OPA.Entities;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class PeopleController : BaseController
    {
        // GET: /People
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                var person = UserHelper.GetCurrentUser().Person;
                return person != null ?
                    RedirectToAction("Edit", new { id = person.Id }) :
                    RedirectToAction("Index", "Manage");
            }

            var people = Database.People.ToList();
            if (!UserHelper.IsOwnerAdmin())
            {
                people.RemoveAll(p => p.LastName.Contains("Anonymous"));
            }

            var peopleViewModel = people
                .Select(p => new PersonViewModel(p))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.DateOfBirth);

            return View(peopleViewModel);
        }

        // GET: /People/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? parentId)
        {
            var model = new PersonViewModel
            {
                ParentId = parentId,
                ForCouple = parentId != null && PersonHelper.IsMarried(parentId.Value)
            };

            ViewBag.MemberTypeList = PersonHelper.GetMemberTypeList();
            return View(model);
        }

        // POST: /People/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LastName,FirstName,MiddleName,Sex,DateOfBirth,MemberType,Active,ParentId,ForCouple")] PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var person = model.MapToPerson();

                if (model.ParentId != null)
                {
                    var parent = Database.People.Find(model.ParentId);
                    if (model.ForCouple)
                    {
                        var spouse = PersonHelper.GetSpouse(parent.Id);
                        person.FatherId = parent.Sex == Sex.Male ? parent.Id : spouse.Id;
                        person.MotherId = parent.Sex == Sex.Female ? parent.Id : spouse.Id;
                    }
                    else if (parent.Sex == Sex.Male)
                    {
                        person.FatherId = parent.Id;
                    }
                    else if (parent.Sex == Sex.Female)
                    {
                        person.MotherId = parent.Id;
                    }
                }

                Database.People.Add(person);
                Database.SaveChanges();

                return RedirectToAction("Edit", new { id = person.Id });
            }

            ViewBag.MemberTypeList = PersonHelper.GetMemberTypeList();
            return View(model);
        }

        // GET: /People/Edit/5
        public ActionResult Edit(int? id, bool? success)
        {
            var person = Database.People.SingleOrDefault(p => p.Id == id);
            if (!UserHelper.UserCanEditPerson(User, id) || person == null)
            {
                return HttpNotFound();
            }

            ViewBag.Success = success;
            var model = new PersonViewModel(person)
            {
                Addresses = person.ContactAddresses.Select(c => new ContactAddressViewModel(c)).ToList(),
                Contacts = person.Contacts.Select(c => new ContactViewModel(c)).ToList(),
                ProfilePhoto = PersonHelper.GetProfilePhoto(person.Id)
            };

            var spouse = PersonHelper.GetSpouse(person.Id);
            if (spouse != null && UserHelper.UserCanEditPerson(User, spouse.Id))
            {
                model.Spouse = new PersonViewModel(spouse);
            }

            var parents = PersonHelper.GetParents(person).Where(p => UserHelper.UserCanEditPerson(User, p.Id));
            var children = PersonHelper.GetChildren(person.Id, spouse?.Id).Where(p => UserHelper.UserCanEditPerson(User, p.Id));

            model.Parents = parents.Select(p => new PersonViewModel(p)).ToList();
            model.Children = children.Select(p => new PersonViewModel(p)).ToList();

            model.Pledges = FinancialHelper.GetPledges(person.Id, model.Spouse?.Id).Select(p => new PledgeViewModel(p)).ToList();
            model.Donations = FinancialHelper.GetDonations(person.Id, model.Spouse?.Id).Select(d => new DonationViewModel(d)).ToList();

            ViewBag.MemberTypeList = PersonHelper.GetMemberTypeList();
            ViewBag.DonationForm = PaymentHelper.GetDonationForm(UserHelper.GetCurrentUser(), person);
            ViewBag.DonationUserAcct = PaymentHelper.GetDonationUserAcctUrl(UserHelper.GetCurrentUser(), person);

            return View(model);
        }

        // POST: /People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LastName,FirstName,MiddleName,Sex,DateOfBirth,MemberType,Active")] PersonViewModel model)
        {
            var person = Database.People.Find(model.Id);

            if (ModelState.IsValid)
            {
                model.UpdatePerson(person);
                Database.SaveChanges();
                return RedirectToAction("Edit", new { id = person.Id, success = true });
            }

            ViewBag.MemberTypeList = PersonHelper.GetMemberTypeList();
            ViewBag.DonationForm = PaymentHelper.GetDonationForm(UserHelper.GetCurrentUser(), person);
            ViewBag.DonationUserAcct = PaymentHelper.GetDonationUserAcctUrl(UserHelper.GetCurrentUser(), person);

            return View(model);
        }

        // GET: /People/AddSpouse
        [Authorize(Roles = "Admin")]
        public ActionResult AddSpouse(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var person = Database.People.SingleOrDefault(p => p.Id == id);
            if (person == null || PersonHelper.IsMarried((int)id))
            {
                return HttpNotFound();
            }

            var model = new CoupleViewModel
            {
                Person1Id = person.Id
            };

            ViewBag.PersonId = person.Id;
            ViewBag.EligibleMates = PersonHelper.GetEligibleMates(person.Sex);
            return PartialView(model);
        }

        // POST: /People/AddSpouse
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddSpouse([Bind(Include = "Id,Person1Id,Person2Id")] CoupleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var couple = model.MapToCouple();

                var existingCouple = Database.Couples.SingleOrDefault(c => c.Person1Id == couple.Person1Id && c.Person2Id == couple.Person2Id);
                if (existingCouple == null)
                {
                    Database.Couples.Add(couple);
                }

                Database.SaveChanges();
                return RedirectToAction("Edit", new { id = model.Person1Id, success = true });
            }

            return RedirectToAction("AddSpouse");
        }

        // POST: /People/RemoveSpouse
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveSpouse(int personId)
        {
            var couple = Database.Couples.SingleOrDefault(c => c.Person1Id == personId || c.Person2Id == personId);
            Database.Couples.Remove(couple);
            Database.SaveChanges();
            return RedirectToAction("Edit", new { id = personId, success = true });
        }

        // GET: /People/ProfilePhoto/5
        [HttpGet]
        public ActionResult ProfilePhoto(int? id)
        {
            if (!UserHelper.UserCanEditPerson(User, id))
            {
                return HttpNotFound();
            }

            ViewBag.PersonId = id;
            return View();
        }

        // GET: /People/_ProfilePhoto
        [HttpGet]
        public ActionResult _ProfilePhoto()
        {
            return PartialView();
        }

        // POST: /People/_ProfilePhoto/files
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ProfilePhoto(IEnumerable<HttpPostedFileBase> files)
        {
            if (files == null)
            {
                return Json(new { success = false, errorMessage = "No file uploaded." });
            }

            var file = files.FirstOrDefault();
            if (file == null || !Utilities.IsImageFile(file) || file.ContentLength <= 0)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format or empty." });
            }

            // Get the image file, resize and convert to Base 64
            var image = Image.FromStream(file.InputStream);
            image = Utilities.ScaleImage(image, 401);
            image = Utilities.CropImage(image, 1, 1, image.Width - 1, image.Height - 1);
            var base64String = Utilities.ConvertImageToBase64(image, ImageFormat.Jpeg);
            return Json(new { success = true, imageSrc = base64String });
        }

        // POST: /People/SaveProfilePhoto
        [HttpPost]
        public ActionResult SaveProfilePhoto(int id, string x, string y, string width, string height, string imageSrc)
        {
            if (!UserHelper.UserCanEditPerson(User, id))
            {
                return HttpNotFound();
            }

            try
            {
                var image = Utilities.ConvertBase64ToImage(imageSrc);
                image = Utilities.CropImage(
                    image,
                    Convert.ToInt32(x),
                    Convert.ToInt32(y),
                    Convert.ToInt32(width),
                    Convert.ToInt32(height));

                image = Utilities.ScaleImage(image, 200, 200);

                var profilePhoto = PersonHelper.GetProfilePhoto(id, false);
                image.Save(HttpContext.Server.MapPath(profilePhoto));
                return Json(new { success = true, imageSrc = profilePhoto });
            }
            catch (Exception)
            {
                return Json(new { success = false, errorMessage = "Unable to upload file." });
            }
        }
    }
}
