using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;

namespace SocketClientTest.Client.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Ip { get; set; }

        //public RSAKeyValue RSAKeyValue { get; set; }
    }
}
