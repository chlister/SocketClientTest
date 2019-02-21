using SocketClientTest.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocketClientTest.Client
{
    public class SimpelSocketClient
    {
        List<string> messageBuffer;
        string mesReceived = string.Empty;
        List<object> result = new List<object>();

        //IPHostEntry ipHost;
        //IPAddress myIp;
        public int Port { get; set; }
        public string ServerIp { get; set; }
        public TcpClient Master { get; set; }
        private Task _reader;
        private Task _writer;

        private SimpelSocketClient()
        {
            //ipHost = Dns.GetHostEntry(Dns.GetHostName());
            //myIp = ipHost.AddressList[2];
            messageBuffer = new List<string>();
        }

        /// <summary>
        /// Provides a client connection at a specified port and server endpoint
        /// </summary>
        /// <param name="tcpSocket">TcpClient for the socket connection</param>
        /// <param name="port">Port of the server endpoint</param>
        /// <param name="serverEndPoint">Ip address of the server endpoint</param>
        public SimpelSocketClient(TcpClient tcpSocket, int port, string serverEndPoint): base()
        {
            Master = tcpSocket;
            Port = port;
            ServerIp = serverEndPoint;
        }

        private void WriteToStream(NetworkStream ns)
        {
            while (Master.Connected)
            {
                bool sending = true;
                string mess;
                do
                {
                    // Define message
                    Console.Write("Inut message to send: ");
                    mess = Console.ReadLine();
                    sending = false;
                } while (sending);
                // Switch on commands
                if (mess.ToUpper() == "EXIT")
                {
                    ns.Dispose();
                    break;
                }
                else
                {
                    Console.WriteLine("Sending");
                    //mess = SendMessage(mess);
                    //var byteMess = Encoding.UTF8.GetBytes(mess);
                    //ns.Write(byteMess, 0, byteMess.Length);
                    // Send XML
                    //Console.ReadLine();
                    string oldMes = "";
                    for (int i = 0; i < 50000; i++)
                    {
                        Thread.Sleep(500);
                         oldMes += "Hej Med dig! ";
                    SendXML(ns, oldMes);
                    }
                    //Console.WriteLine("Sending: " + mess);
                }
            }
            Console.WriteLine("Writer stopped");
        }

        private void SendXML(NetworkStream ns, string v)
        {
            XmlSerializer xmlSer;
            Message ms = new Message(new To(), new From(), new List<User>(), new MessageBody());
            ms.From.Ip = Dns.GetHostName();
            ms.From.Name = "Marc";
            ms.To.Name = "AnyOne";
            ms.To.Ip = "192.168.1.13";
            ms.Mb.Body = String.Format("Get this {0}!", v);
            xmlSer = new XmlSerializer(typeof(Message));
            xmlSer.Serialize(ns, ms);
        }

        public void StartClient()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), Port);
            Console.WriteLine("This is the client...");
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                Master.Connect(ServerIp, Port);
                Console.WriteLine("Connected to: {0}", ServerIp);
                //ns = Master.GetStream();
                _reader = new Task(() => ReadStream(Master.GetStream()));
                _writer = new Task(() => WriteToStream(Master.GetStream()));
                _reader.Start();
                //_writer.Start();
                //reader.Start();
                //writer.Start();
                WriteToStream(Master.GetStream());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Master.Close();
            }
        }

        private string SendMessage(string message, string reciever = "ZBC", string ip = "192.168.1.6")
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress myIp = ipHost.AddressList[2];
            // nickname:ip
            string myNickname = Environment.MachineName;
            return myNickname + ":" + myIp + ":" + reciever + ":" + ip + ":" + message + Environment.NewLine;

        }

        private void SendXML()
        {
            XmlSerializer xmlSer;
            Message ms = new Message(new To(), new From(), new List<User>(), new MessageBody());
            ms.From.Ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[2].ToString();
            ms.From.Name = "Marc";
            ms.To.Name = "AnyOne";
            ms.To.Ip = "192.168.1.13";
            ms.Mb.Body = "Hello from XML";
            xmlSer = new XmlSerializer(typeof(Message));
            xmlSer.Serialize(Master.GetStream(), ms);
        }


        private void ReadStream(NetworkStream ns)
        {
            Thread.Sleep(500); // So that the client can properly connect before reading
            while (Master.Connected)
            {
                byte[] buffer;
                try
                {
                    if (ns.CanRead)
                    {
                        buffer = new byte[Master.ReceiveBufferSize];

                        var bytesRead = ns.Read(buffer, 0, buffer.Length);
                        ReadResponse(bytesRead, buffer);
                        //Console.WriteLine("Server response: {0}", Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Argument error");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.InnerException);
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("Server has been closed");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.InnerException);

                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Server state invalid");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.InnerException);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Some other thing");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.Source);
                    Console.WriteLine(e.InnerException);

                }
            }
            // Connection has stopped - Thread needs to be stopped
            try
            {
                Console.WriteLine("Reader stopped...");
                Thread.CurrentThread.Abort();

            }
            catch (Exception e)
            {
                Console.WriteLine("Thread was aborted with an error:");
                Console.WriteLine(e.StackTrace);
            }
        }

        private void ReadResponse(int bytesRead, byte[] buffer)
        {

            //string mess = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            mesReceived += Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Number of bytes received: " + bytesRead);
            if (mesReceived.Contains("<Message") && mesReceived.Contains("</Message"))
            {
                string[] xmls = mesReceived.Split(new string[] { "</message>" }, StringSplitOptions.None);
                Console.WriteLine("Message splitted");
                mesReceived = string.Empty;
                foreach (var item in xmls)
                {
                    if (item.Contains("</Message>"))
                    {
                        messageBuffer.Add(item);
                    }
                    else
                        mesReceived += item;
                }
            }

            // GetBufferValues(messageBuffer);


            // Tries to deserialize the objects in the buffer
            DeSerializeObject(messageBuffer);


            // If any has been deserialized the print it (Only message + from user ip)
            PrintDSMessage();

        }

        private void PrintDSMessage()
        {
            if (result.Count > 0)
            {
                foreach (Message item in result)
                {
                    try
                    {
                        if (item is Message)
                            Console.WriteLine("Message recieve from {0}. {1}", item.From.Ip, item.Mb.Body);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.StackTrace);
                    }
                }
                result.Clear();
            }
        }

        private void DeSerializeObject(List<string> messageBuffer)
        {
            foreach (var item in messageBuffer)
            {
                XmlSerializer de = new XmlSerializer(typeof(Message));
                using (TextReader reader = new StringReader(item))
                {
                    try
                    {
                        result.Add(de.Deserialize(reader));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't deserialize object");
                        //Console.WriteLine(e.Message);
                        //Console.WriteLine(e.StackTrace);
                    }
                }
            }
            messageBuffer.Clear();
        }

        private void GetBufferValues(List<string> messageBuffer)
        {
            foreach (var item in messageBuffer)
            {
                Console.WriteLine("Split message:");
                Console.WriteLine(item);
                Console.WriteLine("Split message end");
            }
        }
    }
}
