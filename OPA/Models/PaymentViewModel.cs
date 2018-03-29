// <copyright file="PaymentViewModel.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
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

    public class DesignationViewModel
    {
        public string Fund { get; set; }
        public string Designation { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentDonationViewModel
    {
        public PaymentDonationViewModel()
        {
        }

        public PaymentDonationViewModel(Payment payment, int initialize)
        {
            PaymentId = payment.Id;
            DonorName = payment.DonorName;
            PaymentMethod = payment.PaymentMethod + "\n" + payment.TransactionDate + "\n$" + payment.Amount;
            PaymentAmount = payment.Amount;
            PaymentDetails = payment.PaymentDetails;
            Designations = new List<DesignationViewModel> { new DesignationViewModel { Amount = payment.Amount } };

            for (var i = 1; i < initialize; i++)
            {
                Designations.Add(new DesignationViewModel());
            }
        }

        public int PaymentId { get; set; }
        public string DonorName { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentDetails { get; set; }

        public List<SelectListItem> DonorList { get; set; }
        public List<SelectListItem> FundList { get; set; }

        [Display(Name = "Donor")]
        public int PersonId { get; set; }
        public List<DesignationViewModel> Designations { get; set; }
    }
}