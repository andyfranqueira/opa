// <copyright file="PersonViewModel.cs" company="The OPA Project">
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
using System.ComponentModel.DataAnnotations;
using OPA.BusinessLogic;
using OPA.Entities;

namespace OPA.Models
{
    public class PersonViewModel
    {
        public PersonViewModel()
        {
        }

        public PersonViewModel(Person person)
        {
            MapToPersonViewModel(person);
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Name")]
        public string FullName { get; set; }

        public Sex Sex { get; set; }

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Member Type")]
        public string MemberType { get; set; }
        public bool Active { get; set; }

        public int? ParentId { get; set; }

        [Display(Name = "Child of Spouse")]
        public bool ForCouple { get; set; }

        public PersonViewModel Spouse { get; set; }
        public List<PersonViewModel> Parents { get; set; }
        public List<PersonViewModel> Children { get; set; }
        public List<ContactAddressViewModel> Addresses { get; set; }
        public List<ContactViewModel> Contacts { get; set; }
        public List<PledgeViewModel> Pledges { get; set; }
        public List<DonationViewModel> Donations { get; set; }

        public string ProfilePhoto { get; set; }

        public Person MapToPerson()
        {
            return new Person
            {
                Id = Id,
                LastName = LastName,
                FirstName = FirstName,
                MiddleName = MiddleName,
                Sex = Sex,
                DateOfBirth = DateOfBirth,
                MemberType = MemberType,
                Active = Active
            };
        }

        private void MapToPersonViewModel(Person person)
        {
            Id = person.Id;
            LastName = person.LastName;
            FirstName = person.FirstName;
            MiddleName = person.MiddleName;
            FullName = Utilities.FormatName(person, true);
            Sex = person.Sex;
            DateOfBirth = person.DateOfBirth;
            MemberType = person.MemberType;
            Active = person.Active;
        }
    }

    public class CoupleViewModel
    {
        public int Id { get; set; }
        public int Person1Id { get; set; }

        [Display(Name = "Spouse")]
        public int Person2Id { get; set; }

        public PersonViewModel Person1 { get; set; }
        public PersonViewModel Person2 { get; set; }

        public Couple MapToCouple()
        {
            return new Couple
            {
                Id = Id,
                Person1Id = Person1Id,
                Person2Id = Person2Id
            };
        }
    }
}