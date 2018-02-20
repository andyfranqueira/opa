// <copyright file="UserLogic.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OPA.DataAccess;
using OPA.Entities;

namespace OPA.BusinessLogic
{
    public class UserLogic
    {
        public UserLogic(OpaContext database, PersonLogic personHelper)
        {
            Database = database;
            PersonHelper = personHelper;
        }

        protected OpaContext Database { get; set; }
        protected PersonLogic PersonHelper { get; set; }

        public ApplicationUser GetCurrentUser()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);
        }

        public bool IsAdmin(ApplicationUser user)
        {
            return user.Roles.Any(r => r.RoleId == AdminRoleId());
        }

        public bool IsOwnerAdmin()
        {
            return GetCurrentUser().Email == Utilities.OwnerEmail;
        }

        public string[] GetAdminEmails()
        {
            var users = Database.ApplicationUsers.ToList();
            return users
                .Where(u => u.Roles.Any(r => r.RoleId == AdminRoleId()))
                .Select(u => u.Email)
                .ToArray();
        }

        public bool UserCanEditPerson(IPrincipal user, int? editPersonId)
        {
            if (editPersonId == null)
            {
                return false;
            }

            var userPerson = GetCurrentUser().Person;
            if (user.IsInRole("Admin") || userPerson.Id == editPersonId)
            {
                return true;
            }

            var spouse = PersonHelper.GetSpouse(userPerson.Id);
            if (spouse != null && spouse.Id == editPersonId)
            {
                return true;
            }

            var editPerson = Database.People.Find(editPersonId);
            return (editPerson.FatherId != null && (userPerson.Id == editPerson.FatherId || spouse?.Id == editPerson.FatherId))
                || (editPerson.MotherId != null && (userPerson.Id == editPerson.MotherId || spouse?.Id == editPerson.MotherId));
        }

        public int? FindUserPerson(ApplicationUser user)
        {
            // Check if the email address belongs to an existing person
            var existingContacts = Database.Contacts.Where(c => c.ContactDetails.ToLower().Equals(user.Email.ToLower())).ToList();
            if (existingContacts.Count == 1)
            {
                return existingContacts[0].PersonId;
            }

            return null;
        }

        private string AdminRoleId()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Database));
            return roleManager.Roles.FirstOrDefault(r => r.Name == "Admin")?.Id;
        }
    }
}
