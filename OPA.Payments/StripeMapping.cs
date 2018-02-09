// <copyright file="StripeMapping.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using Stripe;

namespace OPA.Payments
{
    public static class StripeMapping
    {
        public static Transaction MapToTransaction(StripeBalanceTransaction transaction, StripeCharge charge)
        {
            string donorName;
            string donorEmail;
            string donorId;
            string designation;
            string recurringDonation;

            charge.Metadata.TryGetValue("donorbox_name", out donorName);
            charge.Metadata.TryGetValue("donorbox_email", out donorEmail);
            charge.Metadata.TryGetValue("donorbox_donor_id", out donorId);
            charge.Metadata.TryGetValue("donorbox_designation", out designation);
            charge.Metadata.TryGetValue("donorbox_recurring_donation", out recurringDonation);

            return new Transaction
            {
                TransactionId = transaction.Id,
                Source = "DonorBox",
                DonorName = donorName,
                DonorEmail = donorEmail,
                DonorId = donorId,
                PaymentMethod = GetPaymentMethod(charge),
                Amount = decimal.Divide(transaction.Amount, 100),
                Fee = decimal.Divide(transaction.Fee, 100),
                Net = decimal.Divide(transaction.Net, 100),
                Currency = transaction.Currency.ToUpper(),
                PaymentDetails = designation,
                RecurringDonation = recurringDonation == "true",
                TransactionDate = transaction.Created,
                PostedDate = transaction.AvailableOn
            };
        }

        private static string GetPaymentMethod(StripeCharge charge)
        {
            if (charge.Source.Card != null)
            {
                return charge.Source.Card.Brand + " ****" + charge.Source.Card.Last4;
            }

            if (charge.Source.BankAccount != null)
            {
                return charge.Source.BankAccount.BankName + " ****" + charge.Source.BankAccount.Last4;
            }

            return string.Empty;
        }
    }
}
