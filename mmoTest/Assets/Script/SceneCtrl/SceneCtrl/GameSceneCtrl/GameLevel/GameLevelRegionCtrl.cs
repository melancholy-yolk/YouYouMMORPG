using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡区域控制器
/// </summary>
public class GameLevelRegionCtrl : MonoBehaviour 
{
    public int RegionId;
    public int RegionIndex;

    [SerializeField]
    public Transform RoleBornPos;//主角出生点

    [SerializeField]
    public Transform[] MonsterBornPos;//怪物出生点

    [SerializeField]
    private GameLevelDoorCtrl[] AllDoor;//区域门

    public GameObject RegionMask;//区域遮挡物体

    void Awake()
    {
        if (MonsterBornPos != null && MonsterBornPos.Length > 0)
        {
            for (int i = 0; i < MonsterBornPos.Length; i++)
            {
                Renderer renderer = MonsterBornPos[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }

        if (AllDoor != null && AllDoor.Length > 0)
        {
            for (int i = 0; i < AllDoor.Length; i++)
            {
                AllDoor[i].OwnerRegionId = RegionId;
            }
        }

    }

    /// <summary>
    /// 获取下一个区域的门
    /// </summary>
    /// <param name="nextRegionId"></param>
    /// <returns></returns>
    public GameLevelDoorCtrl GetToNextRegionDoor(int nextRegionId)
    {
        if (AllDoor != null && AllDoor.Length > 0)
        {
            for (int i = 0; i < AllDoor.Length; i++)
            {
                if (AllDoor[i].ConnectToDoor.OwnerRegionId == nextRegionId)
                {
                    return AllDoor[i];
                }
            }
        }
        return null;
    }

	void Start () {
		
	}
	
	
	void Update () 
    {
       
    }

    

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (RoleBornPos != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(RoleBornPos.position, 0.5f);

            Gizmos.DrawLine(transform.position, RoleBornPos.position);
        }

        if (MonsterBornPos != null && MonsterBornPos.Length > 0)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < MonsterBornPos.Length; i++)
            {
                Gizmos.DrawSphere(MonsterBornPos[i].position, 0.5f);
                Gizmos.DrawLine(transform.position, MonsterBornPos[i].position);
            }
        }

        if (AllDoor != null && AllDoor.Length > 0)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < AllDoor.Length; i++)
            {
                Gizmos.DrawLine(transform.position, AllDoor[i].transform.position);
            }
        }
    }
#endif
}
