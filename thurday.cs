//sender
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Program
    {

        
        // to communicate with multiple clients at the same time
        public static void communicate(object? handlerObject)
        {
            var handler = (Socket?)handlerObject;
            // var handler = handlerTask.Result;



            //calculate a bandwidth
            // long total = 1234124695;
            // int sent = 0;
            // int chunkSize = 1024 * 1024 * 100;
            // DateTime started = DateTime.Now;

            FileStream? fs1 = null;
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024 * 1024 * 100];
                var received = handler.Receive(buffer, buffer.Length, SocketFlags.None);

                //calculate a bandwidth
                // sent+=received;
                // TimeSpan elapsedTime=DateTime.Now-started;
                // TimeSpan estimatedTime=TimeSpan.FromSeconds(total-sent)/((double)sent / elapsedTime.TotalSeconds);
                // Console.WriteLine($"kbps : {estimatedTime}");

                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var nameEnd = "<|NAME|>";
                var eom = "<|EOM|>";
                if (response.IndexOf(nameEnd) > -1) // is end of message 
                {
                    fs1 = new(@"videos/" + response.Substring(0, response.IndexOf(nameEnd)).Replace('/', '-').Replace(' ', '_').Replace(':', '-'), FileMode.CreateNew);
                    fs1.Write(buffer, response.IndexOf(nameEnd) + nameEnd.Length, received - response.IndexOf(nameEnd) - nameEnd.Length);
                }
                else
                 if (response.IndexOf(eom) > -1 )// is end of message
                {
                    fs1?.Write(buffer, 0, received - eom.Length);
                    handler.Close();
                    fs1?.Close();
                    break;
                }
                else
                {
                    fs1?.Write(buffer, 0, received);
                }
                // Console.ReadKey();
            }
            Console.WriteLine("Msg has been recived");
        }

        public static void Main(string[] args)
        {
            IPEndPoint ipEndPoint=new(IPAddress.Parse("172.16.0.129"),8080);



            using Socket listener = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(10);

            while (true)
            {
                var handlerTask = listener.Accept();
                if (handlerTask is not null) new Thread(Program.communicate).Start(handlerTask);
            }

        }
    }
}


//c
#define TRACE

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Net.WebSockets;
using System.Net.Http;
// Establish the local endpoint for the socket.
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransferSender
{
    public class Program
    {
        public IPEndPoint ipEndPoint;
        public string fileName;
        public static int numOfSenders = 2;
        public static Semaphore senders = new(numOfSenders, numOfSenders);

        public Program()
        {
            ipEndPoint = new(IPAddress.Parse("172.16.0.129"), 8080);
            fileName = "";
        }

        public void sendFile()
        {

            Stopwatch sw=new Stopwatch();
            Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ipEndPoint);


            // Create the preBuffer data.
            string string1 = string.Format($"{DateTime.UtcNow:MM-dd-yyy_HH-mm-ss-fff}_{fileName.Substring(fileName.LastIndexOf('/') + 1)}<|NAME|>");
            byte[] preBuf = Encoding.ASCII.GetBytes(string1);
            // Create the postBuffer data.
            string string2 = string.Format($"<|EOM|>");
            byte[] postBuf = Encoding.ASCII.GetBytes(string2);


            sw.Start();
            client.SendFile(fileName, preBuf, postBuf, TransmitFileOptions.UseDefaultWorkerThread);
            sw.Stop();

            var et=sw.ElapsedMilliseconds;
            var fi=new FileInfo(fileName);
            Console.WriteLine($"File : {fileName} {et}send speed : {fi.Length/et*1000}");

            // Release the socket.
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            senders.Release();
        }

        public static void Main(string[] args)
        {

            string sourceDirectory = @"/Users/chiragmemriya/Downloads/video_samples";

            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.mp4");

            foreach (var currentFile in txtFiles)
            {
                var p1 = new Program();
                p1.fileName = currentFile;
                Thread t1 = new Thread(new ThreadStart(p1.sendFile));
                senders.WaitOne();
                t1.Start();
            }

        }
    }
}
