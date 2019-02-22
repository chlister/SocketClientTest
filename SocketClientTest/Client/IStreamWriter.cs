using SocketClientTest.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClientTest.Client
{
    interface IStreamWriter
    {
        void WriteToStream(NetworkStream networkStream);
        void WriteToStream(NetworkStream networkStream, Message message);
    }
}
