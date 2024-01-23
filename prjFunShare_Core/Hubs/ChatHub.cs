using Microsoft.AspNetCore.Http;
using prjFunShare_Core.Models;
using prjFunShare_Core.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System;
using System.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace prjFunShare_Core.Hubs
{
    public class ChatHub: Hub
    {
        private readonly FUNShareContext _context;
        private readonly HttpContext httpContext;
        private static List<CChatUserInfoViewModel> userConnections = new List<CChatUserInfoViewModel>();

        public ChatHub(FUNShareContext context)
        {
            _context = context;
        }
        
        //連線找到使用者ID 如果沒有新增
        public async Task UpdateUserInfo(string senderId, string vconnectionId)
        {
            
            // 獲取使用者的 accountId 和 ConnectionId
            string accountId = senderId;
            string connectionId = Context.ConnectionId;
            CChatUserInfoViewModel existingUserInfo = userConnections.FirstOrDefault(u => u.AccountId == accountId);
            if (existingUserInfo != null)
            {
                // 更新現有的 ConnectionId
                existingUserInfo.ConnectionId = connectionId;
            }
            else
            {
                // 新增新的使用者資訊
                CChatUserInfoViewModel userInfo = new CChatUserInfoViewModel
                {
                    AccountId = accountId,
                    ConnectionId = connectionId
                };
                userConnections.Add(userInfo);
            }
            await Clients.All.SendAsync("UpdateUserInfo", accountId, connectionId);
        }
        //找到收件者UserConnID
        private string getUserConnId(string receiverId)
        {
            var user = userConnections.Find(u => u.AccountId == receiverId);
            if (!string.IsNullOrEmpty(user.ConnectionId))
            {
                return user.ConnectionId;
            }
            else { return "No Find."; }
        }
        //傳送私人訊息
        public async Task SendPrivateMessage(string senderId, string receiverId, string message)
        {
            string roomid = "";
            string senderid = "";
            string receiverid = "";
            int customerid = 0;
            int supplierid = 0;
            bool who = false;
            //判別聊天室ID   //目前都是C 等到真的是供應商時再改
            var chat = from i in _context.Chat
                       .Where(u => ("S"+u.SupplierId.ToString() == senderId && "C"+u.CustomerId.ToString() == receiverId) || ("C"+u.CustomerId.ToString() == senderId && "S" + u.SupplierId.ToString() == receiverId))
                       select i;
            if (!chat.IsNullOrEmpty())
            {
                roomid = chat.First().ChatRoomId;
            }
            else
            {
                //roomid = Guid.NewGuid().ToString("N");
                //roomid = roomid.Substring(0, 5);
                var guidstring = Guid.NewGuid().ToString("N");
                var getNumbers = (from t in guidstring
                                  where char.IsDigit(t)
                                  select t).ToArray();
                roomid = new string(getNumbers).Substring(0, 5);
            }
            //判斷是客戶還是供應商
            if (senderId.FirstOrDefault()=='C')
            {
                senderid = senderId.Substring(1);
                customerid = Convert.ToInt32(senderid);
                who = true;
            }
            else
            {
                senderid = senderId.Substring(1);
                supplierid = Convert.ToInt32(senderid);
                who = false;
            }
            if (receiverId.FirstOrDefault() == 'S')
            {
                receiverid = receiverId.Substring(1);
                supplierid = Convert.ToInt32(receiverid);
            }
            else
            {
                receiverid = receiverId.Substring(1);
                customerid = Convert.ToInt32(receiverid);
            }
      

           

            //先存入資料庫
            var chatMessage = new Chat
            {
                ChatRoomId = roomid,
                CustomerId = customerid,
                SupplierId = supplierid,
                MessageContent = message,
                MessageCreateTime = DateTime.Now,
                WhoSend = who
            };
            _context.Chat.Add(chatMessage);
            await _context.SaveChangesAsync();

            // 將訊息傳送給特定的使用者
            string connectionId = getUserConnId(receiverId);
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message, receiverId);

        }



    }
}
//update chat 新增chat 資料庫欄位(whoSend)
//待完成:
//1.是否第一次傳訊息
//2.進入聊天室按鈕
//3.訊息時間
//4.count