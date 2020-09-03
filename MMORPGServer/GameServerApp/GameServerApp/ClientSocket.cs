using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServerApp.PVP.WorldMap;

namespace GameServerApp
{
    /// <summary>
    /// 客户端连接对象 负责和客户端进行通讯
    /// </summary>
    public class ClientSocket
    {
        //所属角色
        private Role m_Role;

        //客户端socket
        private Socket m_Socket;

        //接收数据的线程
        private Thread m_RecieveThread;

        #region 接收消息所需变量
        //接收数据包的字节数组缓冲区
        private byte[] m_RecieveBuffer = new byte[10240];

        //接收数据包的缓冲数据流
        private MMO_MemoryStream m_ReceiveMS = new MMO_MemoryStream();
        #endregion

        #region 发送消息所需变量
        private Queue<byte[]> m_SendQueue = new Queue<byte[]>();
        private Action m_CheckSendQueue;
        #endregion

        private const int m_CompressLen = 200;

        //构造函数
        public ClientSocket(Socket socket, Role role)
        {
            m_Role = role;
            m_Role.Client_Socket = this;
            m_Socket = socket;

            //启动接收线程
            m_RecieveThread = new Thread(RecieveMsg);
            m_RecieveThread.Start();

            m_CheckSendQueue = OnCheckSendQueueCallBack;

            //using (MMO_MemoryStream ms = new MMO_MemoryStream())
            //{
            //    ms.WriteUTF8String("欢迎登陆服务器" + DateTime.Now);
            //    this.SendMsg(ms.ToArray());
            //}
        }

        #region 接收数据
        //接收数据
        private void RecieveMsg()
        {
            //异步接收数据
            m_Socket.BeginReceive(m_RecieveBuffer, 0, m_RecieveBuffer.Length, SocketFlags.None, RecieveCallBack, m_Socket);
        }

