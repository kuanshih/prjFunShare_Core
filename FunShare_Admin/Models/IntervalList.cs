﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class IntervalList
    {
        public IntervalList()
        {
            Product = new HashSet<Product>();
        }

        public int IntervalId { get; set; }
        public string IntervalDescription { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}