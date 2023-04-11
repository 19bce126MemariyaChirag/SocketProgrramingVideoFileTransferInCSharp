//172.16.0.135
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyExampleNamesapce
{
    class mySocketServer
    {
        public static async Task Main(string[] args)
        {
            try
            {
                IPHostEntry ipHostEntry = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostEntry.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);

                Socket listner = new Socket(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                listner.Bind(ipEndPoint);
                listner.Listen(10);
                Console.WriteLine("Server listening...");
                Socket handler =  listner.Accept();

                Console.WriteLine("client connected..");
                        // Create a buffer to hold the incoming file data
                        byte[] buffer = new byte[1024 * 1024 * 1000];
                        // Receive the file data from the server
                        using (FileStream fileStream = new FileStream($@"videoOutput.mp4", FileMode.Create))
                        {
                            int bytesRead = handler.Receive(buffer);
                            Console.WriteLine("Received file data");
                            if (bytesRead > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                               // bytesRead = handler.Receive(buffer);
                            }
                            Console.WriteLine("write file data completed");
                            //closing file
                            fileStream.Close();
                        }
                        Console.Write($"Enter msg : ");
                        var ackMesage = Console.ReadLine();
                        var echoByte = Encoding.UTF8.GetBytes(ackMesage);
                        handler.Send(echoByte, 0);
                    handler.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
    }
}
