﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDBModel<T, P> 
			 where T : class, new()
			 where P : AbstractEntity
{

	protected List<P> m_List;
	protected Dictionary<int, P> m_Dic;

	public AbstractDBModel()
	{
		m_List = new List<P>();
		m_Dic = new Dictionary<int, P>();
		LoadData();
	}

	public static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new T();
			}
			return instance;
		}
	}

	protected abstract string FileName { get; }
	protected abstract P MakeEntity(GameDataTableParser parse);

	private void LoadData()
	{
		//1.读取文件
		//2.解压缩
        string path = string.Empty;
#if DISABLE_ASSETBUNDLE
        path = Application.dataPath + "/Download/DataTable/" + FileName;
#else
        path = Application.persistentDataPath + "/Download/DataTable/" + FileName;
#endif

        using (GameDataTableParser parse = new GameDataTableParser(path))
		{
			while (!parse.Eof)
			{
				P p = MakeEntity(parse);
				m_List.Add(p);
				m_Dic[p.Id] = p;
				parse.Next();
			}
		}
		
	}

	public List<P> GetList()
	{
		return m_List;
	}

	public P GetEntity(int id)
	{
		if (m_Dic.ContainsKey(id))
		{
			return m_Dic[id];
		}
		return null;
	}

}
