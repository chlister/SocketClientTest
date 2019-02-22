using SocketClientTest.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SocketClientTest.Client
{
    public class SimpelSocketClient
    {
        public List<string> MessageBuffer { get; set; }
        string mesReceived = string.Empty;
        List<object> result;
        private static readonly byte[] Key = Convert.FromBase64String("W+jcxfBJm37AAZujiktg4qCdy3k8D+vIrj4exFxFpIY=");
        public List<User> Users { get; private set; }

        public int Port { get; set; }
        public string ServerIp { get; private set; }
        public TcpClient Master { get; set; }
        private Task _reader;
        private Task _writer;

        private SimpelSocketClient()
        {
            //ipHost = Dns.GetHostEntry(Dns.GetHostName());
            //myIp = ipHost.AddressList[2];
            MessageBuffer = new List<string>();
            result = new List<object>();
        }

        /// <summary>
        /// Provides a client connection at a specified port and server endpoint
        /// </summary>
        /// <param name="tcpSocket">TcpClient for the socket connection</param>
        /// <param name="port">Port of the server endpoint</param>
        /// <param name="serverEndPoint">Ip address of the server endpoint</param>
        public SimpelSocketClient(TcpClient tcpSocket, int port, string serverEndPoint) : base()
        {
            Users = new List<User>();
            MessageBuffer = new List<string>();
            result = new List<object>();
            Master = tcpSocket;
            Port = port;
            ServerIp = serverEndPoint;
        }

        /// <summary>
        /// Write to a given networkstream
        /// </summary>
        /// <param name="ns"></param>
        private void WriteToStream(NetworkStream ns)
        {
            while (Master.Connected)
            {
                bool sending = true;
                string mess;
                string receiverIp;
                do
                {
                    // Define message
                    Console.Write("Inut message to send: ");
                    mess = Console.ReadLine();
                    Console.Write("Inut Ip to send to: ");
                    receiverIp = Console.ReadLine();
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
                    Console.WriteLine("Sending: {0}", mess);
                    SendXML(ns, mess, receiverIp);
                }
            }
            Console.WriteLine("Writer stopped");
        }

        /// <summary>
        /// Write a specified Message object to a NetworkStream
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="message"></param>
        public void WriteToStream(NetworkStream ns, Message message)
        {
            if (Master.Connected)
            {
                SendXML(ns, message);
            }
            else
            {
                throw new SocketException();
            }
        }

        /// <summary>
        /// Sends a given Message object over the NetworkStream
        /// </summary>
        /// <param name="ns">NetworkStream</param>
        /// <param name="message">Message Object</param>
        private void SendXML(NetworkStream ns, Message message)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Message));
            serializer.Serialize(ns,message);
        }

        private void SendXML(NetworkStream ns, string message, string recieverIp)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress myIp = ipHost.AddressList[2];
            XmlSerializer xmlSer;
            Message ms = new Message(new To(), new From(), new List<User>(), new MessageBody());
            ms.From.Ip = myIp.ToString();
            ms.From.Name = "Marc";
            ms.To.Name = "AnyOne";
            ms.To.Ip = recieverIp;

            ms.Mb.Body = EncryptMessage(Key, message);
            xmlSer = new XmlSerializer(typeof(Message));
            xmlSer.Serialize(ns, ms);
        }

        private string EncryptMessage(byte[] key, string message)
        {
            byte[] encrypted;
            using (Aes myAes = Aes.Create())
            {
                myAes.Padding = PaddingMode.PKCS7;
                myAes.KeySize = 128;
                myAes.IV = new byte[128 / 8];
                myAes.Key = Key;
                //byte[] messBytes = Encoding.UTF8.GetBytes(message);
                ICryptoTransform encr = myAes.CreateEncryptor(myAes.Key, myAes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encr, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(message);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        private string DecryptUsingAes(string body)
        {
            byte[] encrypted = Convert.FromBase64String(body);

            string plaintext = "";
            using (Aes myAes = Aes.Create())
            {
                myAes.Padding = PaddingMode.PKCS7;
                myAes.KeySize = 128;
                myAes.IV = new byte[128 / 8];
                myAes.Key = Key;
                //byte[] messBytes = Encoding.UTF8.GetBytes(message);
                ICryptoTransform encr = myAes.CreateDecryptor(myAes.Key, myAes.IV);

                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encr, CryptoStreamMode.Read))
                    {
                        using (StreamReader sw = new StreamReader(cs))
                        {
                            plaintext = sw.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
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
                _reader = new Task(() => ReadStream(Master.GetStream()));
                _writer = new Task(() => WriteToStream(Master.GetStream()));
                _reader.Start();
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
                        if (bytesRead > 0)
                            ReadResponse(bytesRead, buffer, MessageBuffer);
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

        private void ReadResponse(int bytesRead, byte[] buffer, List<string> messageBuffer)
        {

            //string mess = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            mesReceived += Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Number of bytes received: " + bytesRead);
            if (mesReceived.Contains("<Message") && mesReceived.Contains("</Message"))
            {
                string[] xmls = mesReceived.Split(new string[] { "</message>" }, StringSplitOptions.None);
                Console.WriteLine("Message splitted");
                Console.WriteLine(mesReceived);
                mesReceived = string.Empty;
                foreach (var item in xmls)
                {
                    try
                    {
                        if (item.Contains("</Message>"))
                        {
                            //if (!item.Contains("<RSAKeyValue"))
                            messageBuffer.Add(item);
                        }
                        else
                            mesReceived += item;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine(e.Message);
                    }
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
                        if (item.Mb.Body != null)
                            Console.WriteLine("Message recieve from {0}. {1}", item.From.Ip, DecryptUsingAes(item.Mb.Body));
                        if (item.Users.Count > 0)
                        {
                            for (int i = 0; i < item.Users.Count; i++)
                            {
                                var user = new User(item.Users[i].Name, item.Users[i].Ip);
                                if (!Users.Contains(user))
                                    Users.Add(new User(item.Users[i].Name, item.Users[i].Ip));
                            }
                        }
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
