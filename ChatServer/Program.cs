﻿using System;
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

        Socket socket = null;

        //创建一个字典，用来存储客户机的IP地址和套接字
        Dictionary<string, Socket> clientConnectList = new Dictionary<string, Socket>();


        static void Main(string[] args)
        {
            Program pr = new Program();
            pr.ServerStart();
            Console.ReadKey();
        }

        public void ServerStart()
        {
            //设定IP地址
            IPAddress ip = IPAddress.Parse(ChatConfig.IP);

            //绑定端口
            IPEndPoint ipEnd = new IPEndPoint(ip, ChatConfig.port);

            //设置socket协议
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //将socket绑定到端口和IP
                socket.Bind(ipEnd);

                //开始监听进程，10个阻塞
                socket.Listen(10);

                Console.WriteLine("启动监听...");

                //开启监听线程
                thread = new Thread(Listen);
                thread.IsBackground = true;
                thread.Start(socket);

            }
            catch (Exception ex)
            {
                Console.WriteLine("出现异常:" + ex);
            }
        }

        /// <summary>
        /// 将套接字作为参数传入其中
        /// </summary>
        /// <param name="s"></param>
        private void Listen(object s)
        {
            Socket socket = s as Socket;

            while (true)
            {
                //创建一个新的链接
                Socket sConnect = socket.Accept();

                //获取到远端的IP地址
                string remoteIPAddress = sConnect.RemoteEndPoint.ToString();

                //将客户添加到字典中
                clientConnectList.Add(remoteIPAddress, sConnect);

                Console.WriteLine("有客户链接上了： " + remoteIPAddress);


                //启动线程接收消息
                threadGetMsg = new Thread(ReceiveMsg);
                threadGetMsg.IsBackground = true;
                threadGetMsg.Start(sConnect);

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

            while (true)
            {

                //创建一个接收池
                byte[] b = new byte[1024 * 1024];

                try
                {
                    //接收到的报文长度
                    int count = socket.Receive(b);

                    if (count == 0)
                    {
                        Console.WriteLine(remoteIPAddress + "离线！");
                        break;//当离线时直接终止while循环
                    }

                    //将报文从byte转换为unicode后解析为字符串类型
                    //如果直接GetString会导致后面很多0 要限定数据范围
                    string receiveMsg = Encoding.Unicode.GetString(b, 0, count);

                    BroadcastMsg(receiveMsg,remoteIPAddress);//广播消息

                    Console.WriteLine(remoteIPAddress + " 面无表情的说到:" + receiveMsg);
                }
                catch
                {
                    socket.Close();
                    Console.WriteLine(remoteIPAddress + " 断开连接!");
                    break;//当离线时直接终止while循环
                }
            }
        }


        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        private void BroadcastMsg(string msg,string ip)
        {


            //遍历已经链接的客户端，广播消息给他们
            foreach(Socket ss in clientConnectList.Values)
            {
                try
                {

                    string sendMsg = ip + "痴汉笑着说: " + msg;
                    //编码为byte
                    byte[] b = Encoding.Unicode.GetBytes(sendMsg);

                    ss.Send(b);

                }
                catch(Exception ex)
                {
                    ss.Close();
                    Console.WriteLine(clientConnectList.Keys+"广播消息异常！已经关闭连接！");
                    Console.WriteLine("错误消息："+ex);
                }
            }
        }
    }
}
