using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Xml;
using System.Runtime.Serialization;

namespace SocketClientTest.Client.Models
{
    public class User
    {
        [OptionalField]
        private string name;
        //[OptionalField]
        //private RSAKeyValue rSAKeyValue;

        public string Name { get => name; set => name = value; }

        public string Ip { get; set; }


        //public RSAKeyValue R½SAKeyValue
        //{
        //    get => rSAKeyValue; set => rSAKeyValue = value;
        //}

        public User() { }

        //public User(string name, RSAKeyValue rSAKeyValue, string ip)
        //{
        //    this.name = name;
        //    //this.rSAKeyValue = rSAKeyValue;
        //    Ip = ip;
        //}

        public User(string name, string ip)
        {
            this.name = name;
            Ip = ip;
        }
    }
}
