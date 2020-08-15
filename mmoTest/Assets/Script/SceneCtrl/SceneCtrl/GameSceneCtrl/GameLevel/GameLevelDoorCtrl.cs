using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelDoorCtrl : MonoBehaviour 
{
    [SerializeField]
    public GameLevelDoorCtrl ConnectToDoor;//关联的门

    [HideInInspector]
    public int OwnerRegionId;//所属区域的Id

	void Start () 
    {

    }

    

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (ConnectToDoor != null)
        {
            Gizmos.DrawLine(this.transform.position, ConnectToDoor.transform.position);
        }
    }
#endif

}
