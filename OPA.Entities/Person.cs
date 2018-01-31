// <copyright file="Person.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Collections.Generic;

namespace OPA.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public Sex Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Orthodox { get; set; }
        public bool Active { get; set; }
        public int? LegacyMemberId { get; set; }
        public int? LegacyFamilyId { get; set; }
        public int? FatherId { get; set; }
        public int? MotherId { get; set; }
        public string DonorId { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<ContactAddress> ContactAddresses { get; set; }
    }
}
