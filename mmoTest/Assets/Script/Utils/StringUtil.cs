using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtil 
{
	public static int ToInt(this string str)
	{
		int temp = 0;
		int.TryParse(str, out temp);
		return temp;
	}

	public static float ToFloat(this string str)
	{
		float temp = 0;
		float.TryParse(str, out temp);
		return temp;
	}

    public static long ToLong(this string str)
    {
        long temp = 0;
        long.TryParse(str, out temp);
        return temp;
    }
}
