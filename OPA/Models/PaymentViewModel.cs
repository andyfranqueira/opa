// <copyright file="PaymentViewModel.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OPA.Entities;

namespace OPA.Models
{
    public class PaymentViewModel
    {
        public PaymentViewModel()
        {
        }

        public PaymentViewModel(Payment payment)
        {
            Id = payment.Id;
            TransactionId = payment.TransactionId;
            Source = payment.Source;
            DonorName = payment.DonorName;
            DonorEmail = payment.DonorEmail;
            DonorId = payment.DonorId;
            PaymentMethod = payment.PaymentMethod;
            Amount = payment.Amount;
            Fee = payment.Fee;
            Net = payment.Net;
            Currency = payment.Currency;
            PaymentDetails = payment.PaymentDetails;
            RecurringDonation = payment.RecurringDonation;
            TransactionDate = payment.TransactionDate;
            PostedDate = payment.PostedDate;
            DonationRecorded = payment.Donations?.Any() ?? false;
        }

        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string Source { get; set; }

        [Display(Name = "Donor Name")]
        public string DonorName { get; set; }

        [Display(Name = "Donor Email")]
        public string DonorEmail { get; set; }
        public string DonorId { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Fee { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Net { get; set; }
        public string Currency { get; set; }

        [Display(Name = "Payment Details")]
        public string PaymentDetails { get; set; }

        [Display(Name = "Recurring Donation")]
        public bool RecurringDonation { get; set; }

        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Posted Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PostedDate { get; set; }

        [Display(Name = "Donation Recorded")]
        public bool DonationRecorded { get; set; }
    }

    public class PaymentDonationViewModel
    {
        public PaymentDonationViewModel()
        {
        }

        public PaymentDonationViewModel(Payment payment)
        {
            PaymentId = payment.Id;
            DonorName = payment.DonorName;
            PaymentMethod = payment.PaymentMethod;
            Amount = payment.Amount;
            PaymentDetails = payment.PaymentDetails;
            TransactionDate = payment.TransactionDate;
        }

        public int PaymentId { get; set; }

        [Display(Name = "Donor Name")]
        public string DonorName { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TransactionDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Details")]
        public string PaymentDetails { get; set; }

        [Display(Name = "Donor")]
        public int PersonId { get; set; }

        public string Fund { get; set; }
    }
}