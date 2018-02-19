// <copyright file="FinancialController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using OPA.BusinessLogic;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class FinancialController : BaseController
    {
        // GET: /Financial/Pledges
        [Authorize(Roles = "Admin")]
        public ActionResult Pledges()
        {
            var pledges = Database.Pledges
                .OrderByDescending(p => p.Year)
                .ThenBy(p => p.Fund)
                .ToList();

            var model = pledges.Select(p => new PledgeViewModel(p));
            return View(model);
        }

        // GET: /Financial/CreatePledge
        public ActionResult CreatePledge(int? personId)
        {
            if (personId == null || !UserHelper.UserCanEditPerson(User, personId))
            {
                return HttpNotFound();
            }

            var model = new PledgeViewModel
            {
                PersonId = (int)personId,
                Year = DateTime.Now.Year
            };

            ViewBag.YearList = FinancialHelper.GetPledgeYearList();
            ViewBag.FrequencyList = FinancialHelper.GetFrequencyList();
            ViewBag.FundList = FinancialHelper.GetFundList();
            return PartialView(model);
        }

        // POST: /Financial/CreatePledge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePledge([Bind(Include = "Id,Year,Amount,Frequency,Fund,PersonId")] PledgeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pledge = model.MapToPledge();
                Database.Pledges.Add(pledge);
                Database.SaveChanges();
                return ReturnToSender(model.PersonId);
            }

            ViewBag.YearList = FinancialHelper.GetPledgeYearList();
            ViewBag.FrequencyList = FinancialHelper.GetFrequencyList();
            ViewBag.FundList = FinancialHelper.GetFundList();
            return View(model);
        }

        // GET: /Financial/EditPledge/5
        public ActionResult EditPledge(int? id, int? personId)
        {
            if (id == null || personId == null)
            {
                return HttpNotFound();
            }

            var pledge = Database.Pledges.Find(id);
            if (pledge == null || !UserHelper.UserCanEditPerson(User, pledge.PersonId))
            {
                return HttpNotFound();
            }

            var model = new PledgeViewModel(pledge);
            ViewBag.YearList = FinancialHelper.GetPledgeYearList();
            ViewBag.FrequencyList = FinancialHelper.GetFrequencyList();
            ViewBag.FundList = FinancialHelper.GetFundList();
            return PartialView(model);
        }

        // POST: /Financial/EditPledge
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPledge([Bind(Include = "Id,Year,Amount,Frequency,Fund,PersonId")] PledgeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pledge = model.MapToPledge();
                Database.Entry(pledge).State = EntityState.Modified;
                Database.SaveChanges();
            }

            return ReturnToSender(model.PersonId);
        }

        // POST: /Financial/DeletePledge/5
        [HttpPost, ActionName("DeletePledge")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePledge(int pledgeId, int? personId)
        {
            var pledge = Database.Pledges.Find(pledgeId);
            Database.Pledges.Remove(pledge);
            Database.SaveChanges();
            return ReturnToSender(personId);
        }

        // GET: /Financial/Pledges
        [Authorize(Roles = "Admin")]
        public ActionResult Donations()
        {
            var donations = Database.Donations
                .OrderByDescending(d => d.DonationDate)
                .ThenBy(d => d.Fund)
                .ToList();

            var model = donations.Select(d => new DonationViewModel(d));
            return View(model);
        }

        // GET: /Financial/CreateDonation/5
        [Authorize(Roles = "Admin")]
        public ActionResult CreateDonation(int? personId, int? organizationId)
        {
            var model = new DonationViewModel
            {
                PersonId = personId,
                OrganizationId = organizationId
            };

            ViewBag.FundList = FinancialHelper.GetFundList();
            return PartialView(model);
        }

        // POST: /Donation/CreateDonation
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDonation([Bind(Include = "Id,DonationDate,Amount,Fund,Designation,CheckNumber,PersonId,OrganizationId")] DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var donation = model.MapToDonation();
                Database.Donations.Add(donation);
                Database.SaveChanges();
                return ReturnToSender(model.PersonId, model.OrganizationId);
            }

            ViewBag.FundList = FinancialHelper.GetFundList();
            return View(model);
        }

        // GET: /Financial/BatchDonations
        [Authorize(Roles = "Admin")]
        public ActionResult BatchDonations(int? count)
        {
            var model = new BatchDonationsViewModel(count ?? 5)
            {
                DonorList = FinancialHelper.GetDonorList(),
                FundList = FinancialHelper.GetFundList()
            };

            return View(model);
        }

        // POST: /Financial/BatchDonations
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult BatchDonations(BatchDonationsViewModel model)
        {
            model.Donations.Add(new DonationViewModel());
            model.DonorList = FinancialHelper.GetDonorList();
            model.FundList = FinancialHelper.GetFundList();
            return View(model);
        }

        // POST: /Financial/CreateBatchDonations
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBatchDonations(BatchDonationsViewModel model)
        {
            foreach (var modelDonation in model.Donations)
            {
                if (modelDonation.PersonId != null)
                {
                    var donation = modelDonation.MapToDonation();
                    Database.Donations.Add(donation);
                    Database.SaveChanges();
                }
            }

            return RedirectToAction("Donations", "Financial");
        }

        // GET: /Donation/EditDonation/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditDonation(int? id, int? personId)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var donation = Database.Donations.Find(id);
            if (donation == null)
            {
                return HttpNotFound();
            }

            var model = new DonationViewModel(donation);
            ViewBag.FundList = FinancialHelper.GetFundList();
            return PartialView(model);
        }

        // POST: /Financial/EditDonation
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditDonation([Bind(Include = "Id,DonationDate,Amount,Fund,Designation,CheckNumber,PersonId,OrganizationId")] DonationViewModel model)
        {
            if (!ModelState.IsValid || model.DonationDate == null || model.Amount == null)
            {
                ViewBag.FundList = FinancialHelper.GetFundList();
                return PartialView(model);
            }

            if (ModelState.IsValid)
            {
                var donation = model.MapToDonation();
                Database.Entry(donation).State = EntityState.Modified;
                Database.SaveChanges();
            }

            return ReturnToSender(model.PersonId, model.OrganizationId);
        }

        // POST: /Financial/DeleteDonation/5
        [HttpPost, ActionName("DeleteDonation")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDonation(int donationId, int? personId)
        {
            var donation = Database.Donations.Find(donationId);
            var organizationId = donation.OrganizationId;
            Database.Donations.Remove(donation);
            Database.SaveChanges();

            return ReturnToSender(personId, organizationId);
        }

        // GET: /Donation/DonationReceipt/5
        [Authorize(Roles = "Admin")]
        public FileResult DonationReceipt(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var donation = Database.Donations.Find(id);
            if (donation == null)
            {
                return null;
            }

            var template = Server.MapPath(FinancialLogic.ReceiptTemplate);

            var spouse = PersonHelper.GetSpouse(donation.Person.Id);
            var fieldValues = FinancialHelper.ReceiptData(donation, spouse);
            var output = Utilities.GenerateWordDocument(template, fieldValues);
            return File(output, MediaTypeNames.Application.Octet, "Receipt-" + id + ".docx");
        }

        private RedirectResult ReturnToSender(int? id)
        {
            return Redirect(Url.RouteUrl(new { controller = "People", action = "Edit", id = id }) + "#pledges");
        }

        private RedirectResult ReturnToSender(int? personId, int? organizationId)
        {
            return personId != null ?
                Redirect(Url.RouteUrl(new { controller = "People", action = "Edit", id = personId }) + "#donations") :
                Redirect(Url.RouteUrl(new { controller = "Organization", action = "Edit", id = organizationId }) + "#donations");
        }
    }
}
