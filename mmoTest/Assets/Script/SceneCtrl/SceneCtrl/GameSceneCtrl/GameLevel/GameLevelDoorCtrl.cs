using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelDoorCtrl : MonoBehaviour 
{
    [SerializeField]
    public GameLevelDoorCtrl ConnectToDoor;//��������

    [HideInInspector]
    public int OwnerRegionId;//���������Id

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
