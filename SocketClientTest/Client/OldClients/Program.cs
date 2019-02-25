//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Sockets;
//using System.Security.Cryptography;
//using System.Security.Cryptography.Xml;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

//namespace AsymmetricChatBot
//{
//    class Program
//    {
//        static TcpClient client;
//        static StreamReader reader;
//        static StreamWriter writer;
//        static void Main(string[] args)
//        {
//            client = new TcpClient();
//            client.Connect(Properties.Settings.Default.host, 8891);
//            client.SendBufferSize = 32000;
//            reader = new StreamReader(client.GetStream(), System.Text.Encoding.ASCII);
//            writer = new StreamWriter(client.GetStream());
//            writer.AutoFlush = true;

//            jokes = new string[Properties.Settings.Default.jokes.Count];
//            Properties.Settings.Default.jokes.CopyTo(jokes, 0);

//            Thread t = new Thread(new ThreadStart(handleCom));
//            t.Start();

//            SendMyKey();

//            key her

//            var timer = new Timer(TimerTask, null, 0, 20000);

//            Console.ReadKey();

//        }

//        static RSACryptoServiceProvider myKeyRSA = new RSACryptoServiceProvider(2048);

//        static RSAParameters myRSAKeyInfo;

//        static void SendMyKey()
//        {
//            Console.WriteLine("Nøgle sendes");
//            asymmetrisk Encrypter og decrypter vis RSA algoritmen.Som parameter kan man angive nøgle størrelsen i bit


//           En container der indeholder RSA parametre såsom public key og lign.Hvis parameter sættes til true, gemmes også private key
//          Nøgleparret er genereret
//            myRSAKeyInfo = myKeyRSA.ExportParameters(false);

//            var networkStream = client.GetStream();
//            if (networkStream.CanWrite)
//            {
//                byte[] dd = Encoding.ASCII.GetBytes(myKeyRSA.ToXmlString(false));
//        networkStream.Write(dd, 0, dd.Length);

//            }
//    Console.WriteLine("Nøgle er sendt");

//        }
//static List<User> uu = new List<User>();

//static Dictionary<string, EncryptData> users = new Dictionary<string, EncryptData>();
//static string[] jokes;


//private static void TimerTask(object state)
//{
//    if (uu.Count > 0)
//    {

//        Console.WriteLine("Klar til at sende");

//        Random rnd = new Random();
//        int r = rnd.Next(uu.Count);

//        From f = new From();
//        f.Ip = Properties.Settings.Default.me;
//        f.Name = Environment.MachineName;
//        To t = new To();
//        t.Ip = uu[r].Ip;
//        t.Name = "Cammy";
//        MessageBody mb = new MessageBody();

//        Find en random joke

//                int j = rnd.Next(jokes.Length);
//        mb.Body = jokes[j];
//        Message m = new Message();
//        m.From = f;
//        m.To = t;
//        m.Mb = mb;


//        Console.WriteLine("Jeg vil gerne sende en besked til " + uu[r].Ip);

//        først skal vi finde ud af om det er en bruger jeg tidligere har talt med
//                RSACryptoServiceProvider clientRSA = new RSACryptoServiceProvider(2048);

//        if (!users.ContainsKey(uu[r].Ip))
//        {
//            Console.WriteLine("Jeg har aldrig talt med denne bruger før");

//            Først oprettes en asym og den offentlige nøgle indlæses


//            så er jeg nødt til at serialisere brugens nøgle
//                    XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(RSAKeyValue), new XmlRootAttribute("RSAKeyValue"));
//            using (StringWriter textWriter = new StringWriter())
//            {

//                Console.WriteLine("Serialize");
//                x.Serialize(textWriter, uu[r].RSAKeyValue);
//                Console.WriteLine("Nøglen indlæses i RSA" + textWriter.ToString());
//                clientRSA.FromXmlString(textWriter.ToString());

//                using (var aesAlg = Aes.Create())
//                {
//                    aesAlg.Padding = PaddingMode.PKCS7;
//                    aesAlg.KeySize = 128;
//                    aesAlg.GenerateKey();

