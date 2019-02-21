using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client.Models
{
    public class Message
    {
        public To To { get; set; }
        public From From { get; set; }
        public MessageBody Mb { get; set; }
        public List<User> Users { get; set; }

        public Message()
        {
            To = new To();
            From = new From();
            Mb = new MessageBody();
            Users = new List<User>();
        }

        public Message(To to, From from, List<User> users, MessageBody messageBody)
        {
            To = to;
            From = from;
            Users = users;
            Mb = messageBody;
        }
    }
}
