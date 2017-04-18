using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using WebChat.Models;
using System;
using System.IO;

namespace WebChat.Hooks
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultUserImage = "/Images/UsersImage/imgDefault.jpg";
        private const string RoomId = "1";
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetUserList(string roomId, string conversationId)
        {
            roomId = roomId ?? RoomId;
            conversationId = conversationId ?? string.Empty;
            //eleminate current user from available users
            var users = _context.Users.Where(q => q.UserName
                    != Context.User.Identity.Name).ToList();
            var chatUsers = (from user in users
                             select new ChatModel
                             {
                                 Email = user.Email,
                                 ProfilePictureUrl = (!string.IsNullOrEmpty(user.ImagePath) ?
                                 File.Exists(Context.Request.GetHttpContext().Server.MapPath(Path.Combine("~", user.ImagePath))) ?
                                 user.ImagePath : DefaultUserImage : DefaultUserImage),
                                 Id = user.UserName,
                                 Name = user.FirstName,
                                 RoomId = roomId,
                                 Status = (_connections.GetConnections(user.Email).Count() == 0 ? "0" : RoomId)
                             }).ToList();
            if (chatUsers.Count > 0)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(chatUsers);
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetUserInfo(string userId)
        {
            var users = _context.Users.ToList();
            //filter specific user from available users
            //Status will be shown as online and offline i.e for 0 offline will be consider 1 for online
            var chatUser = (from user in users
                            where user.UserName == userId
                            select new ChatModel
                            {
                                Email = user.Email,
                                ProfilePictureUrl = (!string.IsNullOrEmpty(user.ImagePath) ?
                                 File.Exists(Context.Request.GetHttpContext().Server.MapPath(Path.Combine("~", user.ImagePath))) ?
                                 user.ImagePath : DefaultUserImage : DefaultUserImage),
                                Id = user.UserName,
                                Name = user.FirstName,
                                RoomId = RoomId,
                                Status = (_connections.GetConnections(user.Email).Count() == 0 ? "0" : "1"),
                                CurrentId = Context.User.Identity.Name
                            }).FirstOrDefault();
            return Newtonsoft.Json.JsonConvert.SerializeObject(chatUser);
        }

        public string GetRoomsList()
        {
            return string.Empty;
        }

        public void SendMessage(string roomId, string conversationId, string otherUserId,
            string messageText, string clientGuid)
        {
            roomId = roomId ?? RoomId; var isSeen = true;
            conversationId = conversationId ?? string.Empty;
            var userId = Context.User.Identity.Name;
            var message = new MessageInfo
            { 
                ConversationId = conversationId,
                Message = messageText,
                RoomId = roomId,
                UserFromId = userId,
                UserToId = otherUserId
            };
            var host = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            isSeen = _connections.GetConnections(otherUserId).Count() > 0 ? true : false;
            if (_connections.GetConnections(otherUserId).Count() > 0)
                host.Clients.User(otherUserId).SendMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            _context.UsersChat.Add(new UsersChat
            {
                ClientGuidId = clientGuid,
                CreatedOn = DateTime.Now,
                FromUserId = otherUserId,
                OtherUserId = userId,
                IsSeen = isSeen,
                Message = messageText,
                RoomId = roomId
            }); _context.SaveChanges();
        }

        public void SendTypingSignal(string roomId, string conversationId, string userToId)
        {
            var userId = Context.User.Identity.Name;
            var users = _context.Users.ToList();
            var chatUser = (from user in users
                            where user.UserName == userId
                            select new ChatModel
                            {
                                Email = user.Email,
                                ProfilePictureUrl = (!string.IsNullOrEmpty(user.ImagePath) ?
                                 File.Exists(Context.Request.GetHttpContext().Server.MapPath(Path.Combine("~", user.ImagePath))) ?
                                 user.ImagePath : DefaultUserImage : DefaultUserImage),
                                Id = user.UserName,
                                Name = user.FirstName,
                                RoomId = RoomId,
                                Status = (_connections.GetConnections(user.Email).Count() == 0 ? "0" : "1")
                            }).FirstOrDefault();
            var signal = new ChatSignaling
            {
                ConversationId = null,
                RoomId = RoomId,
                UserToId = userToId,
                UserFrom = chatUser
            };
            var host = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            host.Clients.User(userToId).SendTypingSignal(Newtonsoft.Json.JsonConvert.SerializeObject(signal));
        }

        public string GetMessageHistory(string roomId,
            string conversationId, string otherUserId)
        {
            roomId = roomId ?? RoomId;
            var history = (from message in _context.UsersChat
                           where message.FromUserId == otherUserId ||
                           message.OtherUserId == otherUserId
                           orderby message.CreatedOn.Value
                           select new MessageInfo
                           {
                               ClientGuid = message.ClientGuidId,
                               ConversationId = conversationId,
                               Message = message.Message,
                               RoomId = roomId,
                               UserFromId = message.OtherUserId,
                               UserToId = message.FromUserId
                           }).ToList();

            return Newtonsoft.Json.JsonConvert.SerializeObject(history);
        }

        public void EnterRoom(string roomId)
        {
            var y = string.Empty;
        }

        public string LeaveRoom(string roomId)
        {
            return string.Empty;
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            //update user's list on client side for online
            UpdateConnectedUsers();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            //update user's list on client side for offline
            UpdateConnectedUsers("Diconnected");

            return base.OnDisconnected(stopCalled);
        }

        private void UpdateConnectedUsers(string onDisconnect = null)
        {
            var name = Context.User.Identity.Name;
            if (string.IsNullOrEmpty(onDisconnect) ?
                _connections.GetUsers().Count() > 1
                : _connections.GetUsers().Count() > 0)
            {
                foreach (var who in _connections.GetUsers().Where(q => !q.Contains(name)))
                {
                    var usersInfo = _context.Users.Where(q => !q.UserName.Contains(who)).ToList();
                    if (usersInfo.Count > 0)
                    { 
                        var chatUsersDetail = (from user in usersInfo
                                               select new ChatModel
                                               {
                                                   Email = user.Email,
                                                   ProfilePictureUrl = (!string.IsNullOrEmpty(user.ImagePath) ?
                                                   File.Exists(Context.Request.GetHttpContext().Server.MapPath(Path.Combine("~", user.ImagePath))) ?
                                                   user.ImagePath : DefaultUserImage : DefaultUserImage),
                                                   Id = user.UserName,
                                                   Name = user.FirstName,
                                                   RoomId = RoomId,
                                                   Status = (_connections.GetConnections(user.Email).Count() == 0 ? "0" : "1")
                                               }).ToList();
                        var host = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        host.Clients.User(who).UserListChanged(Newtonsoft.Json.JsonConvert.SerializeObject(chatUsersDetail));
                    }
                }
            }
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            //update user's list on client side for online
            UpdateConnectedUsers();

            return base.OnReconnected();
        }
    }
}