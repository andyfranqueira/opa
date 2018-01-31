// <copyright file="Transaction.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;

namespace OPA.Payments
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string Source { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }
        public string DonorId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal Net { get; set; }
        public string Currency { get; set; }
        public string PaymentDetails { get; set; }
        public bool RecurringDonation { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
