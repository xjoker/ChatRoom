using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CharClient
{
    class ConnectServer
    {

        Socket socket = null; //连接套接字

        Thread thread = null;//连接线程

        public void Connect()
        {
            //设定IP地址
            IPAddress ip = IPAddress.Parse(ChatConfig.IP);

            //绑定端口
            IPEndPoint ipEnd = new IPEndPoint(ip, ChatConfig.port);

            //设置socket协议
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {

                //连接上服务器
                socket.Connect(ipEnd);

                //thread=new Thread(GetMessage)


            }
            catch (Exception ex)
            {
                Console.WriteLine("连接错误 " + ex);
            }

        }

        /// <summary>
        /// 获取消息
        /// </summary>
        private void GetMessage()
        {

        }


        /// <summary>
        /// 消息发送模块
        /// </summary>
        /// <param name="str"></param>
        public void SendMessage(string str)
        {
            //检测socket非空并且已经连接
            if (socket != null && socket.Connected)
            {
                try
                {
                    //将文字转换为byte
                    byte[] b = Encoding.Unicode.GetBytes(str);

                    //调用socket的发送方法发送
                    socket.Send(b);
                }
                catch (Exception ex)
                {
                    socket.Close();
                    Console.WriteLine("出现错误  " + ex);
                }
            }

        }
    }
}
