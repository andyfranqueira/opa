// <copyright file="Value.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

namespace OPA.Entities
{
    public class Value
    {
        public int Id { get; set; }
        public ValueSet Set { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
    }
}
