using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ����������
/// </summary>
public class GameLevelRegionCtrl : MonoBehaviour 
{
    public int RegionId;
    public int RegionIndex;

    [SerializeField]
    public Transform RoleBornPos;//���ǳ�����

    [SerializeField]
    public Transform[] MonsterBornPos;//���������

    [SerializeField]
    private GameLevelDoorCtrl[] AllDoor;//������

    public GameObject RegionMask;//�����ڵ�����

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
    /// ��ȡ��һ���������
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
