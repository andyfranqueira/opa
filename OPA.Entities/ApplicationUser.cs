// <copyright file="ApplicationUser.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OPA.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLogin { get; set; }
        public int? LoginCount { get; set; }
        public int? PersonId { get; set; }

        public virtual Person Person { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}