//                    byte[] encryptedSymmetricKey = clientRSA.Encrypt(aesAlg.Key, false);
//                    byte[] encryptedSymmetricIV = clientRSA.Encrypt(aesAlg.IV, false);
//                    m.Key = Convert.ToBase64String(encryptedSymmetricKey);
//                    m.Iv = Convert.ToBase64String(encryptedSymmetricIV);

//                    EncryptData edd = new EncryptData();
//                    edd.Iv = aesAlg.IV;
//                    edd.Key = aesAlg.Key;
//                    edd.EKey = Convert.ToBase64String(encryptedSymmetricKey);
//                    edd.EIv = Convert.ToBase64String(encryptedSymmetricIV);

//                    users.Add(uu[r].Ip, edd);
//                }

//            }
//        }
//        EncryptData ed = users[uu[r].Ip];

//        mb.Body = Encrypt(mb.Body, ed.Iv, ed.Key);
//        m.Key = ed.EKey;
//        m.Iv = ed.EIv;

//        Console.WriteLine(mb.Body);
//        Nu er der klar til serialisering

//                var xmlSerializer = new XmlSerializer(m.GetType());
//        var networkStream = client.GetStream();
//        if (networkStream.CanWrite)
//        {
//            Console.WriteLine("Sender");
//            xmlSerializer.Serialize(networkStream, m);
//        }

//    }
//}

//private static StringBuilder myCompleteMessage = new StringBuilder();

//private static void handleCom()
//{
//    while (true)
//    {

//        if (client.GetStream().CanRead)
//        {

//            int bytesRead = 0;
//            byte[] message = new byte[32000];

//            do
//            {
//                bytesRead = client.GetStream().Read(message, 0, message.Length);
//                myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(message, 0, bytesRead));
//                if (myCompleteMessage.ToString().ToLower().IndexOf("</message>") > -1)
//                {

//                    break;
//                }
//            }
//            while (client.GetStream().DataAvailable);

//            if (myCompleteMessage.ToString().ToLower().IndexOf("</message>") > -1)
//            {
//                handleData();

//            }

//        }

//    }

//}

//private static void handleData()
//{
//    Console.ForegroundColor = ConsoleColor.Yellow;

//    Console.WriteLine("MS:" + myCompleteMessage.ToString());
//    Console.ForegroundColor = ConsoleColor.Gray;
//    Console.WriteLine("*:");

//    MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(myCompleteMessage.ToString()));
//    XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Message), new XmlRootAttribute("Message"));
//    Message cm = null;

//    cm = (Message)x.Deserialize(stream);
//    if ((cm.Users != null) && (cm.Users.Count > 0))
//    {
//        opdater listen
//                uu = cm.Users;
//        Console.ForegroundColor = ConsoleColor.Cyan;
//        foreach (var item in uu)
//        {
//            Console.WriteLine(item.Ip + ":" + item.RSAKeyValue.Exponent);
//        }

//        Console.ForegroundColor = ConsoleColor.Gray;

//    }

//    incomming
//            if ((cm.Mb != null) && (cm.Mb.Body != null) && (cm.Mb.Body.Length > 0))
//    {
//        Console.ForegroundColor = ConsoleColor.Red;

//        Console.WriteLine("Jeg har fået en besked");

//        decrypt
//                 Console.WriteLine("Besked fra " + cm.From.Ip + ":" + Decrypt(cm.Mb.Body));

//        kender jeg iv og key ?
//                if (users.ContainsKey(cm.From.Ip))
//        {

//            Console.WriteLine("Jeg kender IV");

//            EncryptData e = users[cm.From.Ip];

//            Console.WriteLine("Besked: " + Decrypt(cm.Mb.Body, e.Iv, e.Key));

//        }
//        else
//        {

//            Console.WriteLine("Jeg kender IKKE IV , så jeg dekrypterer med min private nøgle");
//            //for at kende IV skal jeg først trække IV ud af obj
//            //IV ligger i en base64 encoding

