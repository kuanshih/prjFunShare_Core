﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class MemberCoupon
    {
        public int MemberCouponId { get; set; }
        public int? CouponId { get; set; }
        public int? MemberId { get; set; }
        public int? StatusId { get; set; }

        public virtual CouponList Coupon { get; set; }
        public virtual CustomerInfomation Member { get; set; }
    }
}