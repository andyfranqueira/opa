// <copyright file="ContactController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class ContactController : BaseController
    {
        // GET: /Contact/Create
        public ActionResult Create(int? personId, int? organizationId)
        {
            if (!UserCanEdit(personId, organizationId))
            {
                return HttpNotFound();
            }

            ViewBag.PersonId = personId;
            ViewBag.OrganizationId = organizationId;
            ViewBag.ContactTypes = ContactHelper.GetContactTypes();
            return PartialView();
        }

        // POST: /Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PersonId,OrganizationId,Id,ContactType,ContactDetails")] ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!UserCanEdit(model.PersonId, model.OrganizationId))
                {
                    return HttpNotFound();
                }

                var contact = model.MapToContact();
                Database.Contacts.Add(contact);
                Database.SaveChanges();

                return ReturnToSender(model.PersonId, model.OrganizationId);
            }

            ViewBag.ContactTypes = ContactHelper.GetContactTypes();
            return View(model);
        }

        // GET: /Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var contact = Database.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            if (!UserCanEdit(contact.PersonId, contact.OrganizationId))
            {
                return HttpNotFound();
            }

            ViewBag.ContactTypes = ContactHelper.GetContactTypes();
            return PartialView(new ContactViewModel(contact));
        }

        // POST: /Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersonId,OrganizationId,Id,ContactType,ContactDetails")] ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!UserCanEdit(model.PersonId, model.OrganizationId))
                {
                    return HttpNotFound();
                }

                var contact = model.MapToContact();
                Database.Entry(contact).State = EntityState.Modified;
                Database.SaveChanges();

                return ReturnToSender(model.PersonId, model.OrganizationId);
            }

            ViewBag.ContactTypes = ContactHelper.GetContactTypes();
            return PartialView(model);
        }

        // POST: /Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int contactId)
        {
            var contact = Database.Contacts.Find(contactId);
            var personId = contact.PersonId;
            var organizationId = contact.OrganizationId;

            if (!UserCanEdit(contact.PersonId, contact.OrganizationId))
            {
                return HttpNotFound();
            }

            Database.Contacts.Remove(contact);
            Database.SaveChanges();

            return ReturnToSender(personId, organizationId);
        }

        // GET: /Contact/CreateAddress
        public ActionResult CreateAddress(int? personId, int? organizationId)
        {
            if (!UserCanEdit(personId, organizationId))
            {
                return HttpNotFound();
            }

            ViewBag.PersonId = personId;
            ViewBag.OrganizationId = organizationId;

            if (personId != null)
            {
                ViewBag.ExistingAddresses = ContactHelper.GetEligibleAddressList(personId);
            }

            return PartialView();
        }

        // POST: /Contact/CreateAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAddress([Bind(Include = "Id,PersonId,OrganizationId,AddressId,AddressLine,City,State,PostalCode,Country")] ContactAddressViewModel model)
        {
            if (!UserCanEdit(model.PersonId, model.OrganizationId))
            {
                return HttpNotFound();
            }

            if (model.AddressId == null && !ModelState.IsValid)
            {
                ViewBag.ExistingAddresses = ContactHelper.GetEligibleAddressList(model.PersonId);
                return View(model);
            }

            if (model.AddressId == null)
            {
                var address = model.MapToAddress();
                Database.Addresses.Add(address);
                Database.SaveChanges();
                model.AddressId = address.Id;
            }

            var contactAddress = model.MapToContactAddress();
            Database.ContactAddresses.Add(contactAddress);
            Database.SaveChanges();

            return ReturnToSender(model.PersonId, model.OrganizationId);
        }

        // GET: /Contact/EditAddress/5
        public ActionResult EditAddress(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var contactAddress = Database.ContactAddresses.Find(id);
            if (contactAddress == null)
            {
                return HttpNotFound();
            }

            if (!UserCanEdit(contactAddress.PersonId, contactAddress.OrganizationId))
            {
                return HttpNotFound();
            }

            return PartialView(new ContactAddressViewModel(contactAddress));
        }

        // POST: /Contact/EditAddress/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAddress([Bind(Include = "Id,PersonId,OrganizationId,AddressId,AddressLine,City,State,PostalCode,Country")] ContactAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!UserCanEdit(model.PersonId, model.OrganizationId))
                {
                    return HttpNotFound();
                }

                var address = model.MapToAddress();
                Database.Entry(address).State = EntityState.Modified;
                Database.SaveChanges();

                return ReturnToSender(model.PersonId, model.OrganizationId);
            }

            return PartialView(model);
        }

        // POST: /Contact/DeleteAddress/5
        [HttpPost, ActionName("DeleteAddress")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAddressConfirmed(int contactAddressId)
        {
            var contactAddress = Database.ContactAddresses.Find(contactAddressId);
            var address = Database.Addresses.Find(contactAddress.AddressId);
            var personId = contactAddress.PersonId;
            var organizationId = contactAddress.OrganizationId;

            if (!UserCanEdit(contactAddress.PersonId, contactAddress.OrganizationId))
            {
                return HttpNotFound();
            }

            Database.ContactAddresses.Remove(contactAddress);
            Database.SaveChanges();

            if (!Database.ContactAddresses.Any(c => c.AddressId == address.Id))
            {
                Database.Addresses.Remove(address);
                Database.SaveChanges();
            }

            return ReturnToSender(personId, organizationId);
        }

        private bool UserCanEdit(int? personId, int? organizationId)
        {
            return !(personId != null && organizationId != null)
                   && ((personId != null && UserHelper.UserCanEditPerson(User, personId))
                       || (organizationId != null && User.IsInRole("Admin")));
        }

        private RedirectResult ReturnToSender(int? personId, int? organizationId)
        {
            return personId != null ?
                Redirect(Url.RouteUrl(new { controller = "People", action = "Edit", id = personId }) + "#contacts") :
                Redirect(Url.RouteUrl(new { controller = "Organization", action = "Edit", id = organizationId }) + "#contacts");
        }
    }
}
