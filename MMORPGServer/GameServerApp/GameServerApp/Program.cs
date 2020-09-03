using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServerApp.PVP.WorldMap;

namespace GameServerApp
{
    class Program
    {
        public static string m_ServerIP = "127.0.0.1";
        public static int m_Port = 1037;

        private static Socket m_ServerSocket;

        /// <summary>
        /// 初始化所有控制器
        /// </summary>
        private static void InitAllController()
        {
            RoleController.Instance.Init();

            //世界地图场景管理器初始化
            WorldMapSceneMgr.Instance.Init();
        }

        static void Main(string[] args)
        {
            InitAllController();

            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(m_ServerIP), m_Port));
            m_ServerSocket.Listen(3000);
            Console.WriteLine("启动监听{0}成功", m_ServerSocket.LocalEndPoint.ToString());

            Thread mThread = new Thread(ListenClientCallBack);
            mThread.Start();



            Console.ReadLine();
            
        }

        private static void ListenClientCallBack()
        {
            while(true)
            {
                Socket socket = m_ServerSocket.Accept();
                Console.WriteLine("客户端{0}已经连接", socket.RemoteEndPoint.ToString());
                //一个角色相当于一个客户端
                Role role = new Role();
                ClientSocket clientSocket = new ClientSocket(socket, role);
                //role.Client_Socket = clientSocket;

                RoleMgr.Instance.AllRole.Add(role);
            }
        }
    }
}
