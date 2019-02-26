using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client.Models
{
public    class EncryptionHandler
    {
        public User User { get; set; } // Contains the public key
        public byte[] EncryptionKey { get; set; }  // Contains the negotiated key
        public byte[] EncryptionIV { get; set; } // Contains the negotiated IV


        public EncryptionHandler(User user)
        {
            User = user;   
        }
    }
}
