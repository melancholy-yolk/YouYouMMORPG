using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActorController : MonoBehaviour
{
    public GameObject model;//��ɫģ����Ϸ����
    public CameraController cameraController;
    private IUserInput pi;//�û�����ģ��
    public float walkSpeed = 2.4f;//�����ٶ�
    public float runMultiplier = 2.0f;//�ܲ�ʱ���ٶ�������ʱ�Ķ��ٱ�
    public float jumpVelocity = 5.0f;//��Ծ���ϳ���
    public float rollVelocity = 3.0f;//��������

    [Space(10)]
    [Header("==== Physic Setting ====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    public Animator Anim { get { return anim; } }
    private Rigidbody rigid;
    private CapsuleCollider capCol;//������ײ��
    private Vector3 planarVec;//XZƽ����Ҹ�����ٶ�����
    private Vector3 thrustVec;//ģ����Ծ�����ϳ���
    // private float weightLerpTarget;//attack������Ŀ��Ȩ��
    private Vector3 deltaPos;//��ɫģ�Ͷ���λ������

    private bool lockPlanar = false;//Ϊ��ʱ��סƽ����ٶ� �����Ч������סʱ����֮ǰ���ƶ��ٶ� Ҳ�����й���
    private bool trackDirection = false;//��������ʱֻ�е����ƶ�ʱ�Ž���ɫǰ����������Ŀ�귽��
    private bool canAttack;

    public bool leftIsShield = true;//�����Ƿ�Ϊ��

    public delegate void OnActionDelegate();
    public event OnActionDelegate OnAction;

    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled == true)
            {
                pi = input;
                break;
            }
        }

        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);//��ɫģ�ͳ���
        }
        if (lockPlanar == false)
        {
            planarVec = pi.Dmag * model.transform.forward * walkSpeed * (pi.run ? runMultiplier : 1.0f);//magnitude
        }
        anim.SetFloat("forward", pi.Dmag);
        if (pi.attack)
        {
            anim.SetTrigger("attack");
        }

        if (pi.skill1)
        {
            anim.SetTrigger("skill1");
        }
        if (pi.skill2)
        {
            anim.SetTrigger("skill2");
        }
        if (pi.skill3)
        {
            anim.SetTrigger("skill3");
        }
        if (pi.skill4)
        {
            anim.SetTrigger("skill4");
        }
        if (pi.skill5)
        {
            anim.SetTrigger("skill5");
        }
        if (pi.skill6)
        {
            anim.SetTrigger("skill6");
        }
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z);
    }

    public void IsGround()//OnGroundSensor̽����
    {
        //anim.SetBool("isGround", true);
    }

    public void IsNotGround()//OnGroundSensor̽����
    {
        //anim.SetBool("isGround", false);
    }
}
