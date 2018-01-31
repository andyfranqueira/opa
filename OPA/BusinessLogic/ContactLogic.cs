// <copyright file="ContactLogic.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OPA.DataAccess;
using OPA.Entities;

namespace OPA.BusinessLogic
{
    public class ContactLogic
    {
        public ContactLogic(OpaContext database)
        {
            Database = database;
        }

        protected OpaContext Database { get; set; }

        public List<SelectListItem> GetContactTypes()
        {
            return Database.ValueSets
                .Where(v => v.Set == ValueSet.ContactType)
                .OrderBy(v => v.Order)
                .Select(v => new SelectListItem { Value = v.Option, Text = v.Option })
                .ToList();
        }

        public IEnumerable<Address> GetFamilyAddresses(Person person)
        {
            var personHelper = new PersonLogic(Database);
            var familyIds = personHelper.GetImmediateFamily(person).Select(p => (int?)p.Id).ToList();
            return Database.ContactAddresses.Where(c => familyIds.Contains(c.PersonId)).Select(c => c.Address).Distinct().ToList();
        }

        public IEnumerable<SelectListItem> GetEligibleAddressList(int? personId)
        {
            var person = Database.People.Find(personId);

            var familyAddresses = GetFamilyAddresses(person);
            var currentAddresses = Database.ContactAddresses.Where(c => c.PersonId == personId).Select(c => c.AddressId);

            var addressList = new List<SelectListItem>();
            foreach (var address in familyAddresses.Where(a => !currentAddresses.Contains(a.Id)))
            {
                addressList.Add(new SelectListItem { Value = address.Id.ToString(), Text = Utilities.FormatAddress(address) });
            }

            return addressList.OrderBy(i => i.Text).ToList();
        }
    }
}
