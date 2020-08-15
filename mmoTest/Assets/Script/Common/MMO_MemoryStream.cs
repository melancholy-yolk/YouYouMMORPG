using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class MMO_MemoryStream :  MemoryStream
{
	public MMO_MemoryStream() : base()
	{

	}
	public MMO_MemoryStream(byte[] buffer) : base(buffer)
	{

	}
	//------------------------------------------------------------
	public short ReadShort()
	{
		byte[] arr = new byte[2];
		base.Read(arr, 0, 2);
		return BitConverter.ToInt16(arr, 0);
	}

	public void WriteShort(short value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public ushort ReadUShort()
	{
		byte[] arr = new byte[2];
		base.Read(arr, 0, 2);
		return BitConverter.ToUInt16(arr, 0);
	}

	public void WriteUShort(ushort value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public int ReadInt()
	{
		byte[] arr = new byte[4];
		base.Read(arr, 0, 4);
		return BitConverter.ToInt32(arr, 0);
	}

	public void WriteInt(int value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public uint ReadUInt()
	{
		byte[] arr = new byte[4];
		base.Read(arr, 0, 4);
		return BitConverter.ToUInt32(arr, 0);
	}

	public void WriteUInt(uint value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public long ReadLong()
	{
		byte[] arr = new byte[8];
		base.Read(arr, 0, 8);
		return BitConverter.ToInt64(arr, 0);
	}

	public void WriteLong(long value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public ulong ReadULong()
	{
		byte[] arr = new byte[8];
		base.Read(arr, 0, 8);
		return BitConverter.ToUInt64(arr, 0);
	}

	public void WriteULong(ulong value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public float ReadFloat()
	{
		byte[] arr = new byte[4];
		base.Read(arr, 0, 4);
		return BitConverter.ToSingle(arr, 0);
	}

	public void WriteFloat(float value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public double ReadDouble()
	{
		byte[] arr = new byte[8];
		base.Read(arr, 0, 8);
		return BitConverter.ToDouble(arr, 0);
	}

	public void WriteDouble(double value)
	{
		byte[] arr = BitConverter.GetBytes(value);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------
	public bool ReadBool()
	{
		return base.ReadByte() == 1;
	}

	public void WriteBool(bool value)
	{
		base.WriteByte((byte)(value == true ? 1 : 0));
	}
	//------------------------------------------------------------
	public string ReadUTF8String()
	{
		ushort len = this.ReadUShort();
		byte[] arr = new byte[len];
		base.Read(arr, 0, len);
		return Encoding.UTF8.GetString(arr);
	}

	public void WriteUTF8String(string str)
	{
		byte[] arr = Encoding.UTF8.GetBytes(str);
		if (arr.Length > 65535)
		{
			throw new InvalidCastException("字符串超出范围");
		}
		WriteUShort((ushort)arr.Length);
		base.Write(arr, 0, arr.Length);
	}
	//------------------------------------------------------------

	//------------------------------------------------------------
	//------------------------------------------------------------
	//------------------------------------------------------------
}
