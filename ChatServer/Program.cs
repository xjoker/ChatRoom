using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;


namespace ChatServer
{
    class Program
    {
        Thread thread = null;//监听线程

        Thread threadGetMsg = null;//消息线程

        //创建一个字典，用来存储客户机的IP地址和套接字
        Dictionary<string, Socket> clientConnectList = new Dictionary<string, Socket>();


        static void Main(string[] args)
        {
        }

        private void ServerStart()
        {
            //设定IP地址
            IPAddress ip = IPAddress.Parse(ChatConfig.IP);

            //绑定端口
            IPEndPoint ipEnd = new IPEndPoint(ip, ChatConfig.port);

            //设置socket协议
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //将socket绑定到端口和IP
                socket.Bind(ipEnd);

                //开始监听进程，10个阻塞
                socket.Listen(10);
               
            }
        }

        /// <summary>
        /// 将套接字作为参数传入其中
        /// </summary>
        /// <param name="s"></param>
        private ArrayList Listen(object s)
        {
            Socket socket = s as Socket;

            while(true)
            {
                //创建一个新的链接
                Socket sConnect = socket.Accept();
                
                //获取到远端的IP地址
                string remoteIPAddress = sConnect.RemoteEndPoint.ToString();

                //将客户添加到字典中
                clientConnectList.Add(remoteIPAddress, sConnect);

                Console.WriteLine("有客户链接上了： "+remoteIPAddress);

                threadGetMsg = new Thread();

            }
        }


        /// <summary>
        /// 消息接收
        /// </summary>
        /// <param name="o"></param>
        private void ReceiveMsg(object o)
        {
            Socket socket = o as Socket;

            //获取到远端的IP地址
            string remoteIPAddress = socket.RemoteEndPoint.ToString();

            while(true)
            {

                //创建一个接收池
                byte[] b = new byte[1024 * 1024];

                try
                {
                    //接收到的报文长度
                    int count = socket.Receive(b);

                    if(count ==0)
                    {
                        Console.WriteLine(remoteIPAddress+"离线！");
                        break;//当离线时直接终止while循环
                    }

                    //将报文从byte转换为unicode后解析为字符串类型
                    string receiveMsg = Encoding.Unicode.GetString(b, 0, count);

                    Console.WriteLine(remoteIPAddress+" 面无表情的说到:"+receiveMsg);
                }
                catch
                {
                    socket.Close();
                    Console.WriteLine(remoteIPAddress+" 断开连接!");
                    break;//当离线时直接终止while循环
                }
            }
        }
    }
}
