using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用户输入抽象类
public abstract class IUserInput : MonoBehaviour
{
    [Header("==== output signal ====")]//输出信号
    public float Dup;//自定义垂直轴向 Direction Up
    public float Dright;//自定义水平轴向 Direction Right

    public float Dmag;//平方和开根号 Direction Magnitude
    public Vector3 Dvec;//向量运算 Direction Vector

    public float JUp;//视角操作垂直轴向 Joystick Up
    public float JRight;//视角操作水平朝向 Joystick Right

    // 1.pressing signal
    public bool run;//现在是长按信号:刚开始按不会生效 一定delay之后才会生效
    public bool defense;//防御信号

    // 2.trigger once
    //跳跃
    public bool action;
    public bool jump;
    protected bool lastJump;
    //攻击
    public bool attack;
    protected bool lastAttack;
    public bool roll;
    public bool lockOn;//锁定视角
    public bool lb;
    public bool lt;
    public bool rb;
    public bool rt;

    // 3.double triger

    [Header("==== others ====")]
    public bool inputEnabled = true;

    //插值计算用临时变量
    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;

    [Header("==== skill ====")]
    public bool skill1;
    public bool skill2;
    public bool skill3;
    public bool skill4;
    public bool skill5;
    public bool skill6;

    //椭圆映射法 将正方向的坐标系映射到一个圆上 这样来避免斜45°方向速度比坐标轴方向速度快的bug.
    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }

    protected void UpdateDmagDvec(float Dup2, float Dright2)
    {
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;
    }

}
