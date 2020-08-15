using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelEntity 
{
    private Vector2 m_Position = Vector2.zero;

    /// <summary>
    /// 关卡在关卡上的本地坐标
    /// </summary>
    public Vector2 Position
    {
        get
        {
            if (m_Position == Vector2.zero)
            {
                if (!string.IsNullOrEmpty(PosInMap))
                {
                    string[] arr = PosInMap.Split('_');
                    if (arr.Length >= 2)
                    {
                        float x = 0, y = 0;
                        float.TryParse(arr[0], out x);
                        float.TryParse(arr[1], out y);
                        m_Position = new Vector2(x,y);
                    }
                }
            }
            return m_Position;
        }
    }

    

}
