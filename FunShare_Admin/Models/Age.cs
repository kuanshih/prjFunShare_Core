﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class Age
    {
        public Age()
        {
            Product = new HashSet<Product>();
        }

        public int AgeId { get; set; }
        public string Grade { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}