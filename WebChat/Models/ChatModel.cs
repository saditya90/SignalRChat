namespace WebChat.Models
{
    public class ChatModel
    {
        public string Id { get; set; }
        public string RoomId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Status { get; set; }
        public string CurrentId { get; set; }
    }

    public class ChatSignaling
    {
        public string ConversationId { get; set; }
        public string RoomId { get; set; }
        public string UserToId { get; set; }
        public ChatModel UserFrom { get; set; }
    }

    public class MessageInfo
    {
        public string UserFromId { get; set; }
        public string UserToId { get; set; }
        public string RoomId { get; set; }
        public string ConversationId { get; set; }
        public string Message { get; set; }
        public string ClientGuid { get; set; }
    }
}