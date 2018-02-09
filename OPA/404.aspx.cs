// <copyright file="404.aspx.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

namespace OPA
{
    using System;

    public partial class C404 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.StatusCode = 404;
        }
    }
}