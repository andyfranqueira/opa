// <copyright file="Couple.cs" company="The OPA Project">
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
    public class Couple
    {
        public int Id { get; set; }
        public int Person1Id { get; set; }
        public int? Person2Id { get; set; }

        public virtual Person Person1 { get; set; }
        public virtual Person Person2 { get; set; }
    }
}
