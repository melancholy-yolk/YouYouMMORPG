using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �û����ݴ������
/// </summary>
public class TransferData
{
    public TransferData()
    {
        m_PutValues = new Dictionary<string, object>();
    }

    #region m_PutValues
    /// <summary>
    /// �����ֵ�
    /// </summary>
    private Dictionary<string, object> m_PutValues;

    public Dictionary<string, object> PutValues
    {
        get { return m_PutValues; }
    }
    #endregion

    /// <summary>
    /// ��ֵ
    /// </summary>
    /// <typeparam name="TM"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValue<TM>(string key, TM value)
    {
        m_PutValues[key] = value;
    }

    /// <summary>
    /// ȡֵ
    /// </summary>
    /// <typeparam name="TM"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public TM GetValue<TM>(string key)
    {
        if (m_PutValues.ContainsKey(key))
        {
            return (TM)m_PutValues[key];
        }
        return default(TM);
    }
}
