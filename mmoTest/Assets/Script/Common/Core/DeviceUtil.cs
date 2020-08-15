using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUtil 
{
    /// <summary>
    /// �ͻ����豸id
    /// </summary>
    public static string DeviceIdentifier
    {
        get
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }

    /// <summary>
    /// ��ȡ�豸�ͺ�
    /// </summary>
    public static string DeviceModel
    {
        get
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            return Device.generation.ToString();
#else
            return SystemInfo.deviceModel;
#endif
        }
    }
}
