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
                IPHostEntry ipHostEntry = await Dns.GetHostEntryAsync("localhost");
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
                Socket handler = await listner.AcceptAsync();
                Console.WriteLine("clients connected..");
                while (true)
                {
                   
                        //receive Message
                        var buffer = new byte[1024];
                        var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);
                    if (response == "end")
                    {
                        break;
                    }

                    if (response.Length>0)
                        {
                            Console.WriteLine($"Socket Server received: {response}");
                            Console.Write($"Enter msg : ");
                            var ackMesage=Console.ReadLine();
                            var echoByte = Encoding.UTF8.GetBytes(ackMesage);
                            await handler.SendAsync(echoByte, 0);
                            
                        }
                       
                        //break;
                    
                }
                handler.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
    }
}
