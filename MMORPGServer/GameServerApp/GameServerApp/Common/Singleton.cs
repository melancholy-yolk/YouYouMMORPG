using System.Collections;
using System.Collections.Generic;
using System;

public class Singleton<T> : IDisposable where T : new() 
{
	protected static T instance;
    protected static readonly object lockObj = new object();
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
		        lock(lockObj)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
			}
			return instance;
		}
	}

	public virtual void Dispose()
	{
		
	}

}
