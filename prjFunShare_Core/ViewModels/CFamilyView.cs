namespace prjFunShare_Core.ViewModels
{
    public class CFamilyView
    {
        public int MemberId { get; set; }        
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public byte[] Photo { get; set; }
        public int Level { get; set; }

    }
}
