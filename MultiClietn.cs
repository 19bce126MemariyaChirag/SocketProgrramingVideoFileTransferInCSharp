//Server

using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ep = new IPEndPoint(ipAddress, 8080);

            Socket ServerListener = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            ServerListener.Bind(ep);
            ServerListener.Listen(10);

            Console.WriteLine("Server listening..." + ep);

            Program ob = new Program();
            Socket? clientSocket = default(Socket);
            int count = 0;
            while (true)
            {
                count++;
                clientSocket = ServerListener.Accept();
                Console.WriteLine(count + " clients connected");
                Thread userThrread = new Thread(new ThreadStart(() => ob.User(clientSocket,count)));
                userThrread.Start();

            }
        }

        public void User(Socket client, int count)
        {
            while (true)
            {
                byte[] msg = new byte[1024];
                int size = client.Receive(msg);
                if (size > 0)
                {
                    Console.WriteLine($"msg from cleint{count} : "+System.Text.Encoding.ASCII.GetString(msg));
                }
                client.Send(msg, 0, size, SocketFlags.None);
            }
        }
    }
}


//client
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Namespace
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 8080);

            // Create a TCP/IP  socket.
            Socket clientSocket = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(remoteEP);

            Console.WriteLine("Client is connected!");

            while (true)
            {
                string? messageFromClient = null;
                Console.WriteLine("Enter the Message");
                messageFromClient = Console.ReadLine();
                clientSocket.Send(System.Text.Encoding.ASCII.GetBytes(messageFromClient), 0, messageFromClient.Length, SocketFlags.None);

                byte[] msgFromServer = new byte[1024];
                int size = clientSocket.Receive(msgFromServer);
                Console.WriteLine("Serer Echo Test :" + System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size));
            }
        }
    }
}


//15/3/23

//client
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Namespace
{
    public class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);

            // Create a TCP/IP  socket.
            Socket clientSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipEndPoint);

            Console.WriteLine("Client is connected!");

            Console.WriteLine("preparing to the sending a file to the server...");

            //get file name
            // Console.Write("Enter File name : ");
            string? fileName = " 8K_ULTRA_HD.mp4";
            // string? fileName = "8K_ULTRA_HD.mp4";
            
            // Open the file and read its contents
            // byte[] encodedFileName=System.Text.Encoding.ASCII.GetBytes(fileName);

            // string path="/Users/chiragmemriya/Desktop/testing/";
            byte[] fileBytes = File.ReadAllBytes(System.IO.Path.Combine("./", fileName));
            // Send the file to the server
            clientSocket.Send(fileBytes);
            Console.WriteLine("File has been send to the server");


            // while (true)
            // {
            //     string? messageFromClient = null;
            //     Console.WriteLine("Enter the Message");
            //     // messageFromClient = Console.ReadLine();
            //     clientSocket.Send(System.Text.Encoding.ASCII.GetBytes(messageFromClient), 0, messageFromClient.Length, SocketFlags.None);

            //     byte[] msgFromServer = new byte[1024];
            //     int size = clientSocket.Receive(msgFromServer);
            //     Console.WriteLine("Serer Echo Test :" + System.Text.Encoding.ASCII.GetString(msgFromServer, 0, size));
            // }

        }
    }
}

//server

using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ep = new IPEndPoint(ipAddress, 8080);

            Socket ServerListener = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            ServerListener.Bind(ep);
            ServerListener.Listen(10);

            Console.WriteLine("Server listening..." + ep);

            Program ob = new Program();
            Socket? clientSocket = default(Socket);
            int count = 0;
            while (true)
            {
                count++;
                clientSocket = ServerListener.Accept();
                Console.WriteLine(count + " clients connected");
                Thread userThrread = new Thread(new ThreadStart(() => ob.User(clientSocket, Thread.CurrentThread.ManagedThreadId)));
                userThrread.Start();

            }
        }

        public void User(Socket client, int count)
        {
            // while (true)
            // {

            //     byte[] msg = new byte[1024];
            //     int size = client.Receive(msg);
            //     if (size > 0)
            //     {
            //         Console.WriteLine($"msg from cleint{count} : " + System.Text.Encoding.ASCII.GetString(msg));
            //     }
            //     client.Send(msg, 0, size, SocketFlags.None);
            // }
            using (FileStream fileStream = new FileStream("/Users/chiragmemriya/Desktop/testing/hello1.mp4", FileMode.Create))
            {
                // Create a buffer to hold the incoming file data
                byte[] buffer = new byte[1024 * 1024 * 2000];
                // Receive the file data from the server
                int bytesRead = client.Receive(buffer);



                Console.WriteLine("Received file data");
                while (bytesRead > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    bytesRead = client.Receive(buffer);
                }
                Console.WriteLine("return 1");

                //closing file
                fileStream.Close();
            }


        }
    }
}
