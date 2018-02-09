// <copyright file="BundleConfig.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Web.Optimization;

namespace OPA
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-datepicker.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.bootstrap4.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables-buttons").Include(
                        "~/Scripts/DataTables/dataTables.buttons.min.js",
                        "~/Scripts/DataTables/buttons.html5.min.js",
                        "~/Scripts/DataTables/buttons.flash.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/Scripts/jquery.Jcrop.js",
                      "~/Scripts/jquery.form.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/DataTables/css/dataTables.bootstrap4.min.css",
                      "~/Content/opa-theme.css"));

            bundles.Add(new StyleBundle("~/Content/jcrop").Include(
                      "~/Content/jquery.Jcrop.css"));
        }
    }
}
