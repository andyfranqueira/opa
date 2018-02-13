// <copyright file="HomeController.cs" company="The OPA Project">
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
using System.Linq;
using System.Web.Mvc;
using log4net.Appender;
using OPA.BusinessLogic;
using OPA.Entities;
using OPA.Models;

namespace OPA.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        // GET: /Home
        public ActionResult Index()
        {
            Logger.Info(User.IsInRole("Admin") ? "Admin" : "User");

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "People");
            }

            ViewBag.IsOwnerAdmin = UserHelper.IsOwnerAdmin();
            return View();
        }

        // GET: /Home/ValueSets
        [Authorize(Roles = "Admin")]
        public ActionResult ValueSets()
        {
            var model = new List<ValueSetViewModel>();

            foreach (ValueSet valueSet in Enum.GetValues(typeof(ValueSet)))
            {
                var values = Database.ValueSets
                    .Where(v => v.Set == valueSet)
                    .OrderBy(v => v.Order)
                    .ToList();

                model.Add(new ValueSetViewModel
                {
                    Set = valueSet,
                    Values = values.Select(v => new ValueViewModel(v)).ToList()
                });
            }

            return View(model);
        }

        public void UpdateRow(int id, int newPos)
        {
            Database.ValueSets.Find(id).Order = newPos;
            Database.SaveChanges();
        }

        // GET: /Home/CreateValue
        [Authorize(Roles = "Admin")]
        public ActionResult CreateValue(ValueSet set)
        {
            var model = new ValueViewModel { Set = set };
            return PartialView(model);
        }

        // POST: /Home/CreateValue
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateValue([Bind(Include = "Id,Set,Option,Order")] ValueViewModel model)
        {
            if (ModelState.IsValid)
            {
                var values = Database.ValueSets.Where(v => v.Set == model.Set);
                model.Order = values.Any() ? values.Max(v => v.Order) + 1 : 1;
                Database.ValueSets.Add(model.MapToValue());
                Database.SaveChanges();
            }

            return RedirectToAction("ValueSets", "Home");
        }

        // POST: /Home/DeleteValue/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteValue(int id)
        {
            var value = Database.ValueSets.Find(id);
            Database.ValueSets.Remove(value);
            Database.SaveChanges();
            return RedirectToAction("ValueSets", "Home");
        }

        // GET: /Home/GetRawData
        [Authorize(Roles = "Admin")]
        public ActionResult GetRawData()
        {
            if (!UserHelper.IsOwnerAdmin())
            {
                return HttpNotFound();
            }

            var tables = new Dictionary<string, List<List<string>>>
            {
                { "Organizations", MapObjectList(Database.Organizations.ToList()) },
                { "People", MapObjectList(Database.People.ToList()) },
                { "Couples", MapObjectList(Database.Couples.ToList()) },
                { "Contacts", MapObjectList(Database.Contacts.ToList()) },
                { "Addresses", MapObjectList(Database.Addresses.ToList()) },
                { "ContactAddresses", MapObjectList(Database.ContactAddresses.ToList()) },
                { "Pledges", MapObjectList(Database.Pledges.ToList()) },
                { "Donations", MapObjectList(Database.Donations.ToList()) },
                { "Payments", MapObjectList(Database.Payments.ToList()) }
            };

            ViewBag.Tables = tables;
            return View();
        }

        // GET: /Home/LogFile
        [Authorize(Roles = "Admin")]
        public FileResult LogFile()
        {
            var logfile = Logger.Logger.Repository.GetAppenders().OfType<RollingFileAppender>().FirstOrDefault()?.File;
            return logfile == null ? null : File(Utilities.GetFile(logfile), "text/plain", "Opa.log");
        }

        private static List<List<string>> MapObjectList<T>(IEnumerable<T> objectList)
        {
            var rows = new List<List<string>>();

            foreach (var obj in objectList)
            {
                var headers = new List<string>();
                var cells = new List<string>();

                foreach (var property in obj.GetType().GetProperties())
                {
                    if (!rows.Any())
                    {
                        headers.Add(property.Name);
                    }

                    var value = property.GetValue(obj);
                    value = value ?? string.Empty;

                    cells.Add(value.ToString());
                }

                if (!rows.Any())
                {
                    rows.Add(headers);
                }

                rows.Add(cells);
            }

            return rows;
        }
    }
}