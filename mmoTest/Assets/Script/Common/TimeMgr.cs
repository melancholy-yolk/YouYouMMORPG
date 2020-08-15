using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr : MonoBehaviour
{
    private static TimeMgr instance;
    public static TimeMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("TimeMgr");
                instance = obj.AddComponent<TimeMgr>();
            }
            return instance;
        }
    }

    private bool IsScale;//�Ƿ���������
    private float EndTime = 0f;//ʱ�����Ž���ʱ��

    void Update()
    {
        if (IsScale)
        {
            if (Time.realtimeSinceStartup > EndTime)
            {
                Time.timeScale = 1;
                IsScale = false;
            }
        }
    }

    /// <summary>
    /// �޸�ʱ������
    /// </summary>
    /// <param name="toTimeScale">���ų̶�</param>
    /// <param name="continueTime">����ʱ��</param>
    public void ChangeTimeScale(float toTimeScale, float continueTime)
    {
        IsScale = true;
        Time.timeScale = toTimeScale;
        EndTime = Time.realtimeSinceStartup + continueTime;
    }
}
