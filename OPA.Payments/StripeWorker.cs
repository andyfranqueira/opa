// <copyright file="StripeWorker.cs" company="Anargyroi Development">
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
using Stripe;

namespace OPA.Payments
{
    public static class StripeWorker
    {
        public static List<Transaction> GetDonorBoxTransactions(string apiKey, DateTime startDate, DateTime endDate)
        {
            StripeConfiguration.SetApiKey(apiKey);
            var transactions = new List<Transaction>();

            var balanceClient = new StripeBalanceService();
            var parameters = new StripeBalanceTransactionListOptions();

            var chargeClient = new StripeChargeService();
            var refundClient = new StripeRefundService();

            parameters.Created = new StripeDateFilter
            {
                GreaterThanOrEqual = startDate,
                LessThanOrEqual = endDate
            };

            foreach (var balanceTransaction in balanceClient.List(parameters))
            {
                StripeCharge charge = null;

                if (balanceTransaction.Type == "charge" || balanceTransaction.Type == "payment")
                {
                    charge = chargeClient.Get(balanceTransaction.SourceId);
                }
                else if (balanceTransaction.Type == "refund")
                {
                    var chargeId = refundClient.Get(balanceTransaction.SourceId).ChargeId;
                    charge = chargeClient.Get(chargeId);
                }

                if (charge?.Status == "succeeded")
                {
                    transactions.Add(StripeMapping.MapToTransaction(balanceTransaction, charge));
                }
            }

            return transactions;
        }
    }
}
