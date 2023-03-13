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
        static async Task Main(string[] args)
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            // IPEndPoint ipEndPoint = new(new IPAddress(new byte[] { 172, 16, 0, 205 }), 8080);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);


            using Socket client = new(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);
            Console.WriteLine("Client is connected!");
            while (true)
            {
                // Send message.
                Console.Write("Enter Message : ");
                String str = Console.ReadLine();
                var messageBytes = Encoding.UTF8.GetBytes(str);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);
                // Console.WriteLine($"Socket client sent message: \"{str}");

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                if (response.EndsWith("end")|| str.Equals("end"))
                {
                    break;
                }
                    response = response.Remove(response.Length);
                    Console.WriteLine(
                        $"Socket client received acknowledgment: \"{response}");
            }

            client.Shutdown(SocketShutdown.Both);
        }
    }
}
