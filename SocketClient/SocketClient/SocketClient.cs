using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SocketClientTest.Client.Models;

namespace SocketClient.SocketClient
{
    public class SocketClient : IStreamReader, IStreamWriter
    {
        public TcpClient Client { get; private set; }
        public int Port { get; private set; }
        public string ServerEndpoint { get; private set; }

        public SocketClient(TcpClient client, int port, string serverEndpont)
        {
            Client = client;
            Port = port;
            ServerEndpoint = serverEndpont;
            Client.ReceiveBufferSize = 32000;
        }

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
        
        /// <summary>
        /// Sending the message object over a given network stream
        /// </summary>
        /// <param name="networkStream"><i>NetworkStream</i> of a Socket</param>
        /// <param name="message"><i>Message</i> object containing who the message is intended and the message</param>
        public void WriteToStream(NetworkStream networkStream, Message message)
        {
            if (Client.Connected)
            {
                XmlSerializer se = new XmlSerializer(typeof(Message));
                se.Serialize(networkStream, message);
            }
        }

        /// <summary>
        /// Reads a given networkstream
        /// </summary>
        /// <param name="networkStream">A NetworkStream from a TcpClient</param>
        /// <returns><i>byte[]</i> read from the NetworkStream</returns>
        public byte[] ReadStream(NetworkStream networkStream)
        {
            // Define return
            byte[] buffer = null;

            // Only read if client connected
            if (Client.Connected)
            {
                if (networkStream.CanRead)
                {
                    buffer = new byte[Client.ReceiveBufferSize];
                    var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        // Only return bytes if there is any to read
                        return buffer;
                    }
                }
            }
            return buffer;
        }

        /// <summary>
        /// Sending bytes over the NetworkStream
        /// </summary>
        /// <param name="networkStream"></param>
        public void WriteToStream(NetworkStream networkStream, byte[] message)
        {
            if (Client.Connected)
            {
                networkStream.Write(message, 0, message.Length);
            }
        }
    }
}