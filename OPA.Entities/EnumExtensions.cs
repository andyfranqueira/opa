// <copyright file="EnumExtensions.cs" company="The OPA Project">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of OPA.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace OPA.Entities
{
    public enum Sex
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public enum ValueSet
    {
        [Display(Name = "Contact Type")]
        ContactType = 0,
        [Display(Name = "Frequency")]
        Frequency = 1,
        [Display(Name = "Fund")]
        Fund = 2,
        [Display(Name = "Member Type")]
        MemberType = 3
    }

    public static class EnumExtensions
    {
        public static string GetDisplay(this object enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), true);
            return attributes?.Length > 0 ? ((DisplayAttribute)attributes[0]).Name : enumValue.ToString();
        }
    }
}
