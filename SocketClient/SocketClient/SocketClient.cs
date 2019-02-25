using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SocketClientTest.Client.Models;

namespace SocketClient.SocketClient
{
    public class SocketClient : IStreamReader, IStreamWriter
    {
        public TcpClient Client { get; private set; }
        public int Port { get; private set; }
        public string ServerEndpoint { get; private set; }



        /// <summary>
        /// Connects the client to the specified server endpoint and port
        /// </summary>
        public void StartClient()
        {
            try
            {
                Client.Connect(IPAddress.Parse(ServerEndpoint), Port);
                Console.WriteLine("Client connected to {0}, on port num: {1}", 
                    ServerEndpoint, Port);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting");
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
            }

        }

        public void WriteToStream(NetworkStream networkStream, Message message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a given networkstream
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        public byte[] ReadStream(NetworkStream networkStream)
        {
            // Define return

            // Only read if client connected
            if (Client.Connected)
            {

            }
            return null;
        }
    }
}
