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
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net;
using OPA.DataAccess;
using OPA.Entities;
using OPA.Payments;

namespace OPA.BusinessLogic
{
    public class PaymentLogic
    {
        private static readonly string SquareKey = ConfigurationManager.AppSettings["key:Square"];
        private static readonly string StripeKey = ConfigurationManager.AppSettings["key:Stripe"];
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentLogic(OpaContext database, PersonLogic personHelper)
        {
            Database = database;
            PersonHelper = personHelper;
        }

        protected OpaContext Database { get; set; }
        protected PersonLogic PersonHelper { get; set; }

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

            Logger.Info("Get Square Transactions");
            var transactions = PaymentManager.GetSquareTransactions(SquareKey, startDate, endDate);

            Logger.Info("Get Donor Box Transactions");
            transactions.AddRange(PaymentManager.GetDonorBoxTransactions(StripeKey, startDate, endDate));

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
            var unprocessedPayments = Database.Payments.Where(p => !p.Donations.Any()).ToList();

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
                DonorId = transaction.DonorId,
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
            Person person = null;

            if (!string.IsNullOrWhiteSpace(payment.DonorId))
            {
                person = PersonHelper.GetPersonByDonorId(payment.DonorId);
            }

            if (person == null && !string.IsNullOrWhiteSpace(payment.DonorEmail))
            {
                person = PersonHelper.GetPersonByUserEmail(payment.DonorEmail);

                if (person != null && !string.IsNullOrWhiteSpace(payment.DonorId))
                {
                    person.DonorId = payment.DonorId;
                    Database.SaveChanges();
                }
            }

            return person;
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
