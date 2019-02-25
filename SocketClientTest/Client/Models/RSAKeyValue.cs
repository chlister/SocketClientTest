using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client.Models
{
  public  class RSAKeyValue
    {
        private string _modulus, _exponent;

        public string Modulus { get => _modulus; set => _modulus = value; }
        public string Exponent { get => _exponent; set => _exponent = value; }

        public RSAKeyValue()
        {
        }

        public RSAKeyValue(string modulus, string exponent)
        {
            Modulus = modulus;
            Exponent = exponent;
        }
    }
}