        //接收数据回调
        private void RecieveCallBack(IAsyncResult ar)
        {
            //直接杀掉进程会报异常
            try
            {
                int len = m_Socket.EndReceive(ar);

                if (len > 0)
                {
                    //把接收到的数据 写入缓冲数据流的尾部
                    m_ReceiveMS.Position = m_ReceiveMS.Length;

                    //把指定长度的字节数组 写入数据流
                    m_ReceiveMS.Write(m_RecieveBuffer, 0, len);

                    //如果缓存数据流的长度大于2 说明至少有个不完整的包过来了
                    //为什么这里是2：因为客户端封装数据包时，包头使用ushort长度为2字节
                    if (m_ReceiveMS.Length > 2)
                    {
                        //进行循环 拆分数据包
                        while(true)
                        {
                            m_ReceiveMS.Position = 0;

                            //包体的长度
                            int currMsgLength = m_ReceiveMS.ReadUShort();

                            //总包的长度
                            int currFullMsgLen = 2 + currMsgLength;

                            //如果数据流的长度 >= 整包的长度 说明至少收到了一个完整包
                            if (m_ReceiveMS.Length >= currFullMsgLen)
                            {
                                byte[] buffer = new byte[currMsgLength];

                                //把数据流指针放到2的位置 也就是包体的位置(数据流的前面两个字节ushort是包体的长度)
                                m_ReceiveMS.Position = 2;

                                //把包体读到byte[]数组
                                m_ReceiveMS.Read(buffer, 0, currMsgLength);
//########################################################################################################################
                                byte[] bufferNew = new byte[buffer.Length - 3];
                                bool isCompress = false;
                                ushort crc = 0;
                                using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
                                {
                                    isCompress = ms.ReadBool();
                                    crc = ms.ReadUShort();
                                    ms.Read(bufferNew, 0, bufferNew.Length);
                                }
                                int newCrc = Crc16.CalculateCrc16(bufferNew);
                                if(newCrc == crc)
                                {
                                    bufferNew = SecurityUtil.Xor(bufferNew);
                                    if(isCompress)
                                    {
                                        bufferNew = ZlibHelper.DeCompressBytes(bufferNew);
                                    }

                                    ushort protoCode = 0;
                                    byte[] protoContent = new byte[bufferNew.Length - 2];
                                    using(MMO_MemoryStream ms = new MMO_MemoryStream(bufferNew))
                                    {
                                        protoCode = ms.ReadUShort();
                                        ms.Read(protoContent, 0, protoContent.Length);
                                    }
                                    EventDispatcher.Instance.Dispatch(protoCode, m_Role, protoContent);
                                }
                                else
                                {
                                    break;
                                }
//########################################################################################################################
                                //================================================

                                

                                //========读完一整个数据包后 处理剩余字节数据========

                                //剩余字节长度
                                int remainLen = (int)m_ReceiveMS.Length - currFullMsgLen;

                                //读完一个完整的包后 数据流中还有字节数据
                                if (remainLen > 0)
                                {
                                    //把指针放到第一个数据包的尾部
                                    m_ReceiveMS.Position = currFullMsgLen;

                                    
                                    byte[] remainBuffer = new byte[remainLen];

                                    //把剩余字节读入到剩余字节数组中
                                    m_ReceiveMS.Read(remainBuffer, 0, remainLen);

                                    //清空数据流
                                    m_ReceiveMS.Position = 0;
                                    m_ReceiveMS.SetLength(0);

                                    //把剩余字节重新写入数据流
                                    m_ReceiveMS.Write(remainBuffer, 0, remainBuffer.Length);

                                    remainBuffer = null;

                                }
                                else//读完一个完整的包后 数据流中没有剩余字节
                                {
                                    //清空数据流
                                    m_ReceiveMS.Position = 0;
                                    m_ReceiveMS.SetLength(0);
                                    break;
                                }

                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //进行下一次接收数据包
                    RecieveMsg();
                }
                else
                {
                    Console.WriteLine("客户端断开连接,发来的字节数为0,{0}", m_Socket.RemoteEndPoint.ToString());
                    m_Role.UpdateLastInWorldMap();//客户端断开连接时 将客户端的世界地图相关信息更新到数据库里
                    WorldMapSceneMgr.Instance.RoleLeave(m_Role.RoleId, m_Role.LastInWorldMapId);//客户端断开连接时 执行角色离开世界地图场景方法
                    RoleMgr.Instance.AllRole.Remove(m_Role);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}客户端断开连接,接收数据过程发生异常,{1}", m_Role.RoleId, m_Socket.RemoteEndPoint.ToString() );
                m_Role.UpdateLastInWorldMap();//客户端断开连接时 将客户端的世界地图相关信息更新到数据库里
                WorldMapSceneMgr.Instance.RoleLeave(m_Role.RoleId, m_Role.LastInWorldMapId);//客户端断开连接时 执行角色离开世界地图场景方法
                RoleMgr.Instance.AllRole.Remove(m_Role);//客户端断开连接时 执行角色离开世界地图场景方法
            }

        }
        #endregion

        #region 发送数据
        //检查发送队列的委托回调
        private void OnCheckSendQueueCallBack()
        {
            //检查发送队列中是否有数据包
            lock (m_SendQueue)
            {
                if (m_SendQueue.Count > 0)
                {
                    Send(m_SendQueue.Dequeue());
                }
                else
                {

                }
            }
        }

        private void Send(byte[] buffer)//真正发送消息到服务器
        {
            m_Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, m_Socket);//异步发送数据
        }

        private void SendCallBack(IAsyncResult ar)//发送数据到服务器的回调
        {
            m_Socket.EndSend(ar);

            OnCheckSendQueueCallBack();//继续检查队列
        }

        ////封装数据包 包头(数据包长度) + 包体
        //private Byte[] MakeData(byte[] data)
        //{
        //    byte[] retBuffer = null;
        //    using (MMO_MemoryStream ms = new MMO_MemoryStream())
        //    {
        //        ms.WriteUShort((ushort)data.Length);//向流中写入数据 包头
        //        ms.Write(data, 0, data.Length);//向流中写入数据 包体
        //        retBuffer = ms.ToArray();
        //    }
        //    return retBuffer;
        //}
        private Byte[] MakeData(byte[] data)//封装数据包 包头(数据包长度) + 包体
        {
            byte[] retBuffer = null;

            //注意顺序：1.压缩 2.加密 3.计算CRC
            bool isCompress = data.Length > m_CompressLen ? true : false;
            if (isCompress)
            {
                data = ZlibHelper.CompressBytes(data);//压缩包体
            }
            data = SecurityUtil.Xor(data);//异或加密
            ushort crc = Crc16.CalculateCrc16(data);//计算crc

            using (MMO_MemoryStream ms = new MMO_MemoryStream())
            {
                ms.WriteUShort((byte)(data.Length + 3));//包体长度
                ms.WriteBool(isCompress);//压缩标识
                ms.WriteUShort(crc);//crc16
                ms.Write(data, 0, data.Length);//包体内容

                retBuffer = ms.ToArray();
            }
            return retBuffer;
        }

        public void SendMsg(byte[] buffer)
        {
            //得到封装后的数据包
            byte[] sendBuffer = MakeData(buffer);

            //发送消息：
            //1.把消息加入发送队列 
            //2.启动检查发送队列的委托
            lock (m_SendQueue)
            {
                m_SendQueue.Enqueue(sendBuffer);
                m_CheckSendQueue.BeginInvoke(null, null);
            }
        }
        #endregion
    }
}
