using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client
{
    interface IStreamReader
    {
        /// <summary>
        /// Read data from a networkstream
        /// </summary>
        /// <param name="networkStream"></param>
        byte[] ReadStream(NetworkStream networkStream);
    }
}
