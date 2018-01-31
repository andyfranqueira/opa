// <copyright file="PaymentManager.cs" company="Anargyroi Development">
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

namespace OPA.Payments
{
    public static class PaymentManager
    {
        public static List<Transaction> GetSquareTransactions(string apiKey, DateTime startDate, DateTime endDate)
        {
            return SquareWorker.GetTransactions(apiKey, startDate, endDate);
        }

        public static List<Transaction> GetDonorBoxTransactions(string apiKey, DateTime startDate, DateTime endDate)
        {
            return StripeWorker.GetDonorBoxTransactions(apiKey, startDate, endDate);
        }
    }
}
