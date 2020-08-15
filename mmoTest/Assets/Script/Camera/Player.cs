using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    private Rigidbody rigid;
    public GameObject model;

    private float speed = 5f;
    private Vector3 target;

    public float Dup;//自定义垂直轴向 Direction Up
    public float Dright;//自定义水平轴向 Direction Right

    public float Dmag;//平方和开根号 Direction Magnitude
    public Vector3 Dvec;//向量运算 Direction Vector

    private float JUp;//视角操作垂直轴向 Joystick Up
    private float JRight;//视角操作水平朝向 Joystick Right

    //插值计算用到的中间变量
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

        //手柄左摇杆控制人物移动和朝向
        Dup = Input.GetAxis("Vertical");
        Dright = Input.GetAxis("Horizontal");

        //控制能否输入
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

    //椭圆映射法 将正方向的坐标系映射到一个圆上 这样来避免斜45°方向速度比坐标轴方向速度快的bug.
    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }
}
