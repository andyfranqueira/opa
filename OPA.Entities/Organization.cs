// <copyright file="Organization.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.Collections.Generic;

namespace OPA.Entities
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int? LegacyId { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; } 
        public virtual ICollection<ContactAddress> ContactAddresses { get; set; }
    }
}
