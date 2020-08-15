using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    private Rigidbody rigid;
    public GameObject model;

    private float speed = 5f;
    private Vector3 target;

    public float Dup;//�Զ��崹ֱ���� Direction Up
    public float Dright;//�Զ���ˮƽ���� Direction Right

    public float Dmag;//ƽ���Ϳ����� Direction Magnitude
    public Vector3 Dvec;//�������� Direction Vector

    private float JUp;//�ӽǲ�����ֱ���� Joystick Up
    private float JRight;//�ӽǲ���ˮƽ���� Joystick Right

    //��ֵ�����õ����м����
    private float targetDup;
    private float targetDright;
    private float velocityDup;
    private float velocityDright;

    private Vector3 planarVec;

    private bool inputEnabled = true;
	
	void Start () {
        target = transform.position;
        rigid = GetComponent<Rigidbody>();
	}
	
	
	void Update () {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hitInfo;
        //    if (Physics.Raycast(ray, out hitInfo))
        //    {
        //        target = hitInfo.point;
        //    }
        //}

        //if (Vector3.Distance(transform.position, target) > 0.25f)
        //{
        //    target = new Vector3(target.x, transform.position.y, target.z);
        //    transform.LookAt(target);
        //    transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //}

        //�ֱ���ҡ�˿��������ƶ��ͳ���
        Dup = Input.GetAxis("Vertical");
        Dright = Input.GetAxis("Horizontal");

        //�����ܷ�����
        //if (inputEnabled == false)
        //{
        //    targetDup = 0;
        //    targetDright = 0;
        //}

        //Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        //Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        //Dup = targetDup;
        //Dright = targetDright;

        //Vector2 tempDirectionAxis = SquareToCircle(new Vector2(Dright, Dup));
        //float Dup2 = tempDirectionAxis.y;
        //float Dright2 = tempDirectionAxis.x;

        Dmag = Mathf.Sqrt((Dup * Dup) + (Dright * Dright));
        Dvec = Dright * transform.right + Dup * transform.forward;

        Vector3 dir = new Vector3(Dright, 0, Dup);
        transform.LookAt(transform.position + dir);
        transform.Translate(dir * 5 * Time.deltaTime, Space.World);
	}

    void FixedUpdate()
    {
        
    }

    //��Բӳ�䷨ �������������ϵӳ�䵽һ��Բ�� ����������б45�㷽���ٶȱ������᷽���ٶȿ��bug.
    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }
}
