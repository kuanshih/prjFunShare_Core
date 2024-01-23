using Azure.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using NuGet.Protocol.Plugins;
using prjFunShare_backend.Models;
namespace prjFunShare_backend.Hubs
{
    public class ChatHub : Hub
    {
        private static List<CChatUserInfo> userConnections = new List<CChatUserInfo>();
        private readonly FUNShareContext _context;
        public ChatHub(FUNShareContext c)
        {
            _context = c;
        }
        //訊息
        public async Task SendPrivateMessage(int senderId, int receiverId, string message)
        {
            //存入資料庫
            var chatMessage = new Chat
            {
                ChatMessengerId = senderId,
                ReceiverId= receiverId,
                MessageContent= message,
                MessageCreateTime= DateTime.Now
            };
            _context.Chat.Add(chatMessage);
            await _context.SaveChangesAsync();
            // 將訊息傳送給特定的使用者
            string connectionId = getUserConnId(receiverId);
            // 如果接收者的 connectionId 不為 null，則傳送訊息\
            if (string.IsNullOrEmpty(connectionId))
            { 
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, receiverId, message, chatMessage.MessageCreateTime);
            }
        }
        private string? getUserConnId(int receiverId)
        {
            var user = userConnections.Find(c => c.AccountId == receiverId);
            if (user != null && !string.IsNullOrEmpty(user.ConnectionId))
            {
                return user.ConnectionId;
            }
            return null; // 如果找不到使用者或 ConnectionId 為空
        }
        //--------------------更新線上使用者清單--------------------
        public async Task UpdateUserInfo(int senderId, string vconnectionId)
        {
            // 獲取使用者的 accountId 和 ConnectionId
            int accountId = senderId;
            string connectionId = Context.ConnectionId;

            CChatUserInfo existingUserInfo = userConnections.FirstOrDefault(u => u.AccountId == accountId);
            if (existingUserInfo != null)
            {
                // 更新現有的 ConnectionId
                existingUserInfo.ConnectionId = connectionId;
            }
            else
            {
                // 新增新的使用者資訊
                CChatUserInfo userInfo = new CChatUserInfo
                {
                    AccountId = accountId,
                    ConnectionId = connectionId
                };
                userConnections.Add(userInfo);
            }

            await Clients.All.SendAsync("UpdateUserInfo", accountId, connectionId);
        }
    }
}