//            Console.WriteLine("IV " + cm.Iv);
//            byte[] encryptedRsa = Convert.FromBase64String(cm.Iv);
//            byte[] encryptedkey = Convert.FromBase64String(cm.Key);

//            ////Så skal iv dekrypteres med 
//            //byte[] eiv = clientRSA.Decrypt(encryptedRsa, false);
//            //byte[] ekey = clientRSA.Decrypt(encryptedkey, false);
//            //Console.WriteLine("Besked: " + Decrypt(cm.Mb.Body, eiv, ekey));

//        }

//        byte[] encryptedRsa = Convert.FromBase64String(cm.Iv);

//        byte[] encryptedkey = Convert.FromBase64String(cm.Key);
//        byte[] eiv = myKeyRSA.Decrypt(encryptedRsa, false);
//        byte[] ekey = myKeyRSA.Decrypt(encryptedkey, false);

//        Console.WriteLine("Besked: " + Decrypt(cm.Mb.Body, eiv, ekey));
//        Console.ForegroundColor = ConsoleColor.Gray;

//    }
//    myCompleteMessage.Clear();
//}


//public static string Decrypt(string dataToBeDecrypted, byte[] iv, byte[] key)
//{
//    string plaintext = "";
//    using (var aesAlg = Aes.Create())
//    {

//        aesAlg.Padding = PaddingMode.PKCS7;

//        aesAlg.Key = key; // 16 bytes for 128 bit encryption
//        aesAlg.IV = iv; // AES needs a 16-byte IV

//        using (var decryptor = aesAlg.CreateDecryptor(key, aesAlg.IV))
//        {
//            using (MemoryStream msDecrypt = new MemoryStream(System.Convert.FromBase64String(dataToBeDecrypted)))
//            {
//                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//                {
//                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
//                    {

//                        Read the decrypted bytes from the decrypting stream
//                                 and place them in a string.
//                                plaintext = srDecrypt.ReadToEnd();
//                    }
//                }
//            }
//        }
//    }
//    return plaintext;

//}

//public static string Encrypt(string dataToBeEncrypted, byte[] iv, byte[] key)
//{
//    using (var aesAlg = Aes.Create())
//    {

//        Console.WriteLine("::" + key.Length);
//        aesAlg.Padding = PaddingMode.PKCS7;

//        aesAlg.Key = key; // 16 bytes for 128 bit encryption
//        aesAlg.IV = iv; // AES needs a 16-byte IV

//        byte[] plainTextBytes = Encoding.UTF8.GetBytes(dataToBeEncrypted);
//        byte[] encryptetTextBytes;

//        using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
//        {
//            using (var memoryStream = new MemoryStream())
//            {
//                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
//                {
//                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
//                    cryptoStream.FlushFinalBlock();
//                    encryptetTextBytes = memoryStream.ToArray();

//                }

//            }

//        }

//        return Convert.ToBase64String(encryptetTextBytes);
//    }

//}

//public static void WriteMessage(Object message)
//{
//    var xmlSerializer = new XmlSerializer(message.GetType());
//    var networkStream = client.GetStream();
//    if (networkStream.CanWrite)
//    {
//        xmlSerializer.Serialize(networkStream, message);
//    }

//}

//public class XML
//{

//    public XML() { }
//}

//public class Users
//{
//    string _ip, _name;
//    public Users(string ip, string name)
//    {
//        this.Ip = ip;
//        Name = name;
//    }

//    public string Ip { get => _ip; set => _ip = value; }
//    public string Name { get => _name; set => _name = value; }
//}

//public class EncryptData
//{
//    byte[] iv;
//    byte[] key;
//    string eiv;
//    string ekey;
//    public byte[] Iv { get => iv; set => iv = value; }
//    public byte[] Key { get => key; set => key = value; }
//    public string EIv { get => eiv; set => eiv = value; }
//    public string EKey { get => ekey; set => ekey = value; }
//}

//    }

//}