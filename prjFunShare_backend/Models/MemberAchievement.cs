﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjFunShare_backend.Models;

public partial class MemberAchievement
{
    public int MemberAchievementId { get; set; }

    public int MemberId { get; set; }

    public int AchievementId { get; set; }

    public virtual Achievement Achievement { get; set; }

    public virtual CustomerInfomation Member { get; set; }
}