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
using System.Linq;
using System.Xml.Serialization;

namespace SocketClientTest.Client
{
    public class SimpelSocketClient
    {
        Dictionary<User, string> UserAndKey = new Dictionary<User, string>();
        public List<string> MessageBuffer { get; set; }
        private static List<string> keyBuffer = new List<string>();
        string mesReceived = string.Empty;
        List<object> result;
        private static readonly byte[] Key = Convert.FromBase64String("W+jcxfBJm37AAZujiktg4qCdy3k8D+vIrj4exFxFpIY=");
        public List<User> UsersOnline { get; private set; }

        public int Port { get; set; }
        public string ServerIp { get; private set; }
        public TcpClient Master { get; set; }
        static RSACryptoServiceProvider myKeyRSA = new RSACryptoServiceProvider(2048);
        static RSAParameters myRSAKeyInfo;
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
            UsersOnline = new List<User>();
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

                    sending = false;
                } while (sending);
                // Switch on commands
                switch (mess.ToUpper())
                {
                    case "EXIT":
                        ns.Dispose();
                        break;
                    case "USERS":
                        Console.WriteLine("Users online: ");
                        foreach (var user in UsersOnline)
                        {
                            Console.WriteLine("Username: {0}. " +
                                Environment.NewLine + "IP: {1} \n " +
                                "PublicKey: {2}",
                                user.Name, user.Ip, user.RSAKeyValue.Modulus);
                        }
                        break;
                    default:
                        Console.Write("Inut Ip to send to: ");
                        receiverIp = Console.ReadLine();
                        Console.WriteLine("Sending: {0}", mess);
                        SendXML(ns, mess, receiverIp);
                        break;
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
            serializer.Serialize(ns, message);
        }

        private void SendXML(NetworkStream ns, string message, string recieverIp)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress myIp = ipHost.AddressList[2];
            XmlSerializer xmlSer;
            Message ms = new Message(new User(), new User(), new List<User>(), new MessageBody());
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

        /// <summary>
        /// Encrypt a message using a users public key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private string EncryptMessage(string message, User user)
        {
            byte[] encrypted;
            User savedUser = UsersOnline.SingleOrDefault(u => u.Ip == user.Ip);
            if (savedUser != null)
            {
                byte[] pubKey = Encoding.ASCII.GetBytes(savedUser.RSAKeyValue.Modulus); // TODO: Find if we have the user pub key

                using (Aes myAes = Aes.Create())
                {
                    myAes.Padding = PaddingMode.PKCS7;
                    myAes.KeySize = 128;
                    myAes.IV = new byte[128 / 8];
                    myAes.Key = pubKey;
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
            return null;
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
                SendPublicKey();
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

        /// <summary>
        /// Sends the public key to the server
        /// </summary>
        private void SendPublicKey()
        {
            Console.WriteLine("Send public key to server...");

            myRSAKeyInfo = myKeyRSA.ExportParameters(false);
            var netStream = Master.GetStream();
            if (netStream.CanWrite)
            {
                byte[] keySender = Encoding.ASCII.GetBytes(myKeyRSA.ToXmlString(false));
                netStream.Write(keySender, 0, keySender.Length);
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
            Message ms = new Message(new User(), new User(), new List<User>(), new MessageBody());
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
                Console.WriteLine(mesReceived);
                mesReceived = string.Empty;
                foreach (var item in xmls)
                {
                    try
                    {
                        if (item.Contains("</Message>"))
                        {
                            if (!item.Contains("<RSAKeyValue"))
                                MessageBuffer.Add(item);
                            else if (item.Contains("<RSAKeyValue"))
                                keyBuffer.Add(item);
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
            DeSerializeObject();

            // Update online users
            UpdateUsersOnline();

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
                        //if (item.Users.Count > 0)
                        //{
                        //    for (int i = 0; i < item.Users.Count; i++)
                        //    {
                        //        var user = new User(item.Users[i].Name, item.Users[i].Ip);
                        //        if (!UsersOnline.Contains(user))
                        //            UsersOnline.Add(new User(item.Users[i].Name, item.Users[i].Ip));
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.StackTrace);
                    }
                }
                result.Clear();
            }
        }


        private void DeSerializeObject()
        {
            foreach (var item in MessageBuffer)
            {
                try
                {
                    XmlSerializer de = new XmlSerializer(typeof(Message));
                    using (TextReader reader = new StringReader(item))
                    {
                        result.Add(de.Deserialize(reader));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't deserialize object");
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.StackTrace);
                }
            }
            MessageBuffer.Clear();
        }

        private void UpdateUsersOnline()
        {
            foreach (var item in keyBuffer)
            {
                try
                {
                    XmlSerializer de = new XmlSerializer(typeof(Message));
                    Message ms;
                    using (TextReader reader = new StringReader(item))
                    {
                        //result.Add(de.Deserialize(reader));
                        ms = (Message)de.Deserialize(reader);
                        if (ms.Users.Count > 0)
                        {
                            List<User> msUsers = ms.Users;
                            foreach (var user in msUsers)
                            {
                                bool exists = false;

                                for (int i = 0; i < UsersOnline.Count; i++)
                                {
                                    if (user.Ip == UsersOnline[i].Ip)
                                    {
                                        if (user.RSAKeyValue != UsersOnline[i].RSAKeyValue)
                                        {
                                            UsersOnline[i].RSAKeyValue = user.RSAKeyValue;
                                        }
                                        exists = true;
                                        break;
                                    }
                                }
                                if (!exists)
                                    UsersOnline.Add(user);
                            }
                            // TODO: Check if a user is disconnected
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(item);
                    Console.WriteLine("Couldn't deserialize Key");
                    //Console.WriteLine(e.Message);
                    //Console.WriteLine(e.StackTrace);
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }
            keyBuffer.Clear();
        }
    }

}
