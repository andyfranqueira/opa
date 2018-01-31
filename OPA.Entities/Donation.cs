// <copyright file="Donation.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;

namespace OPA.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public DateTime DonationDate { get; set; }
        public decimal Amount { get; set; }
        public string Fund { get; set; }
        public string Designation { get; set; }
        public string CheckNumber { get; set; }
        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }
        public int? PaymentId { get; set; }

        public virtual Person Person { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
