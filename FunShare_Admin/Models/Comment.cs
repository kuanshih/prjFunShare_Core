﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public string Review { get; set; }
        public double? Rank { get; set; }
        public DateTime? Date { get; set; }
        public string ImagePath { get; set; }

        public virtual CustomerInfomation Member { get; set; }
        public virtual Order Order { get; set; }
    }
}