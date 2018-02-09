// <copyright file="ContactAddress.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

namespace OPA.Entities
{
    public class ContactAddress
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }
    }
}
