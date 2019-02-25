using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client.Models
{
    public class Message
    {
        public User To { get; set; }
        public User From { get; set; }
        public MessageBody Mb { get; set; }
        public List<User> Users { get; set; }

        public Message()
        {
            To = new User();
            From = new User();
            Mb = new MessageBody();
            Users = new List<User>();
        }

        public Message(User to, User from, List<User> users, MessageBody messageBody) 
        {
            To = to;
            From = from;
            Users = users;
            Mb = messageBody;
        }
    }
}
