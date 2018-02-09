// <copyright file="CoupleController.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OPA.Entities;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoupleController : BaseController
    {
        // GET: /Couple
        public ActionResult Index()
        {
            var model = new List<CoupleViewModel>();
            foreach (var couple in Database.Couples.ToList())
            {
                model.Add(new CoupleViewModel(couple));
            }

            return View(model.OrderBy(c => c.Husband.LastName).ThenBy(c => c.Husband.DateOfBirth));
        }

        // GET: /Couple/Create
        public ActionResult Create(int? husbandId, int? wifeId)
        {
            ViewBag.HusbandId = husbandId;
            ViewBag.WifeId = wifeId;
            ViewBag.EligibleMales = PersonHelper.GetEligibleSingles(Sex.Male);
            ViewBag.EligibleFemales = PersonHelper.GetEligibleSingles(Sex.Female);

            return View();
        }

        // POST: /Couple/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HusbandId,WifeId,Active")] CoupleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var couple = model.MapToCouple();

                var existingCouple = Database.Couples.SingleOrDefault(c => c.HusbandId == couple.HusbandId && c.WifeId == couple.WifeId && !c.Active);
                if (existingCouple != null)
                {
                    existingCouple.Active = couple.Active;
                    Database.Entry(existingCouple).State = EntityState.Modified;
                }
                else
                {
                    Database.Couples.Add(couple);
                }

                Database.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EligibleMales = PersonHelper.GetEligibleSingles(Sex.Male);
            ViewBag.EligibleFemales = PersonHelper.GetEligibleSingles(Sex.Female);
            return View(model);
        }

        // GET: /Couple/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            var couple = Database.Couples.SingleOrDefault(p => p.Id == id);
            if (couple == null)
            {
                return HttpNotFound();
            }

            var model = new CoupleViewModel(couple);
            return View(model);
        }

        // POST: /Couple/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HusbandId,WifeId,Active")] CoupleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var couple = model.MapToCouple();
                Database.Entry(couple).State = EntityState.Modified;
                Database.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
