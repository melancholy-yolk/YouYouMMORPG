using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Text;

///网络传输Socket
public class NetWorkSocket : MonoBehaviour
{
    #region 单例
    //==========================================单例================================================
	private static NetWorkSocket instance;
	public static NetWorkSocket Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject obj = new GameObject("NetWorkSocket");
				DontDestroyOnLoad(obj);
				instance = obj.AddComponent<NetWorkSocket>();
			}
			return instance;
		}
	}
    //=============================================================================================
    #endregion

    #region 发送消息相关数据
    //=====================================发送消息=================================================
	private byte[] buffer = new byte[2048];
	private Queue<byte[]> m_SendQueue = new Queue<byte[]>();//发送消息队列
	private Action m_CheckSendQueue;//检查发送队列的委托
	private const int m_CompressLen = 200;//压缩数组的长度界限
	private Socket m_Client;
    //=============================================================================================
    #endregion

    #region 接受消息相关数据
    //=====================================接收消息=================================================
	private byte[] m_RecieveBuffer = new byte[10240];//接收数据包的字节数组缓冲区
	private MMO_MemoryStream m_ReceiveMS = new MMO_MemoryStream();//接收数据包的缓冲数据流
	private Queue<byte[]> m_ReceiveQueue = new Queue<byte[]>();//接收消息队列
	private int m_ReceiveCount = 0;
    //=============================================================================================
    #endregion

    public Action OnConnectOK;

	void Update()//接收服务器端发送过来的消息
	{
		while (true)
		{
			if (m_ReceiveCount <= 5)
			{
				m_ReceiveCount++;
				lock (m_ReceiveQueue)
				{
					if (m_ReceiveQueue.Count > 0)
					{
						byte[] buffer = m_ReceiveQueue.Dequeue();//压缩标识 + CRC + 包体
//########################################################################################################################
						//注意顺序：1.计算CRC 2.解密 3.解压
						byte[] bufferNew = new byte[buffer.Length - 3];//异或之后的字节数组

						bool isCompress = false;
						ushort crc = 0;

						using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
						{
							//包体长度 压缩标识 CRC16 xor后的包体
							isCompress = ms.ReadBool();
							crc = ms.ReadUShort();
							ms.Read(bufferNew, 0, bufferNew.Length);
						}
						
						int newCrc = Crc16.CalculateCrc16(bufferNew);//计算CRC16
						if (newCrc == crc)
						{
							bufferNew = SecurityUtil.Xor(bufferNew);//解密

							if (isCompress)
							{
								bufferNew = ZlibHelper.DeCompressBytes(bufferNew);//解压
							}

							ushort protoCode = 0;
							byte[] protoContent = new byte[bufferNew.Length - 2];
							using (MMO_MemoryStream ms2 = new MMO_MemoryStream(bufferNew))
							{
								protoCode = ms2.ReadUShort();
								ms2.Read(protoContent, 0, protoContent.Length);

								SocketDispatcher.Instance.Dispatch(protoCode, protoContent);
							}
						}
						else
						{
							break;
						}
//########################################################################################################################
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				m_ReceiveCount = 0;
				break;
			}
		}
		
	}

	void OnDestroy()
	{
		if (m_Client != null && m_Client.Connected)
		{
			m_Client.Shutdown(SocketShutdown.Both);
			m_Client.Close();
		}
	}

    //===========================================连接服务器=====================================================
    #region 连接socket服务器
	public void Connect(string ip, int port)
	{
		if (m_Client != null && m_Client.Connected) return;

		m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		try
		{
			m_Client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
			m_CheckSendQueue = OnCheckSendQueueCallBack;

			RecieveMsg();
            if (OnConnectOK != null)
            {
                OnConnectOK();
            }
			AppDebug.Log("<color=green>连接成功</color>");
		}
		catch(Exception ex)
		{
            AppDebug.Log("<color=green>连接失败</color>" + ex.Message);
		}
	}
    #endregion

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        if (m_Client != null && m_Client.Connected)
        {
            m_Client.Shutdown(SocketShutdown.Both);
            m_Client.Close();
        }
    }

    //===========================================发送数据=====================================================
    #region 发送数据
    //===========================================发送数据=====================================================
    private void OnCheckSendQueueCallBack()//检查发送队列的委托回调
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
		m_Client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, m_Client);
	}

    private void SendCallBack(IAsyncResult ar)//发送数据到服务器的回调
    {
        m_Client.EndSend(ar);

		OnCheckSendQueueCallBack();//继续检查队列
    }

	//包长度 压缩标识(byte) CRC16校验(ushort) Xor之后的数据包(protoCode + protoContent)
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
			ms.WriteUShort((byte)(data.Length + 3));
			ms.WriteBool(isCompress);
			ms.WriteUShort(crc);
			ms.Write(data, 0, data.Length);
			
			retBuffer = ms.ToArray();
		}
		return retBuffer;
	}

	public void SendMsg(byte[] buffer)
	{
		//得到封装后的数据包
		byte[] sendBuffer = MakeData(buffer);

		//发送消息：1.把消息加入发送队列 2.启动检查发送队列的委托
		lock(m_SendQueue)
		{
			m_SendQueue.Enqueue(sendBuffer);
			m_CheckSendQueue.BeginInvoke(null, null);
		}
	}
    //================================================================================================
    #endregion

    //===========================================接收数据=====================================================
    #region 接收数据
    
	private void RecieveMsg()
	{
		//异步接收数据
		m_Client.BeginReceive(m_RecieveBuffer, 0, m_RecieveBuffer.Length, SocketFlags.None, RecieveCallBack, m_Client);
	}

        //接收数据回调
	private void RecieveCallBack(IAsyncResult ar)
	{
		//直接杀掉进程会报异常
		try
		{
			int len = m_Client.EndReceive(ar);

			if (len > 0)
			{
				//把接收到的数据 写入缓冲数据流的尾部
				m_ReceiveMS.Position = m_ReceiveMS.Length;

				//把指定长度的字节数组 写入数据流
				m_ReceiveMS.Write(m_RecieveBuffer, 0, len);

				//如果缓存数据流的长度大于而 说明至少有个不完整的包过来了
				//为什么这里是2 因为客户端封装数据包使用ushort长度为2字节
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

							//把数据流指针放到2的位置 也就是包体的位置
							m_ReceiveMS.Position = 2;

							//把包体读到byte[]数组
							m_ReceiveMS.Read(buffer, 0, currMsgLength);

							lock (m_ReceiveQueue)
							{
								m_ReceiveQueue.Enqueue(buffer);//将一个完整的数据包字节数组放入队列
							}
							

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
						else//收到的字节数不够一个完整包
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
				Debug.Log(string.Format("<color=red>服务器断开连接{0}</color>", m_Client.RemoteEndPoint.ToString()));
			}
		}
		catch
		{
			Debug.Log(string.Format("<color=red>服务器断开连接{0}</color>", m_Client.RemoteEndPoint.ToString()));
		}

    }
    #endregion

}
