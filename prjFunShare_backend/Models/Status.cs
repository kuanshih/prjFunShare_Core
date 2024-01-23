﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjFunShare_backend.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string Description { get; set; }

    public string StatusType { get; set; }

    public virtual ICollection<CustomerInfomation> CustomerInfomation { get; set; } = new List<CustomerInfomation>();

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();

    public virtual ICollection<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();

    public virtual ICollection<Supplier> Supplier { get; set; } = new List<Supplier>();
}