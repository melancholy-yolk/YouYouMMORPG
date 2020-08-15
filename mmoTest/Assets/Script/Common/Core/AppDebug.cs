using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppDebug 
{
    public static void Log(object message)
    {
#if DEBUG_LOG
        Debug.Log("<color=green>" + message + "</color>");
#endif
    }

    public static void LogError(object message)
    {
#if DEBUG_LOG
        Debug.LogError("<color=red>" + message + "</color>");
#endif
    }
}
