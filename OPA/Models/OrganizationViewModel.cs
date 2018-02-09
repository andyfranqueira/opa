// <copyright file="OrganizationViewModel.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OPA.Entities;

namespace OPA.Models
{
    public class OrganizationViewModel
    {
        public OrganizationViewModel()
        {
        }

        public OrganizationViewModel(Organization organization)
        {
            MapToOrganizationViewModel(organization);
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public bool Active { get; set; }

        public List<ContactAddressViewModel> Addresses { get; set; }
        public List<ContactViewModel> Contacts { get; set; }
        public List<DonationViewModel> Donations { get; set; }

        public Organization MapToOrganization()
        {
            return new Organization
            {
                Id = Id,
                Name = Name,
                Active = Active
            };
        }

        private void MapToOrganizationViewModel(Organization organization)
        {
            Id = organization.Id;
            Name = organization.Name;
            Active = organization.Active;
        }
    }
}