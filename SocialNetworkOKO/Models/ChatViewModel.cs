using System.Data;

namespace SocialNetworkOKO.Models
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            NewMessage = new Message();
        }

        public User You { get; set; }

        public User ToWhom { get; set; }

        public List<Message> History { get; set; }

        public Message NewMessage { get; set; }
    }
}
