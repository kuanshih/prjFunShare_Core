﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace FunShare_Admin.Models
{
    public partial class Achievement
    {
        public Achievement()
        {
            AchievementList = new HashSet<AchievementList>();
            MemberAchievement = new HashSet<MemberAchievement>();
        }

        public int AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementFileName { get; set; }

        public virtual ICollection<AchievementList> AchievementList { get; set; }
        public virtual ICollection<MemberAchievement> MemberAchievement { get; set; }
    }
}