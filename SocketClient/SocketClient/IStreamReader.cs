using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.SocketClient
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
