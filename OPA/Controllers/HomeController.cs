// <copyright file="HomeController.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using log4net.Appender;
using OPA.BusinessLogic;
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

        // GET: /Home/LogFile
        [Authorize(Roles = "Admin")]
        public FileResult LogFile()
        {
            var logfile = Logger.Logger.Repository.GetAppenders().OfType<RollingFileAppender>().FirstOrDefault()?.File;
            return logfile == null ? null : File(Utilities.GetFile(logfile), "text/plain", "Opa.log");
        }

        // GET: /Home/ValueSets
        [Authorize(Roles = "Admin")]
        public ActionResult ValueSets()
        {
            var values = Database.ValueSets.OrderBy(v => v.Set).ThenBy(v => v.Order).ToList();
            var model = values.Select(v => new ValueSetViewModel(v)).ToList();
            model.Add(new ValueSetViewModel());

            return View(model);
        }

        // POST: /Home/ValueSets
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ValueSets(List<ValueSetViewModel> model)
        {
            foreach (var entry in model)
            {
                if (!string.IsNullOrWhiteSpace(entry.Option))
                {
                    var value = entry.MapToValueSet();

                    if (entry.Id == 0)
                    {
                        Database.ValueSets.Add(value);
                        Database.SaveChanges();
                        entry.Id = value.Id;
                    }
                    else
                    {
                        Database.Entry(value).State = EntityState.Modified;
                        Database.SaveChanges();
                    }
                }
            }

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