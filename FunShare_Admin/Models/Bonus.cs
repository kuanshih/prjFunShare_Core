﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class Bonus
    {
        public int PointsId { get; set; }
        public int? MemberId { get; set; }
        public int? Points { get; set; }
        public int? OrderId { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual CustomerInfomation Member { get; set; }
        public virtual Order Order { get; set; }
    }
}