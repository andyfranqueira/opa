// <copyright file="CoupleViewModel.cs" company="Anargyroi Development">
//   Copyright 2018 Andrew Franqueira
//  
//   This file is part of Online Parish Administration.
//   Licensed under GNU General Public License 3.0 or later. 
//   Some rights reserved. See COPYING.
//  
//   @license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+
// </copyright>

using System.ComponentModel.DataAnnotations;
using OPA.Entities;

namespace OPA.Models
{
    public class CoupleViewModel
    {
        public CoupleViewModel()
        {
        }

        public CoupleViewModel(Couple couple)
        {
            MapToCoupleViewModel(couple);
        }

        public int Id { get; set; }
        public int HusbandId { get; set; }
        [Required]
        public int? WifeId { get; set; }
        public bool Active { get; set; }

        public PersonViewModel Husband { get; set; }
        public PersonViewModel Wife { get; set; }

        public Couple MapToCouple()
        {
            return new Couple
            {
                Id = Id,
                HusbandId = HusbandId,
                WifeId = WifeId,
                Active = Active
            };
        }

        private void MapToCoupleViewModel(Couple couple)
        {
            Id = couple.Id;
            HusbandId = couple.HusbandId;
            WifeId = couple.WifeId;
            Active = couple.Active;
            Husband = new PersonViewModel(couple.Husband);
            Wife = new PersonViewModel(couple.Wife);
        }
    }
}