// <copyright file="SquareMapping.cs" company="The OPA Project">
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
using System.Globalization;
using System.Linq;
using System.Net;
using Square.Connect.Model;

namespace OPA.Payments
{
    public static class SquareMapping
    {
        public static Transaction MapToTransaction(V1Payment payment)
        {
            return new Transaction
            {
                TransactionId = payment.Id,
                Source = "Square",
                DonorName = GetCustomerFromReceipt(payment.ReceiptUrl),
                PaymentMethod = GetCardSummary(payment.Tender),
                Amount = decimal.Divide(payment.TotalCollectedMoney.Amount ?? 0, 100),
                Fee = decimal.Divide(-1 * payment.ProcessingFeeMoney.Amount ?? 0, 100),
                Net = decimal.Divide(payment.NetTotalMoney.Amount ?? 0, 100),
                Currency = payment.TotalCollectedMoney.CurrencyCode.ToString(),
                PaymentDetails = GetItemSummary(payment.Itemizations),
                RecurringDonation = false,
                TransactionDate = Convert.ToDateTime(payment.CreatedAt),
                PostedDate = Convert.ToDateTime(payment.CreatedAt)
            };
        }

        private static string GetCustomerFromReceipt(string url)
        {
            using (var client = new WebClient())
            {
                var receipt = client.DownloadString(url);

                if (receipt.Contains("name_on_card"))
                {
                    var position = receipt.IndexOf("name_on_card", StringComparison.Ordinal) + 13;
                    receipt = receipt.Substring(position);
                    position = receipt.IndexOf("</", StringComparison.Ordinal) - 1;
                    receipt = receipt.Substring(1, position);
                }
                else
                {
                    receipt = string.Empty;
                }

                return FormatName(receipt);
            }
        }

        private static string GetItemSummary(IEnumerable<V1PaymentItemization> items)
        {
            var itemSummary = items
                .Select(item => item.Name + ": " 
                + (int)(item.Quantity ?? 0) 
                + " @ " + decimal.Divide(item.TotalMoney.Amount ?? 0, 100).ToString("$0.00"))
                .ToList();
            return string.Join("\n", itemSummary);
        }

        private static string GetCardSummary(IEnumerable<V1Tender> tenders)
        {
            var cardSummary = tenders
                .Select(t => FormatName(t.CardBrand.ToString()) + " ****" + t.PanSuffix)
                .ToList();
            return string.Join("; ", cardSummary);
        }

        private static string FormatName(string name)
        {
            switch (name)
            {
                case "MASTERCARD":
                    return "MasterCard";
                case "AMERICANEXPRESS":
                    return "American Express";
                default:
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
            }
        }
    }
}
