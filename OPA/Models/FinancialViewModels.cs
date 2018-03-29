// <copyright file="FinancialViewModels.cs" company="The OPA Project">
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
using System.Web.Mvc;
using OPA.BusinessLogic;
using OPA.Entities;

namespace OPA.Models
{
    public class FinancialViewModels
    {
    }

    public class PledgeViewModel
    {
        public PledgeViewModel()
        {
        }

        public PledgeViewModel(Pledge pledge)
        {
            MapToPledgeViewModel(pledge);
        }

        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Amount { get; set; }

        public string Frequency { get; set; }

        public string Fund { get; set; }

        public int PersonId { get; set; }

        [Display(Name = "Donor Info")]
        public string DonorInfo { get; set; }

        public Pledge MapToPledge()
        {
            if (Frequency == null)
            {
                return null;
            }

            var pledge = new Pledge
            {
                Id = Id,
                Year = Year,
                Amount = Amount ?? 0,
                Frequency = Frequency,
                Fund = Fund,
                PersonId = PersonId
            };

            return pledge;
        }

        public void UpdatePledge(Pledge pledge)
        {
            pledge.Year = Year;
            pledge.Amount = Amount ?? 0;
            pledge.Frequency = Frequency;
            pledge.Fund = Fund;
        }

        private void MapToPledgeViewModel(Pledge pledge)
        {
            Id = pledge.Id;
            Year = pledge.Year;
            Amount = pledge.Amount;
            Frequency = pledge.Frequency;
            Fund = pledge.Fund;
            PersonId = pledge.PersonId;
            DonorInfo = pledge.Person.LastName.Contains("Anonymous")
                ? pledge.Person.LastName
                : Utilities.FormatName(pledge.Person);
        }
    }

    public class DonationViewModel
    {
        public DonationViewModel()
        {
        }

        public DonationViewModel(Donation donation)
        {
            MapToDonationViewModel(donation);
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Donation Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DonationDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal? Amount { get; set; }

        public string Fund { get; set; }
        public string Designation { get; set; }

        [Display(Name = "Check Number")]
        public string CheckNumber { get; set; }

        public int? PersonId { get; set; }
        public int? OrganizationId { get; set; }

        [Display(Name = "Donor Info")]
        public string DonorInfo { get; set; }

        [Display(Name = "Payment Info")]
        public string PaymentInfo { get; set; }

        [Display(Name = "Recurring")]
        public bool RecurringPayment { get; set; }

        public Donation MapToDonation()
        {
            var donation = new Donation
            {
                Id = Id,
                DonationDate = DonationDate ?? DateTime.Today,
                Amount = Amount ?? 0,
                Fund = Fund,
                Designation = Designation,
                CheckNumber = CheckNumber
            };

            if (OrganizationId != null)
            {
                donation.OrganizationId = OrganizationId;
            }
            else
            {
                donation.PersonId = PersonId;
            }

            return donation;
        }

        public void UpdateDonation(Donation donation)
        {
            donation.DonationDate = DonationDate ?? DateTime.Today;
            donation.Amount = Amount ?? 0;
            donation.Fund = Fund;
            donation.Designation = Designation;
            donation.CheckNumber = CheckNumber;
        }

        private void MapToDonationViewModel(Donation donation)
        {
            Id = donation.Id;
            DonationDate = donation.DonationDate;
            Amount = donation.Amount;
            Fund = donation.Fund;
            Designation = donation.Designation;
            CheckNumber = donation.CheckNumber;
            PersonId = donation.PersonId;
            OrganizationId = donation.OrganizationId;

            if (donation.Payment != null)
            {
                PaymentInfo = donation.Payment.DonorName + "<br/>"
                    + donation.Payment.PaymentMethod + "<br/>"
                    + donation.Payment.TransactionDate + "<br/>" 
                    + "$" + donation.Payment.Amount;

                RecurringPayment = donation.Payment.RecurringDonation;
            }

            DonorInfo = donation.Person != null
                ? (donation.Person.LastName.Contains("Anonymous")
                ? donation.Person.LastName
                : Utilities.FormatName(donation.Person))
                : donation.Organization.Name;
        }
    }

    public class BatchDonationsViewModel
    {
        public BatchDonationsViewModel()
        {
        }

        public BatchDonationsViewModel(int initialize)
        {
            Donations = new List<DonationViewModel>();

            for (var i = 1; i <= initialize; i++)
            {
                Donations.Add(new DonationViewModel());
            }
        }

        public List<SelectListItem> DonorList { get; set; }
        public List<SelectListItem> FundList { get; set; }
        public List<DonationViewModel> Donations { get; set; }
    }
}