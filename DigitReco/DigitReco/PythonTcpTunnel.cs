using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DigitReco
{
    class PythonTcpTunnel
    {
        public static byte[] tcp(byte[] bytes)
        {
            using (var tcp = new TcpClient("127.0.0.1", 5367))
            {
                tcp.GetStream().Write(bytes, 0, bytes.Length);
                bytes = new Byte[1];
                tcp.GetStream().Read(bytes, 0, bytes.Length);
                //Console.Write(bytes[0]);
                return bytes;
            }
        }
    }
}
