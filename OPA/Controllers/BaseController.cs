// <copyright file="BaseController.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Web.Mvc;
using log4net;
using OPA.BusinessLogic;
using OPA.DataAccess;

namespace OPA.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController()
        {
            Logger = LogManager.GetLogger(GetType());
            Database = new OpaContext();
            PersonHelper = new PersonLogic(Database);
            UserHelper = new UserLogic(Database, PersonHelper);
            PaymentHelper = new PaymentLogic(Database, PersonHelper);
            ContactHelper = new ContactLogic(Database);
            FinancialHelper = new FinancialLogic(Database);
        }

        protected ILog Logger { get; set; }
        protected OpaContext Database { get; set; }
        protected ContactLogic ContactHelper { get; set; }
        protected FinancialLogic FinancialHelper { get; set; }
        protected PaymentLogic PaymentHelper { get; set; }
        protected PersonLogic PersonHelper { get; set; }
        protected UserLogic UserHelper { get; set; }

        protected ActionResult Error(string errorMessage)
        {
            Logger.Error(errorMessage);
            ViewBag.errorMessage = errorMessage;
            return View("Error");
        }

        protected override void Dispose(bool disposing)
        {
            Database.Dispose();
            base.Dispose(disposing);
        }
    }
}