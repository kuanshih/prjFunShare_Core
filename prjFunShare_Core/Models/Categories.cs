﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjFunShare_Core.Models;

public partial class Categories
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string CategoryDescription { get; set; }

    public virtual ICollection<SubCategory> SubCategory { get; set; } = new List<SubCategory>();
}