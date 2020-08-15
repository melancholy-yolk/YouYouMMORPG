using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public float offset = 0.3f;

    private Vector3 pointBottom;
    private Vector3 pointTop;
    private float radius;

    void Awake()
    {
        radius = capcol.radius - 0.05f; ;

    }

    void FixedUpdate()
    {
        pointBottom = transform.position + transform.up * (radius - offset);
        pointTop = transform.position + transform.up * (capcol.height - offset) - transform.up * radius;

        Collider[] colArr = Physics.OverlapCapsule(pointBottom, pointTop, radius, LayerMask.GetMask("Ground"));
        if (colArr.Length != 0)
        {
            SendMessageUpwards("IsGround");
        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
}
