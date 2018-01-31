// <copyright file="Global.asax.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OPA
{
    /// <summary>
    /// This class configures basic web application settings
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection)
            {
                // This is an insecure connection so redirect to the secure version
                var uri = new UriBuilder(Context.Request.Url);
                if (!uri.Host.Equals("localhost"))
                {
                    uri.Port = 443;
                    uri.Scheme = "https";
                    Response.Redirect(uri.ToString());
                }
            }
        }
    }
}
