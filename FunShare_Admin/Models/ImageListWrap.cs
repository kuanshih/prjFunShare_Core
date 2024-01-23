using System.ComponentModel;

namespace FunShare_Admin.Models
{
    public class ImageListWrap
    {
        //private readonly long _fileSizeLimit;
        private ImageList _ImageList;
        public ImageList ImageList
        {
            get { return _ImageList; }
            set { _ImageList = value; }
        }
        public ImageListWrap(IConfiguration config)
        {
            _ImageList = new ImageList();
            //_fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        }


        public int ImageId {
            get { return _ImageList.ImageId; }
            set { _ImageList.ImageId = value; }        
        }
        public int ProductId {
            get { return _ImageList.ProductId; }
            set { _ImageList.ProductId = value; }
        }
        [DisplayName("活動花絮圖片群")]
        public byte[] Images {
            get { return _ImageList.Images; }
            set { _ImageList.Images = value; }
        }
        [DisplayName("橫幅圖片路徑")]
        public string ImagePath {
            get { return _ImageList.ImagePath; }
            set { _ImageList.ImagePath = value; }
        }
        [DisplayName("是橫幅")]
        public bool? IsMain {
            get { return _ImageList.IsMain; }
            set { _ImageList.IsMain = value; }
        }

        public virtual Product Product { get; set; }

        public IFormFile photo { get; set; }
    }
}
