using System.Collections;
using System.Collections.Generic;
using GameServerApp;

public class EventDispatcher : Singleton<EventDispatcher> 
{
	public delegate void OnActionHandler(Role role, byte[] buffer);
	private Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();

	public void AddEventListener(ushort protoCode, OnActionHandler handler)
	{
		if (dic.ContainsKey(protoCode))
		{
			dic[protoCode].Add(handler);
		}
		else
		{
			List<OnActionHandler> handlersList = new List<OnActionHandler>();
			handlersList.Add(handler);
			dic[protoCode] = handlersList;
		}
	}

	public void RemoveEventListener(ushort protoCode, OnActionHandler handler)
	{
		if (dic.ContainsKey(protoCode))
		{
			List<OnActionHandler> handlersList = dic[protoCode];
			handlersList.Remove(handler);
			if (handlersList.Count == 0)
			{
				dic.Remove(protoCode);
			}
		}
	}

	public void Dispatch(ushort protoCode, Role role, byte[] buffer)
	{
		if (dic.ContainsKey(protoCode))
		{
			List<OnActionHandler> handlersList = dic[protoCode];
			if (handlersList != null && handlersList.Count > 0)
			{
				for (int i = 0; i < handlersList.Count; i++)
				{
					if (handlersList[i] != null)
					{
						handlersList[i](role, buffer);
					}
				}
			}
		}
	}
}

