using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActorController : MonoBehaviour
{
    public GameObject model;//角色模型游戏物体
    public CameraController cameraController;
    private IUserInput pi;//用户输入模块
    public float walkSpeed = 2.4f;//行走速度
    public float runMultiplier = 2.0f;//跑步时的速度是行走时的多少倍
    public float jumpVelocity = 5.0f;//跳跃向上冲量
    public float rollVelocity = 3.0f;//翻滚冲量

    [Space(10)]
    [Header("==== Physic Setting ====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    public Animator Anim { get { return anim; } }
    private Rigidbody rigid;
    private CapsuleCollider capCol;//胶囊碰撞体
    private Vector3 planarVec;//XZ平面玩家刚体的速度向量
    private Vector3 thrustVec;//模拟跳跃的向上冲量
    // private float weightLerpTarget;//attack动画层目标权重
    private Vector3 deltaPos;//角色模型动画位移增量

    private bool lockPlanar = false;//为真时锁住平面的速度 这里的效果是锁住时保持之前的移动速度 也就是有惯性
    private bool trackDirection = false;//锁定敌人时只有地面移动时才将角色前方向锁定到目标方向
    private bool canAttack;

    public bool leftIsShield = true;//左手是否为盾

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
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);//角色模型朝向
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

    public void IsGround()//OnGroundSensor探测器
    {
        //anim.SetBool("isGround", true);
    }

    public void IsNotGround()//OnGroundSensor探测器
    {
        //anim.SetBool("isGround", false);
    }
}
