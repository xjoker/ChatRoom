using System;

namespace CharClient
{
    class Program
    {


        static void Main(string[] args)
        {
            ConnectServer cs = new ConnectServer();
            cs.Connect();
            Console.WriteLine("连接完成!");
            while(true)
            {
                string sendMsg = Console.ReadLine();
                cs.SendMessage(sendMsg);
                
            }

            //Console.ReadKey();
        }

       
    }
}
