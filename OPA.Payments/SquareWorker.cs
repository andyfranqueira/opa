// <copyright file="SquareWorker.cs" company="The OPA Project">
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
using Square.Connect.Api;
using Square.Connect.Client;
using Square.Connect.Model;

namespace OPA.Payments
{
    public static class SquareWorker
    {
        public static List<Transaction> GetTransactions(string apiKey, DateTime startDate, DateTime endDate)
        {
            Configuration.Default.AccessToken = apiKey;

            var locationClient = new LocationsApi();
            var transactionClient = new V1TransactionsApi();

            var locations = locationClient.ListLocations();
            var payments = new List<V1Payment>();

            foreach (var location in locations.Locations)
            {
                payments.AddRange(transactionClient.ListPayments(
                    location.Id, 
                    null,
                    startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    endDate.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            }

            var transactions = payments.Select(SquareMapping.MapToTransaction).ToList();
            return transactions;
        }
    }
}
