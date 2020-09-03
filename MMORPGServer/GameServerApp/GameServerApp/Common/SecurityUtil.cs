using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 异或加密类
/// </summary>
public sealed class SecurityUtil 
{
	private static byte[] xorScale = new byte[] { 45, 66, 38, 55, 23, 254, 9, 165, 90, 19, 41, 45, 201, 58, 55, 37, 254, 185, 165, 169, 19, 171 };

	private SecurityUtil()
	{

	}

	public static byte[] Xor(byte[] buffer)
	{
		int iScaleLen = xorScale.Length;
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] = (byte)(buffer[i] ^ xorScale[i % iScaleLen]);
		}
		return buffer;
	}
}
