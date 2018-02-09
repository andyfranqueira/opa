// <copyright file="ContactViewModel.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.ComponentModel.DataAnnotations;
using OPA.Entities;

namespace OPA.Models
{
    public class ContactViewModel
    {
        public ContactViewModel()
        {
        }

        public ContactViewModel(Contact contact)
        {
            MapToContactViewModel(contact);
        }

        public int Id { get; set; }

        [Display(Name = "Contact Type")]
        public string ContactType { get; set; }

        [Required]
        [Display(Name = "Contact Details")]
        public string ContactDetails { get; set; }

        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }

        public Contact MapToContact()
        {
            return new Contact
            {
                Id = Id,
                ContactType = ContactType,
                ContactDetails = ContactDetails,
                PersonId = PersonId,
                OrganizationId = OrganizationId
            };
        }

        private void MapToContactViewModel(Contact contact)
        {
            Id = contact.Id;
            ContactType = contact.ContactType;
            ContactDetails = contact.ContactDetails;
            PersonId = contact.PersonId;
            OrganizationId = contact.OrganizationId;
        }
    }

    public class ContactAddressViewModel
    {
        public ContactAddressViewModel()
        {
        }

        public ContactAddressViewModel(ContactAddress contactAddress)
        {
            MapToContactAddressViewModel(contactAddress);
        }

        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }
        public int? AddressId { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string AddressLine { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        public Address MapToAddress()
        {
            var address = new Address
            {
                AddressLine = AddressLine,
                City = City,
                State = State,
                PostalCode = PostalCode,
                Country = Country
            };

            if (AddressId != null)
            {
                address.Id = (int)AddressId;
            }

            return address;
        }

        public ContactAddress MapToContactAddress()
        {
            return new ContactAddress
            {
                Id = Id,
                PersonId = PersonId,
                OrganizationId = OrganizationId,
                AddressId = AddressId
            };
        }

        private void MapToContactAddressViewModel(ContactAddress contactAddress)
        {
            Id = contactAddress.Id;
            PersonId = contactAddress.PersonId;
            OrganizationId = contactAddress.OrganizationId;
            AddressId = contactAddress.Address.Id;
            AddressLine = contactAddress.Address.AddressLine;
            City = contactAddress.Address.City;
            State = contactAddress.Address.State;
            PostalCode = contactAddress.Address.PostalCode;
            Country = contactAddress.Address.Country;
        }
    }
}