using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client.Models
{
    public class Message
    {
        [OptionalField]
        private User _to;
        [OptionalField]
        private User _from;
        [OptionalField]
        private MessageBody _mb;
        [OptionalField]
        private string key;
        [OptionalField]
        private string iv;

        public User To { get => _to; set => _to = value; }
        public User From { get => _from; set => _from = value; }
        public MessageBody Mb { get => _mb; set => _mb = value; }
        public List<User> Users { get; set; }
        public string Iv { get => iv; set => iv = value; }
        public string Key { get => key; set => key = value; }

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
