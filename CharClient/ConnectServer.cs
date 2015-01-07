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
                

                //启动一个接收消息的线程
                thread = new Thread(GetMessage);
                thread.IsBackground = true;
                thread.Start();


            }
            catch (Exception ex)
            {

                //关闭套接字，结束接收消息线程
                socket.Close();
                thread.Abort();
                Console.WriteLine("连接错误 " + ex);
            }

        }

        /// <summary>
        /// 获取消息
        /// </summary>
        private void GetMessage()
        {
            while(true)
            {
                byte[] b = new byte[1024 * 1024];

                try
                {
                    int count = socket.Receive(b);

                    //当有内容时执行
                    if(count!=0)
                    {
                        //如果直接GetString会导致后面很多0 要限定数据范围
                        string receiveMsg = Encoding.Unicode.GetString(b,0,count);
                        Console.WriteLine(receiveMsg);
                        //count = 0;
                        
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine("链接服务器异常");
                    Console.WriteLine(ex);
                    socket.Close();
                    thread.Abort();
                    
                    
                }
            }


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
