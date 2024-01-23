﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            Survey = new HashSet<Survey>();
        }

        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductDetailId { get; set; }
        public int MemberId { get; set; }
        public int StatusId { get; set; }
        public int? Grade { get; set; }
        public decimal? Amount { get; set; }
        public bool? IsAttend { get; set; }

        public virtual CustomerInfomation Member { get; set; }
        public virtual Order Order { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Survey> Survey { get; set; }
    }
}