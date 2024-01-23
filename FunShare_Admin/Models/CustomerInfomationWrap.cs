using FunShare_Admin.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FunShare_Admin.Models
{
    public class CustomerInfomationWrap
    {
        private CustomerInfomation _custInfo;
        public CustomerInfomation CustomerInfomation
        {
            get { return _custInfo; }
            set { _custInfo = value; }
        }
        public CustomerInfomationWrap()
        {
            _custInfo = new CustomerInfomation();
        }
        [DisplayName("會員編號")]
        public int MemberId
        {
            get { return _custInfo.MemberId; }
            set { _custInfo.MemberId = value; }
        }
        [DisplayName("身分字號")]
        [Required(ErrorMessage ="此為必填項目。")]
        [RegularExpression(@"^[A-Za-z]{1}[1-2]{1}[0-9]{8}$")]
        public string ResidentId
        {
            get { return _custInfo.ResidentId; }
            set { _custInfo.ResidentId = value; }
        }
        [DisplayName("家長編號")]
        public int? ParentId
        {
            get
            {
                return _custInfo.ParentId;
            }
            set
            {
                _custInfo.ParentId = value;
            }
        }
        [DisplayName("會員姓名")]
        [Required(ErrorMessage = "此為必填項目。")]
        public string Name
        {
            get { return _custInfo.Name; }
            set { _custInfo.Name = value; }
        }
        [DisplayName("性別")]
        [Required(ErrorMessage = "此為必填項目。")]
        [RegularExpression(@"^[MFS]$")]
        public string Gender
        {
            get { return _custInfo.Gender; }
            set { _custInfo.Gender = value; }
        }
        [DisplayName("電話")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber
        {
            get { return _custInfo.PhoneNumber; }
            set { _custInfo.PhoneNumber = value; }
        }
        [DisplayName("信箱")]
        [Required(ErrorMessage = "此為必填項目。")]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get { return _custInfo.Email; }
            set { _custInfo.Email = value; }
        }
        [DisplayName("行政區編號")]
        public int? DistrictId
        {
            get { return _custInfo.DistrictId; }
            set { _custInfo.DistrictId = value; }
        }

        [DisplayName("地址")]
        public string? Address
        {
            get { return _custInfo.Address; }
            set { _custInfo.Address = value; }
        }
        [DisplayName("生日")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "此為必填項目。")]
        public DateTime? BirthDate
        {
            get { return _custInfo.BirthDate; }
            set { _custInfo.BirthDate = value; }
        }
        [DisplayName("匿名")]
        public string? Nickname
        {
            get { return _custInfo.Nickname; }
            set { _custInfo.Nickname = value; }
        }
        [DisplayName("密碼")]
        [Required(ErrorMessage = "此為必填項目。")]
        [DataType(DataType.Password)]
        public string Password
        {
            get { return _custInfo.Password; }
            set { _custInfo.Password = value; }
        }
        [DisplayName("過敏")]

        public bool? IsAllergy
        {
            get { return _custInfo.IsAllergy; }
            set { _custInfo.IsAllergy = value; }
        }
        [DisplayName("過敏敘述")]
        public string? AllergyDescription
        {
            get { return _custInfo.AllergyDescription; }
            set { _custInfo.AllergyDescription = value; }
        }
        [DisplayName("備註")]
        public string? Note
        {
            get { return _custInfo.Note; }
            set { _custInfo.Note = value; }
        }
        [DisplayName("狀態編號")]
        [Range(1, 3)]
        public int? StatusId
        {
            get { return _custInfo.StatusId; }
            set { _custInfo.StatusId = value; }
        }

        [DisplayName("照片")]
        public byte[]? Photo
        {
            get { return _custInfo.Photo; }
            set { _custInfo.Photo = value; }
        }
        [DisplayName("註冊日")]
        [DataType(DataType.Date)]
        public DateTime? RegistrationDay
        {
            get { return _custInfo.RegistrationDay; }
            set { _custInfo.RegistrationDay = value; }
        }
        [DisplayName("停權原因")]       
        public string? SuspensionReason
        {
            get { return _custInfo.SuspensionReason; }
            set { _custInfo.SuspensionReason = value; }
        }
        [DisplayName("紅利")]
        public virtual ICollection<Bonus> Bonus { get; set; } = new List<Bonus>();
        [DisplayName("評論")]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        [DisplayName("行政區")]
        public virtual District? Disctrict
        {
            get
            {
                //FUNShareContext db = new FUNShareContext();
                //District s = db.District.Find(_custInfo.DisctrictId);
                //_custInfo.Disctrict = s;
                return _custInfo.District;
            }
            set
            {
                _custInfo.District = value;
            }
        }

        public virtual ICollection<Interest> Interests { get; set; } = new List<Interest>();

        public virtual ICollection<CustomerInfomation> InverseParent { get; set; } = new List<CustomerInfomation>();

        public virtual ICollection<MemberAchievement> MemberAchievements { get; set; } = new List<MemberAchievement>();

        public virtual ICollection<MemberCoupon> MemberCoupons { get; set; } = new List<MemberCoupon>();

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [DisplayName("家長")]
        public virtual CustomerInfomation? Parent
        {
            get
            {
                //FUNShareContext db = new FUNShareContext();
                //CustomerInfomation s = db.CustomerInfomation.Find(_custInfo.ParentId);
                //_custInfo.Parent = s;
                return _custInfo.Parent;
            }
            set
            {
                _custInfo.Parent = value;
            }
        }

        public virtual ICollection<PocketList> PocketLists { get; set; } = new List<PocketList>();

        [DisplayName("狀態")]

        public virtual Status Status
        {
            get
            {
                //FUNShareContext db = new FUNShareContext();
                //Status s = db.Status.Find(_custInfo.StatusId);
                //_custInfo.Status = s;
                return _custInfo.Status;
            }
            set { _custInfo.Status = value; }
        }

        public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    }
}
