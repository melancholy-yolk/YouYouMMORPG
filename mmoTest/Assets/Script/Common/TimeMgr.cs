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

    private bool IsScale;//是否在缩放中
    private float EndTime = 0f;//时间缩放结束时间

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
    /// 修改时间缩放
    /// </summary>
    /// <param name="toTimeScale">缩放程度</param>
    /// <param name="continueTime">持续时间</param>
    public void ChangeTimeScale(float toTimeScale, float continueTime)
    {
        IsScale = true;
        Time.timeScale = toTimeScale;
        EndTime = Time.realtimeSinceStartup + continueTime;
    }
}
