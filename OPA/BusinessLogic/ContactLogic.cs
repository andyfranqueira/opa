// <copyright file="ContactLogic.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
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

        public IEnumerable<SelectListItem> GetEligibleAddressList(int personId)
        {
            var familyIds = GetFamilyIds(personId);

            var currentAddressIds = Database
                .ContactAddresses
                .Where(c => c.PersonId == personId)
                .Select(c => c.AddressId);

            var familyAddresses = Database
                .ContactAddresses
                .Where(c => c.PersonId != null && familyIds.Contains(c.PersonId.Value))
                .Select(c => c.Address)
                .Distinct()
                .ToList();

            var eligibleAddresses = new List<SelectListItem>();
            foreach (var address in familyAddresses.Where(a => !currentAddressIds.Contains(a.Id)))
            {
                eligibleAddresses.Add(new SelectListItem
                {
                    Value = address.Id.ToString(),
                    Text = Utilities.FormatAddress(address)
                });
            }

            return eligibleAddresses.OrderBy(i => i.Text).ToList();
        }

        public void SetAddressForFamily(int personId, int addressId)
        {
            foreach (var id in GetFamilyIds(personId))
            {
                if (!Database.ContactAddresses.Any(a => a.PersonId == id && a.AddressId == addressId))
                {
                    Database.ContactAddresses.Add(new ContactAddress { PersonId = id, AddressId = addressId });
                    Database.SaveChanges();
                }
            }
        }

        public bool IsFamilyAddress(int personId, int addressId)
        {
            var familyIds = GetFamilyIds(personId);
            return Database.ContactAddresses.Any(c => c.PersonId != null && familyIds.Contains(c.PersonId.Value) && c.AddressId == addressId);
        }

        private List<int> GetFamilyIds(int personId)
        {
            var personHelper = new PersonLogic(Database);
            var familyIds = new List<int>();

            var spouse = personHelper.GetSpouse(personId);
            if (spouse != null)
            {
                familyIds.Add(spouse.Id);
            }

            familyIds.AddRange(personHelper.GetChildren(personId, spouse?.Id).Select(p => p.Id));
            return familyIds;
        }
    }
}
