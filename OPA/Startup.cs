// <copyright file="Startup.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Configuration;
using log4net.Config;
using Microsoft.Owin;
using OPA.BusinessLogic;
using Owin;

[assembly: OwinStartupAttribute(typeof(OPA.Startup))]
namespace OPA
{
    /// <summary>
    /// This partial class configures additional web application settings on startup
    /// </summary>
    public partial class Startup
    {
        private static readonly string Environment = ConfigurationManager.AppSettings["environment"];

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            if (Environment == "PROD")
            {
                Utilities.EncryptConfigSection("appSettings");
                Utilities.EncryptConfigSection("connectionStrings");
            }

            Utilities.InitializeDatabase();
            XmlConfigurator.Configure();
        }
    }
}
