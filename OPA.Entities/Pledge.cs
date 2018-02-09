// <copyright file="Pledge.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

namespace OPA.Entities
{
    public class Pledge
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; }
        public string Fund { get; set; }
        public int PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
