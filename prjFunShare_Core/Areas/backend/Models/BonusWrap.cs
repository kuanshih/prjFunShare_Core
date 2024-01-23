using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using prjFunShare_Core.Models;

namespace prjFunShare_Core.Areas.backend.Models
{
    public class BonusWrap
    {
        private Bonus _bonus;
        public Bonus bonus
        {
            get { return _bonus; }
            set { _bonus = value; }
        }
        public BonusWrap()
        {
            _bonus = new Bonus();
        }
        [DisplayName("紅利點數")]
        public int PointsId
        {
            get { return _bonus.PointsId; }
            set { _bonus.PointsId = value; }
        }
        [DisplayName("會員編號")]
        public int? MemberId
        {
            get { return _bonus.MemberId; }
            set { _bonus.MemberId = value; }
        }
        [DisplayName("點數")]
        public int? Points
        {
            get { return _bonus.Points; }
            set { _bonus.Points = value; }
        }
        [DisplayName("訂單編號")]
        public int? OrderId
        {
            get { return _bonus.OrderId; }
            set { _bonus.OrderId = value; }
        }
        [DisplayName("截止日期"), DataType(DataType.Date)]

        public DateTime? EndDate
        {
            get { return _bonus.EndDate; }
            set { _bonus.EndDate = value; }
        }
        public virtual CustomerInfomation Member { get; set; }
        public virtual Order Order { get; set; }


    }
}
