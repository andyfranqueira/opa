// <copyright file="PeopleController.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            var people = Database.People.Where(p => !p.LastName.Contains("Anonymous")).ToList();

            if (UserHelper.IsOwnerAdmin())
            {
                var anonymous = Database.People.SingleOrDefault(p => p.LastName.Contains("Anonymous"));
                if (anonymous != null)
                {
                    people.Add(anonymous);
                }
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
            var model = new PersonViewModel { ParentId = parentId, ForCouple = PersonHelper.IsMarried(parentId) };
            return View(model);
        }

        // POST: /People/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LastName,FirstName,MiddleName,Sex,DateOfBirth,Orthodox,Active,ParentId,ForCouple")] PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var person = model.MapToPerson();

                if (model.ParentId != null)
                {
                    var parent = Database.People.Find(model.ParentId);

                    if (model.ForCouple)
                    {
                        var couple = PersonHelper.GetCouple(parent);
                        person.FatherId = couple.HusbandId;
                        person.MotherId = couple.WifeId;
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

            return View(model);
        }

        // GET: /People/Edit/5
        public ActionResult Edit(int? id, bool? success)
        {
            if (id == null || !UserHelper.UserCanEditPerson(User, id))
            {
                return HttpNotFound();
            }

            var person = Database.People.SingleOrDefault(p => p.Id == id);
            if (person == null)
            {
                return HttpNotFound();
            }

            ViewBag.Success = success;
            var model = new PersonViewModel(person)
            {
                ProfilePhoto = PersonHelper.GetProfilePhoto(person.Id),
                Parents = PersonHelper.GetParents(person).Select(p => new PersonViewModel(p)).ToList(),
                Children = PersonHelper.GetChildren(person).Select(p => new PersonViewModel(p)).ToList(),
                Addresses = person.ContactAddresses.Select(c => new ContactAddressViewModel(c)).ToList(),
                Contacts = person.Contacts.Select(c => new ContactViewModel(c)).ToList()
            };

            var couple = PersonHelper.GetCouple(person);
            if (couple != null)
            {
                var spouse = couple.HusbandId == person.Id ? couple.Wife : couple.Husband;
                model.Spouse = new PersonViewModel(spouse);
            }

            model.Pledges = FinancialHelper.GetPledges(person.Id, model.Spouse?.Id).Select(p => new PledgeViewModel(p)).ToList();
            model.Donations = FinancialHelper.GetDonations(person.Id, model.Spouse?.Id).Select(d => new DonationViewModel(d)).ToList();

            return View(model);
        }

        // POST: /People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LastName,FirstName,MiddleName,Sex,DateOfBirth,Orthodox,Active")] PersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var person = model.MapToPerson();
                Database.Entry(person).State = EntityState.Modified;
                Database.SaveChanges();
                return RedirectToAction("Edit", new { id = person.Id, success = true });
            }

            return View(model);
        }

        // GET: /People/ProfilePhoto/5
        [HttpGet]
        public ActionResult ProfilePhoto(int? id)
        {
            if (id == null || !UserHelper.UserCanEditPerson(User, id))
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
        public ActionResult SaveProfilePhoto(int? id, string x, string y, string width, string height, string imageSrc)
        {
            if (id == null || !UserHelper.UserCanEditPerson(User, id))
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

                var profilePhoto = PersonHelper.GetProfilePhoto((int)id, false);
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
