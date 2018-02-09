// <copyright file="PaymentController.cs" company="The OPA Project">
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
    public class PaymentController : BaseController
    {
        // GET: /Payment
        public ActionResult Index()
        {
            var payments = Database.Payments
                .OrderByDescending(p => p.PostedDate)
                .ThenByDescending(p => p.TransactionDate)
                .ToList();

            var model = payments.Select(p => new PaymentViewModel(p));
            return View(model);
        }

        // GET: /Payment/Check
        public ActionResult Check()
        {
            Logger.Info("Loading New Payments");
            PaymentHelper.LoadNewPayments();

            Logger.Info("Processing Payments");
            PaymentHelper.ProcessPayments();

            Logger.Info("Complete");
            return RedirectToAction("Index");
        }

        // GET: /Payment/CreateDonation/5
        public ActionResult CreateDonation(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var payment = Database.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }

            var model = new PaymentDonationViewModel(payment);
            ViewBag.Donors = FinancialHelper.GetDonorList();
            ViewBag.FundList = FinancialHelper.GetFundList();

            return PartialView(model);
        }

        // POST: /Payment/CreateDonation
        [HttpPost, ActionName("CreateDonation")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDonation([Bind(Include = "PaymentId,PaymentInfo,PersonId,Fund")] PaymentDonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var payment = Database.Payments.Find(model.PaymentId);
            var personId = Database.People.Find(model.PersonId).Id;

            PaymentHelper.RecordPaymentAsDonation(payment, personId, model.Fund);

            return RedirectToAction("Index");
        }
    }
}
