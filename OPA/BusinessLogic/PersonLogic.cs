// <copyright file="PersonLogic.cs" company="The OPA Project">
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
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using OPA.DataAccess;
using OPA.Entities;

namespace OPA.BusinessLogic
{
    public class PersonLogic
    {
        public PersonLogic(OpaContext database)
        {
            Database = database;
        }

        protected OpaContext Database { get; set; }

        public List<SelectListItem> GetMemberTypeList()
        {
            return Database.ValueSets
                .Where(v => v.Set == ValueSet.MemberType)
                .OrderBy(v => v.Order)
                .Select(v => new SelectListItem { Value = v.Option, Text = v.Option })
                .ToList();
        }

        public Person GetPersonByUserEmail(string email)
        {
            return Database.ApplicationUsers.FirstOrDefault(u => u.Email == email)?.Person;
        }

        public Person GetPersonByDonorId(string donorId)
        {
            return Database.People.FirstOrDefault(p => p.DonorId == donorId);
        }

        public bool IsMarried(int personId)
        {
            return Database.Couples.Any(c => c.Person1Id == personId || c.Person2Id == personId);
        }

        public Person GetSpouse(int personId)
        {
            var couple = Database.Couples.SingleOrDefault(c => c.Person1Id == personId || c.Person2Id == personId);
            if (couple != null)
            {
                return couple.Person1Id == personId ? couple.Person2 : couple.Person1;
            }

            return null;
        }

        public IEnumerable<Person> GetParents(Person person)
        {
            var parents = new List<Person>();
            parents.AddRange(Database.People.Where(p => p.Id == person.FatherId));
            parents.AddRange(Database.People.Where(p => p.Id == person.MotherId));
            return parents;
        }

        public IEnumerable<Person> GetChildren(Person person)
        {
            var children = new List<Person>();
            children.AddRange(Database.People.Where(p => p.FatherId == person.Id));
            children.AddRange(Database.People.Where(p => p.MotherId == person.Id));
            return children.OrderBy(c => c.DateOfBirth);
        }

        public IEnumerable<Person> GetImmediateFamily(Person person)
        {
            var family = new List<Person> { person };

            // Add person, person's parents & children to list
            family.AddRange(GetParents(person));
            family.AddRange(GetChildren(person));

            // Add spouse, spouse's parents & children to list
            var spouse = GetSpouse(person.Id);
            if (spouse != null)
            {
                family.Add(spouse);
                family.AddRange(GetParents(spouse));
                family.AddRange(GetChildren(spouse));
            }

            return family.Distinct();
        }

        public IEnumerable<SelectListItem> GetEligibleMates(Sex personSex)
        {
            var ageCheck = DateTime.Today.AddYears(-18);
            Sex mateSex;

            switch (personSex)
            {
                case Sex.Male:
                    mateSex = Sex.Female;
                    break;
                case Sex.Female:
                    mateSex = Sex.Male;
                    break;
                default:
                    return null;
            }

            var coupleIds = Database.Couples.Select(c => c.Person1Id).ToList();
            coupleIds.AddRange(Database.Couples.Select(c => (int)c.Person2Id));

            return Database
                .People
                .Where(p => p.Sex == mateSex && !coupleIds.Contains(p.Id) && (p.DateOfBirth ?? ageCheck) <= ageCheck)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.LastName + ", " + p.FirstName })
                .OrderBy(i => i.Text).ToList();
        }

        public string GetProfilePhoto(int personId, bool find = true)
        {
            if (!find || File.Exists(HostingEnvironment.MapPath("~/Content/photos/profile-" + personId + ".jpg")))
            {
                return "/Content/photos/profile-" + personId + ".jpg";
            }

            return "/Content/photos/profile-blank.jpg";
        }
    }
}
