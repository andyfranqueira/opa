// <copyright file="ManageViewModels.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.ComponentModel.DataAnnotations;
using OPA.BusinessLogic;
using OPA.Entities;

namespace OPA.Models
{
    public class ManageViewModels
    {
    }

    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public bool BrowserRemembered { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public int PersonId { get; set; }

        [Display(Name = "Assigned Person Record")]
        public string PersonName { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
        }

        public UserViewModel(ApplicationUser user, UserLogic userHelper)
        {
            MapToUserViewModel(user, userHelper);
        }

        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        public bool Admin { get; set; }
        public int? PersonId { get; set; }

        [Display(Name = "Assigned Person Record")]
        public string PersonName { get; set; }

        private void MapToUserViewModel(ApplicationUser user, UserLogic userHelper)
        {
            Id = user.Id;
            UserName = user.UserName;
            EmailConfirmed = user.EmailConfirmed;
            Admin = userHelper.IsAdmin(user);
            PersonId = user.PersonId;
            PersonName = (user.PersonId != null) ? new PersonViewModel(user.Person).FullName : string.Empty;
        }
    }

    public class ValueSetViewModel
    {
        public ValueSetViewModel()
        {
        }

        public ValueSetViewModel(Value value)
        {
            MapToValueSetViewModel(value);
        }

        public int Id { get; set; }
        public ValueSet Set { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }

        public Value MapToValueSet()
        {
            return new Value
            {
                Id = Id,
                Set = Set,
                Option = Option,
                Order = Order
            };
        }

        private void MapToValueSetViewModel(Value value)
        {
            Id = value.Id;
            Set = value.Set;
            Option = value.Option;
            Order = value.Order;
        }
    }

    public class SupportRequestViewModel
    {
        public int PersonId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
    }
}