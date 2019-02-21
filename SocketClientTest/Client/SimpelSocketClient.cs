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
        List<String> messageBuffer;
        string mesReceived = String.Empty;
        List<object> result = new List<object>();

        IPHostEntry ipHost;
        IPAddress myIp;
        readonly int port = 8889;
        readonly string serverIp = "192.168.1.2";
        TcpClient master;
        NetworkStream ns;
        StreamReader _sr;
        Task _reader;
        Task _writer;

        public SimpelSocketClient()
        {
            ipHost = Dns.GetHostEntry(Dns.GetHostName());
            myIp = ipHost.AddressList[2];
            _reader = new Task(() => ReadStream());
            _writer = new Task(() => WriteToStream());
            messageBuffer = new List<string>();
            StartClient();
        }

        public SimpelSocketClient(Socket socket, int port, IPAddress serverEndPoint)
        {
            ½½
        }

        private void WriteToStream()
        {
            while (master.Connected)
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
                    SendXML(oldMes);
                    }
                    //Console.WriteLine("Sending: " + mess);
                }
            }
            Console.WriteLine("Writer stopped");
        }

        private void SendXML(string v)
        {
            XmlSerializer xmlSer;
            Message ms = new Message(new To(), new From(), new List<User>(), new MessageBody());
            ms.From.Ip = myIp.ToString();
            ms.From.Name = "Marc";
            ms.To.Name = "AnyOne";
            ms.To.Ip = "192.168.1.13";
            ms.Mb.Body = String.Format("Get this {0}!", v);
            xmlSer = new XmlSerializer(typeof(Message));
            xmlSer.Serialize(ns, ms);
        }

        public void StartClient()
        {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            Console.WriteLine("This is the client...");
            // Create a Tcp/ip listener
            master = new TcpClient();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                master.Connect(serverIp, port);
                Console.WriteLine("Connected to: {0}", serverIp);
                ns = master.GetStream();
                //master.Client.ReceiveBufferSize = 32000;
                //master.Client.NoDelay = false;
                _sr = new StreamReader(ns, Encoding.UTF8);
                _reader.Start();
                //_writer.Start();
                //reader.Start();
                //writer.Start();
                WriteToStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                master.Close();
            }
        }

        private string SendMessage(string message, string reciever = "ZBC", string ip = "192.168.1.6")
        {
            // nickname:ip
            string myNickname = Environment.MachineName;
            return myNickname + ":" + myIp + ":" + reciever + ":" + ip + ":" + message + Environment.NewLine;

        }

        private void SendXML()
        {
            XmlSerializer xmlSer;
            Message ms = new Message(new To(), new From(), new List<User>(), new MessageBody());
            ms.From.Ip = myIp.ToString();
            ms.From.Name = "Marc";
            ms.To.Name = "AnyOne";
            ms.To.Ip = "192.168.1.13";
            ms.Mb.Body = "Hello from XML";
            xmlSer = new XmlSerializer(typeof(Message));
            xmlSer.Serialize(ns, ms);
        }


        private void ReadStream()
        {
            Thread.Sleep(500);
            while (master.Connected)
            {
                byte[] buffer;
                try
                {
                    if (ns.CanRead)
                    {
                        buffer = new byte[master.ReceiveBufferSize];

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
                mesReceived = String.Empty;
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
