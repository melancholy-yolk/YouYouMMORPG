using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WorldMapEntity
{
    private Vector3 m_OriginRoleBirthPos = Vector3.zero;

    public Vector3 OriginRoleBirthPos
    {
        get
        {
            if (m_OriginRoleBirthPos == Vector3.zero)
            {
                string[] arr = RoleBirthPos.Split('_');
                if (arr.Length < 3)
                {
                    return Vector3.zero;
                }
                float.TryParse(arr[0], out m_OriginRoleBirthPos.x);
                float.TryParse(arr[1], out m_OriginRoleBirthPos.y);
                float.TryParse(arr[2], out m_OriginRoleBirthPos.z);
            }
            return m_OriginRoleBirthPos;
        }
    }

    private float m_RoleBirthEulerAnglesY = -1;

    public float RoleBirthEulerAnglesY
    {
        get
        {
            if (m_RoleBirthEulerAnglesY == -1)
            {
                string[] arr = RoleBirthPos.Split('_');
                if (arr.Length < 4)
                {
                    return 0;
                }
                float.TryParse(arr[0], out m_RoleBirthEulerAnglesY);
            }

            return m_RoleBirthEulerAnglesY;
        }
    }

    private List<NPCWorldMapData> m_NPCWorldMapList;

    /// <summary>
    /// 世界地图场景中 NPC列表
    /// </summary>
    public List<NPCWorldMapData> NPCWorldMapList
    {
        get
        {
            if (m_NPCWorldMapList == null)
            {
                m_NPCWorldMapList = new List<NPCWorldMapData>();
                string[] arr1 = NPCList.Split('|');
                for (int i = 0; i < arr1.Length; i++)
                {
                    string[] arr2 = arr1[i].Split('_');
                    if (arr2.Length < 6)
                    {
                        break;
                    }
                    int npcId = 0;
                    int.TryParse(arr2[0], out npcId);

                    float x = 0, y = 0, z = 0, anglesY = 0;
                    float.TryParse(arr2[1], out x);
                    float.TryParse(arr2[2], out y);
                    float.TryParse(arr2[3], out z);
                    float.TryParse(arr2[4], out anglesY);

                    string prologue = arr2[5];
                    NPCWorldMapData data = new NPCWorldMapData();
                    data.NPCId = npcId;
                    data.NPCPosition = new Vector3(x,y,z);
                    data.NPCRotationY = anglesY;
                    data.NPCPrologue = prologue;
                    m_NPCWorldMapList.Add(data);
                }
            }
            return m_NPCWorldMapList;
        }
    }

    private List<TransPointWorldMapData> m_TransPointList;
    public List<TransPointWorldMapData> TransPointList
    {
        get
        {
            if (m_TransPointList == null)
            {
                m_TransPointList = new List<TransPointWorldMapData>();
                //传送点（坐标_y轴旋转_传送点编号_要传送的场景Id_目标场景出生传送点id）
                string[] transPointArr = TransPos.Split('|');
                for (int i = 0; i < transPointArr.Length; i++)
                {
                    string[] transPointInfoArr = transPointArr[i].Split('_');

                    float x = 0, y = 0, z = 0, rotationY = 0;
                    int transPointId = 0, targetSceneId = 0, targetSceneTransPointId = 0;

                    float.TryParse(transPointInfoArr[0], out x);
                    float.TryParse(transPointInfoArr[1], out y);
                    float.TryParse(transPointInfoArr[2], out z);
                    float.TryParse(transPointInfoArr[3], out rotationY);
                    int.TryParse(transPointInfoArr[4], out transPointId);
                    int.TryParse(transPointInfoArr[5], out targetSceneId);
                    int.TryParse(transPointInfoArr[6], out targetSceneTransPointId);

                    TransPointWorldMapData transPointData = new TransPointWorldMapData(x, y, z, rotationY, transPointId, targetSceneId, targetSceneTransPointId);
                    m_TransPointList.Add(transPointData);
                }
            }
            return m_TransPointList;
        }
    }
}
