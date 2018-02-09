// <copyright file="FinancialLogic.cs" company="The OPA Project">
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
    public class FinancialLogic
    {
        public const string ReceiptTemplate = "/Content/templates/DonationReceipt.docx";

        public FinancialLogic(OpaContext database)
        {
            Database = database;
        }

        protected OpaContext Database { get; set; }

        public List<SelectListItem> GetFrequencyList()
        {
            return Database.ValueSets
                .Where(v => v.Set == ValueSet.Frequency)
                .OrderBy(v => v.Order)
                .Select(v => new SelectListItem { Value = v.Option, Text = v.Option })
                .ToList();
        }

        public List<SelectListItem> GetFundList()
        {
            return Database.ValueSets
                .Where(v => v.Set == ValueSet.Fund)
                .OrderBy(v => v.Order)
                .Select(v => new SelectListItem { Value = v.Option, Text = v.Option })
                .ToList();
        }

        public List<SelectListItem> GetDonorList()
        {
            return Database.People
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.LastName.Contains("Anonymous") ? p.LastName : p.LastName + ", " + p.FirstName
                })
                .OrderByDescending(i => i.Text.Contains("Anonymous"))
                .ThenBy(i => i.Text)
                .ToList();
        }

        public IEnumerable<Pledge> GetPledges(int personId, int? spouseId)
        {
            var pledges = Database.Pledges.Where(d => d.PersonId == personId).ToList();

            if (spouseId != null)
            {
                pledges.AddRange(Database.Pledges.Where(d => d.PersonId == spouseId));
            }

            return pledges.OrderByDescending(p => p.Year).ThenByDescending(p => p.Amount).ToList();
        }

        public IEnumerable<Donation> GetDonations(int personId, int? spouseId)
        {
            var donations = Database.Donations.Where(d => d.PersonId == personId).ToList();

            if (spouseId != null)
            {
                donations.AddRange(Database.Donations.Where(d => d.PersonId == spouseId));
            }

            return donations.OrderByDescending(d => d.DonationDate).ToList();
        }

        public IEnumerable<Donation> GetOrgDonations(int organizationId)
        {
            return Database.Donations
                .Where(d => d.OrganizationId == organizationId)
                .OrderByDescending(d => d.DonationDate)
                .ToList();
        }

        public Dictionary<string, string> ReceiptData(Donation donation, Person spouse)
        {
            var donorName = donation.Person.FirstName + " " + donation.Person.LastName;

            if (spouse != null)
            {
                donorName = donorName + " and " + spouse.FirstName + " " + spouse.LastName;
            }
 
            var address = donation.Person.ContactAddresses.FirstOrDefault()?.Address;

            var details = $"{donation.Amount:C}"
                          + " on " + donation.DonationDate.ToShortDateString()
                          + " for " + donation.Fund + ". ";

            if (donation.Payment != null)
            {
                details = details + donation.Payment.PaymentMethod;
            }
            else if (!string.IsNullOrEmpty(donation.CheckNumber))
            {
                details = details + "Check Number: " + donation.CheckNumber;
            }
            
            var fieldValues = new Dictionary<string, string>
            {
                { "DonorName", donorName },
                { "DonorAddressLine1", address?.AddressLine },
                { "DonorAddressLine2", address?.City + ", " + address?.State + " " + address?.PostalCode },
                { "DonationDetails", details }
            };

            return fieldValues;
        }
    }
}
