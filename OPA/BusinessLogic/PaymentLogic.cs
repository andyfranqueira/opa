// <copyright file="PaymentLogic.cs" company="The OPA Project">
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
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.Ajax.Utilities;
using OPA.DataAccess;
using OPA.Entities;
using OPA.Payments;

namespace OPA.BusinessLogic
{
    public class PaymentLogic
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentLogic(OpaContext database, PersonLogic personHelper)
        {
            Database = database;
            PersonHelper = personHelper;
        }

        protected OpaContext Database { get; set; }
        protected PersonLogic PersonHelper { get; set; }

        public string GetDonationForm(ApplicationUser user, Person person)
        {
            if (Utilities.DonationFormUrl.IsNullOrWhiteSpace())
            {
                return null;
            }

            if (person == null)
            {
                return Utilities.DonationFormUrl
                    + "&email=" + user.Email;
            }

            if (user.PersonId != person.Id)
            {
                return null;
            }

            if (Utilities.DonationFormUrl.Contains("donorbox"))
            {
                return Utilities.DonationFormUrl
                       + "&first_name=" + person.FirstName
                       + "&last_name=" + person.LastName
                       + "&email=" + user.Email;
            }

            return Utilities.DonationFormUrl;
        }

        public string GetDonationUserAcctUrl(ApplicationUser user, Person person)
        {
            if (user.PersonId != person.Id || Utilities.DonationUserAcctUrl.IsNullOrWhiteSpace())
            {
                return null;
            }

            if (Utilities.DonationUserAcctUrl.Contains("donorbox"))
            {
                return Utilities.DonationUserAcctUrl + "?user_session[email]=" + user.Email;
            }

            return Utilities.DonationUserAcctUrl;
        }

        public void LoadNewPayments()
        {
            var startDate = DateTime.Now.AddMonths(-6);
            var endDate = DateTime.Now.AddDays(1);

            var payments = Database.Payments;
            if (payments.Any())
            {
                startDate = payments.Max(p => p.TransactionDate).AddDays(-10);
            }

            var ids = payments.Select(p => p.TransactionId);
            var transactions = new List<Transaction>();

            if (!Utilities.StripeKey.IsNullOrWhiteSpace())
            {
                Logger.Info("Get Donor Box Transactions");
                transactions.AddRange(PaymentManager.GetDonorBoxTransactions(Utilities.StripeKey, startDate, endDate));
            }

            if (!Utilities.SquareKey.IsNullOrWhiteSpace())
            {
                Logger.Info("Get Square Transactions");
                transactions.AddRange(PaymentManager.GetSquareTransactions(Utilities.SquareKey, startDate, endDate));
            }

            foreach (var transaction in transactions)
            {
                if (!ids.Contains(transaction.TransactionId))
                {
                    var payment = MapToPayment(transaction);
                    payments.Add(payment);
                    Database.Payments.Add(payment);
                    Database.SaveChanges();
                }
            }
        }

        public void ProcessPayments()
        {
            var unprocessedPayments = Database.Payments.Where(p => p.Source != "Square" && !p.Donations.Any()).ToList();

            foreach (var payment in unprocessedPayments)
            {
                var person = GetDonor(payment);
                if (person != null)
                {
                    RecordPaymentAsDonation(payment, person.Id, payment.PaymentDetails, payment.PaymentDetails, payment.Amount);
                }
            }
        }

        public Payment MapToPayment(Transaction transaction)
        {
            return new Payment
            {
                TransactionId = transaction.TransactionId,
                Source = transaction.Source,
                DonorName = transaction.DonorName,
                DonorEmail = transaction.DonorEmail,
                PaymentMethod = transaction.PaymentMethod,
                Amount = transaction.Amount,
                Fee = transaction.Fee,
                Net = transaction.Net,
                Currency = transaction.Currency,
                PaymentDetails = transaction.PaymentDetails,
                RecurringDonation = transaction.RecurringDonation,
                TransactionDate = transaction.TransactionDate,
                PostedDate = transaction.PostedDate
            };
        }

        public Person GetDonor(Payment payment)
        {
            return !string.IsNullOrWhiteSpace(payment.DonorEmail) ? 
                PersonHelper.GetPersonByUserEmail(payment.DonorEmail) : 
                null;
        }

        public void RecordPaymentAsDonation(Payment payment, int personId, string fund, string designation, decimal amount)
        {
            Database.Donations.Add(new Donation
            {
                DonationDate = payment.TransactionDate,
                Fund = fund,
                Amount = amount,
                Designation = designation,
                PersonId = personId,
                PaymentId = payment.Id
            });

            Database.SaveChanges();
        }
    }
}
