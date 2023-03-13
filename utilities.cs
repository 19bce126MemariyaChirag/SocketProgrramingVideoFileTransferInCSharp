using System.Net.Sockets;
using Internal;
using System;
using System.IO;
namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello");
        }
        public byte[] ReadStream(ref NetworkStream ns){
            byte[] data_buff=null;
            int b=0;
            String buff_length="";
            while ((b= ns.ReadByte()!=4))
            {
             buff_length+=(char)b;   
            }
            int data_length=Convert.ToUInt32(buff_length);
            data_buff=new byte[data_length];
            int byte_read=0;
            int byte_offset=0;
            while (byte_offset < data_length)
            {
             byte_read=ns.Read(data_buff,byte_offset,data_length-byte_offset);
             byte_offset+=byte_read;   
            }
            return data_buff;
        }
    }
}
