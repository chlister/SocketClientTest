using SocketClientTest.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.SocketClient
{
    interface IStreamWriter
    {
        /// <summary>
        /// Sending the message object over a given network stream
        /// </summary>
        /// <param name="networkStream"><i>NetworkStream</i> of a Socket</param>
        /// <param name="message"><i>Message</i> object containing who the message is intended and the message</param>
        void WriteToStream(NetworkStream networkStream, Message message);
    }
}
