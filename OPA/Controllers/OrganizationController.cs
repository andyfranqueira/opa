// <copyright file="OrganizationController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Linq;
using System.Web.Mvc;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrganizationController : BaseController
    {
        // GET: /Organization
        public ActionResult Index()
        {
            var organizations = Database.Organizations.ToList();
            var model = organizations.Select(o => new OrganizationViewModel(o)).ToList();
            return View(model.OrderBy(o => o.Name));
        }

        // GET: /Organization/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Active")] OrganizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organization = model.MapToOrganization();
                Database.Organizations.Add(organization);
                Database.SaveChanges();
                return RedirectToAction("Edit", new { id = organization.Id });
            }

            return View(model);
        }

        // GET: /Organization/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var organization = Database.Organizations.FirstOrDefault(p => p.Id == id);
            if (organization == null)
            {
                return HttpNotFound();
            }

            var model = new OrganizationViewModel(organization)
            {
                Donations = FinancialHelper.GetOrgDonations(organization.Id).Select(d => new DonationViewModel(d)).ToList(),
                Addresses = organization.ContactAddresses.Select(c => new ContactAddressViewModel(c)).ToList(),
                Contacts = organization.Contacts.Select(c => new ContactViewModel(c)).ToList()
            };

            return View(model);
        }

        // POST: /Organization/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Active")] OrganizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var organization = Database.Organizations.Find(model.Id);
                model.UpdateOrganization(organization);
                Database.SaveChanges();
                return RedirectToAction("Edit", new { id = organization.Id });
            }

            return View(model);
        }
    }
}
