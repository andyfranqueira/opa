// <copyright file="OpaContext.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using OPA.Entities;

namespace OPA.DataAccess
{
    public class OpaContext : IdentityDbContext
    {
        public OpaContext() : base("OPAConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OpaContext, Migrations.Configuration>());
        }

        public IDbSet<Value> ValueSets { get; set; }
        public IDbSet<ApplicationUser> ApplicationUsers { get; set; }
        public IDbSet<Organization> Organizations { get; set; }
        public IDbSet<Person> People { get; set; }
        public IDbSet<Couple> Couples { get; set; }
        public IDbSet<Contact> Contacts { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<ContactAddress> ContactAddresses { get; set; }
        public IDbSet<Donation> Donations { get; set; }
        public IDbSet<Pledge> Pledges { get; set; }
        public IDbSet<Payment> Payments { get; set; }

        public static OpaContext Create()
        {
            return new OpaContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().ToTable("User");
            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
        }
    }
}
