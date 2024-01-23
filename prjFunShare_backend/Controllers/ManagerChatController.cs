using Microsoft.AspNetCore.Mvc;
using prjFunShare_backend.Models;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class ManagerChatController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerChatController(FUNShareContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER)) //判斷是否有登入
            {
                string userDataJson = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                CLoginSupplier loggedInUser = JsonSerializer.Deserialize<CLoginSupplier>(userDataJson);

                var ava = _context.Supplier.Where(p => p.SupplierId == loggedInUser.SupplierId).Select(p => p.LogoImage).FirstOrDefault();
                byte[] userAvatarBytes = ava;
                string userAvatarBase64 = Convert.ToBase64String(userAvatarBytes);
                string userAvatarUrl = $"data:image/png;base64,{userAvatarBase64}";
                ViewBag.currentLoginAvatarUrl = userAvatarUrl;
                ViewBag.currentLoginId = loggedInUser.SupplierId;
                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
