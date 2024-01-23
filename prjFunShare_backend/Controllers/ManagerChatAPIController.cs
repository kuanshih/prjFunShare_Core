using Microsoft.AspNetCore.Mvc;
using prjFunShare_backend.Models;
using System.Text.Json;

namespace prjFunShare_backend.Controllers
{
    public class ManagerChatAPIController : Controller
    {
        private readonly FUNShareContext _context;
        public ManagerChatAPIController(FUNShareContext c)
        { 
            _context = c;
        }
        public IActionResult UserList()//列出聊天對象清單
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER)) //判斷是否有登入
            {
                string userDataJson = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                CLoginSupplier loggedInUser = JsonSerializer.Deserialize<CLoginSupplier>(userDataJson);


                var orderMessages = _context.Chat
                                .OrderByDescending(m => m.MessageCreateTime) // 按照時間戳排序，最新的在前面
                                .ToList();

                var users = orderMessages
                          .Where(m => m.ChatMessengerId == loggedInUser.SupplierId || m.ReceiverId == loggedInUser.SupplierId)
                          .OrderByDescending(m => m.MessageCreateTime)
                          .Select(m => m.ChatMessengerId == loggedInUser.SupplierId ? m.ReceiverId : m.ChatMessengerId)
                          .Distinct().ToList();

                //JOIN參考資料 https://ithelp.ithome.com.tw/articles/10196394?sc=iThelpR
                //https://www.tutorialsteacher.com/linq/linq-joining-operator-join
                //A.Join(B,a=>a,b=>b,(a,b)=>new{...}) a=>a  b => b 這邊是兩個表相同的欄位全選(這邊是id)
                //(a, b)=>new { ...}  這邊是選擇兩邊的交集，再從中挑出我想要的資訊來
                var usersInfo = users
                                .Join(_context.Supplier, a => a, member => member.SupplierId, (a, member) => new
                                {
                                    member.SupplierId,
                                    member.SupplierName,
                                    member.LogoImage,
                                    LatestMessage = _context.Chat
                                                    .Where(chat =>
                                                           (chat.ChatMessengerId == loggedInUser.SupplierId && chat.ReceiverId == member.SupplierId) ||
                                                           (chat.ReceiverId == loggedInUser.SupplierId && chat.ChatMessengerId == member.SupplierId))
                                                    .OrderByDescending(chat => chat.MessageCreateTime)
                                                    .Select(chat => new
                                                    {
                                                        chat.MessageContent,
                                                        chat.MessageCreateTime
                                                    })
                                                    .FirstOrDefault()
                                }).ToList();
                return Json(usersInfo);
            }
            else
                return RedirectToAction("Index", "Home");
        }

        //列出與聊天對象內容
        public IActionResult ChatDetail(int chatWithId, int page = 1)
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_SUPPLIER)) //判斷是否有登入
            {
                string userDataJson = HttpContext.Session.GetString(CDictionary.SK_LOGINED_SUPPLIER);
                CLoginSupplier loggedInUser = JsonSerializer.Deserialize<CLoginSupplier>(userDataJson);

                double countTotal = _context.Chat.Where(p => p.ChatMessengerId == chatWithId || p.ReceiverId == chatWithId).Count();
                int perpage = 15;//每頁筆數
                int totalPage = (int)Math.Floor(countTotal / perpage) + 1;

                var chatInfo = _context.Chat
                             .Where(chat =>
                                        (chat.ChatMessengerId == loggedInUser.SupplierId && chat.ReceiverId == chatWithId) ||
                                        (chat.ReceiverId == loggedInUser.SupplierId && chat.ChatMessengerId == chatWithId))
                                        .OrderByDescending(chat => chat.MessageCreateTime)
                                        .Skip((page - 1) * perpage)
                                        .Take(perpage)
                                        .ToList();
                chatInfo = chatInfo.OrderBy(chat => chat.MessageCreateTime).ToList();
                return Json(chatInfo);
            }
            else
                return RedirectToAction("Index", "Home");
        }



    }
}

