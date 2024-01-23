﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class City
    {
        public City()
        {
            District = new HashSet<District>();
            Supplier = new HashSet<Supplier>();
        }

        public int CityId { get; set; }
        public string CityName { get; set; }
        public int? RegionId { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<District> District { get; set; }
        public virtual ICollection<Supplier> Supplier { get; set; }
    }
}