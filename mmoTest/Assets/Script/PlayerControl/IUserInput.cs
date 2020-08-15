using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�û����������
public abstract class IUserInput : MonoBehaviour
{
    [Header("==== output signal ====")]//����ź�
    public float Dup;//�Զ��崹ֱ���� Direction Up
    public float Dright;//�Զ���ˮƽ���� Direction Right

    public float Dmag;//ƽ���Ϳ����� Direction Magnitude
    public Vector3 Dvec;//�������� Direction Vector

    public float JUp;//�ӽǲ�����ֱ���� Joystick Up
    public float JRight;//�ӽǲ���ˮƽ���� Joystick Right

    // 1.pressing signal
    public bool run;//�����ǳ����ź�:�տ�ʼ��������Ч һ��delay֮��Ż���Ч
    public bool defense;//�����ź�

    // 2.trigger once
    //��Ծ
    public bool action;
    public bool jump;
    protected bool lastJump;
    //����
    public bool attack;
    protected bool lastAttack;
    public bool roll;
    public bool lockOn;//�����ӽ�
    public bool lb;
    public bool lt;
    public bool rb;
    public bool rt;

    // 3.double triger

    [Header("==== others ====")]
    public bool inputEnabled = true;

    //��ֵ��������ʱ����
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

    //��Բӳ�䷨ �������������ϵӳ�䵽һ��Բ�� ����������б45�㷽���ٶȱ������᷽���ٶȿ��bug.
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
