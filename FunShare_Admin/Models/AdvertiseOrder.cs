﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class AdvertiseOrder
    {
        public int AdOrderId { get; set; }
        public int PackageId { get; set; }
        public int SupplierId { get; set; }

        public virtual AdvertiseProduct Package { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}