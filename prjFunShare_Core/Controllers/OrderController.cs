using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjFunShare_Core.Models;
using QRCoder;
using System.Drawing;

namespace prjFunShare_Core.Controllers
{
    public class OrderController : Controller
    {
        private readonly FUNShareContext _context;
        public OrderController(FUNShareContext context) 
        { 
            _context = context;
        }

        //傳入OrderDetail ID 產生QrCode
        public IActionResult getQrCodeImage(int? id)
        {
            if (id == null)
                return Content(null);

            OrderDetail od = _context.OrderDetail.Include(o=>o.Order).FirstOrDefault(x=>x.OrderDetailId== id);

            if (od == null)
                return Content(null);

            string txtForQrcode = $"OD{od.Order.OrderTime.Date.ToString("yyyyMMdd")}{od.OrderDetailId.ToString("0000")}";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtForQrcode, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            return File(qrCodeAsBitmapByteArr, "image/jpeg");
        }

        //掃QRcode更新報到狀態
        public IActionResult scanQrCode(int productDetailId,string qrcode)
        {
            if(qrcode == null)
                return Content("請重掃");

            int year = Convert.ToInt32(qrcode.Substring(2, 4));
            int month = Convert.ToInt32(qrcode.Substring(6, 2));
            int day = Convert.ToInt32(qrcode.Substring(8, 2));
            DateTime date = new DateTime(year, month, day);

            int id = Convert.ToInt32(qrcode.Substring(10, 4).TrimStart('0'));

            var od = _context.OrderDetail
                .Include(o => o.Order)
                .Include(o=>o.Member)
                .Where(x => x.OrderDetailId == id && x.Order.OrderTime.Date == date.Date).FirstOrDefault();

            if (od != null)
            {
                if (od.IsAttend == null && od.ProductDetailId== productDetailId)
                {               
                    od.IsAttend = true;
                    _context.SaveChanges();
                    return Content($"{od.Member.Name}，報到成功!");
                }
                else if(od.IsAttend != null && od.ProductDetailId == productDetailId)
                {
                    return Content("此票號已有報到記錄");
                }
                else
                {
                    return Content("票號不屬於此課程，請確認");
                }
            }
            else
            {
                return Content("查無此票號");
            }
        }
    }
}
