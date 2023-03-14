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
        private static int totalBytesRead=0;

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
                        byte[] buffer = new byte[1024 * 1024 * 2000];
                        // Receive the file data from the server


                        using (FileStream fileStream = new FileStream("/Users/chiragmemriya/Desktop/testing/hello11234.mp4", FileMode.Create))
                        {
                            int bytesRead = handler.Receive(buffer);
                    
                            Console.WriteLine("Received file data");
                            while (bytesRead>0)
                            {
                             fileStream.Write(buffer,0,bytesRead);
                             bytesRead = handler.Receive(buffer);  
                            }
                            Console.WriteLine("return 1");
                            
                            //closing file
                            fileStream.Close();
                        }

                   
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
    }
}
