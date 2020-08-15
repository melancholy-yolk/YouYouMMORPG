using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    private Transform targetTransform;//���������Ŀ��
    public Vector3 offset;//��Ŀ��λ�õ�ƫ����
    private Quaternion quaternion;

	void Update ()
    {
        if (GlobalInit.Instance.MainPlayer != null)
        {

            Vector3 targetPos = GlobalInit.Instance.MainPlayer.transform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);

            quaternion = Quaternion.FromToRotation(transform.forward, GlobalInit.Instance.MainPlayer.transform.forward);
            Quaternion.Slerp(transform.rotation, quaternion, 0.1f);
        }         
	}

    void FixedUpdate()
    {
        //transform.LookAt(GlobalInit.Instance.MainPlayer.transform);
    }
}
