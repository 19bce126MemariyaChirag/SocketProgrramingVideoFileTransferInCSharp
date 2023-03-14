using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;


namespace MyExampleNamesapce
{
    public class MySocketClient
    {
        static void  Main(string[] args)
        {
            

            // IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            // IPAddress ipAddress = ipHostInfo.AddressList[0];
            // IPEndPoint ipEndPoint = new (ipAddress, 8080);
           IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(new byte[]{172,16,0,189}), 8080);

            using Socket client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

       
            Console.WriteLine("Client is connected!");

                    Console.WriteLine("preparing to the sending a file to the server...");
                    // Open the file and read its contents
                    string fileName="video.mp4";
                    // string path="/Users/chiragmemriya/Desktop/testing/";
                    byte[] fileBytes = File.ReadAllBytes(System.IO.Path.Combine("./", fileName));
                    // Send the file to the server
                    client.Connect(ipEndPoint);
                    client.Send(fileBytes); 
                    Console.WriteLine("File has been send to the server");
            
            client.Shutdown(SocketShutdown.Both);
        }
    }
}
