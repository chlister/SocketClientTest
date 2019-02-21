using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClientTest.Client
{
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    public class SimpelAsyncClient
    {
        public static String response = String.Empty;


        // ManualResetEvent instances signal completion.  
        public static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        public static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        public static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public static void Connect(EndPoint remoteEP, Socket client)
        {
            client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);

            connectDone.WaitOne();
        }

        public static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception)
            {
                Console.WriteLine("Error!");
            }
        }

        public static void Send(Socket client, String data)
        {
            // Add message convention
            data = AddMessagePadding(data);
            Console.WriteLine(data);

            // Convert to byte data using UTF8 Encoding
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), client);
        }

        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device
                int bytesSendt = client.EndSend(ar);
                Console.WriteLine("Send {0} bytes to the server.", bytesSendt);

                // Signal that all bytes have been sent
                sendDone.Set();
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR!");
            }
        }

        public static void Receive(Socket client)
        {
            try
            {
                // Create the state object
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin recieving the data from the remote
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception)
            {
                Console.WriteLine("3RR0R");
            }
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrive the state object and the client socket
                // from the asyncronous state object
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    // There might be more date, so store the data recieved so far
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    // Get the rest of the data
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received
                    receiveDone.Set();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("35505");
                throw;
            }
        }

        /// <summary>
        /// Adds the padding required for the server to accept the message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="reciever"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static string AddMessagePadding(string message, string reciever = "ZBC", string ip = "192.168.1.6")
        {
            // nickname:ip
            string myNickname = Environment.MachineName;
            return myNickname + ":" + ip + ":" + reciever + ":" + ip + ":" + message + "\n";
        }
    }
}
