using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(FunnelModifier))]
public class RoleCtrl : MonoBehaviour
{
    #region ����

    #region ͷ��Ѫ���ͳƺ�
    public Transform m_HeadBarPos;//��ɫͷ�����ƹҵ�
    private GameObject m_HeadBar;//��ɫͷ����������
    #endregion

    [HideInInspector]
    public Animator m_Animator;

    #region ��ɫ�ƶ�
    [HideInInspector]
    public Vector3 TargetPos;//��ɫҪǰ����Ŀ��λ��
    [HideInInspector]
    public CharacterController m_CharacterController;//��ɫ������
    [HideInInspector]
    public float Speed = 10f;//�ƶ��ٶ�
    [HideInInspector]
    public float m_RotateSpeed = 0.2f;//ת���ٶ�
    private Quaternion m_TargetQuaternion;
    #endregion

    [HideInInspector]
    public Vector3 BornPoint;//��ɫ������

    public float ViewRange = 6;//��Ұ��Χ
    public float PatrolRange = 10;//Ѳ�߷�Χ
    public float AttackRange = 3;//ÿ�����ܵĹ�����Χ����ͬ Ҫ���ݼ��ܱ�Ŷ�ȡ

    public RoleAttackInfo CurrAttackInfo;//��ǰ����ʹ��

    #region ��ɫ��Ϣ
    [HideInInspector]
    public RoleType CurrRoleType;//��ɫ����
    public RoleInfoBase CurrRoleInfo;//��ɫ��Ϣ
    public IRoleAI CurrRoleAI = null;
    [HideInInspector]
    public RoleCtrl LockEnemy;//�����ĵ���
    private bool m_IsInit = false;//��ɫ�Ƿ��Ѿ���ʼ��
    public bool IsDied;//��ɫ�Ƿ��Ѿ�����
    #endregion

    [HideInInspector]
    public RoleFSMMgr CurrRoleFSMMgr = null;//����״̬��������
    private RoleHeadBarView roleHeadBarView = null;//��ɫѪ��UI

    #region ��ɫA��Ѱ·
    private Seeker seeker;//����Ѱ·����
    public Seeker Seeker
    {
        get
        {
            return seeker;
        }
    }
    [HideInInspector]
    public ABPath AStarPath;//·��
    [HideInInspector]
    public float nextWaypointDistance = 3;//���¸�Ŀ���ľ���
    [HideInInspector]
    public int AStarCurrentWaypoint = 0;//��ǰҪȥ��·�������
    [HideInInspector]
    public float roleSlerp = 0f;//ת���ٶ�
    [HideInInspector]
    public AstarPath AstarPathController;
    #endregion

    #region ��ɫս��
    //========== ս����� ==========
    public RoleAttack Attack;//�������߼� ��ֵ���� �Ӿ�����
    private RoleHurt m_Hurt;//���������߼� ��ֵ���� �Ӿ�����

    public Action<RoleCtrl> OnDieHandler;//�������ģ���ɫ��������߼�Ҫע�������ί����
    public Action<Transform> OnDestroyHandler;//�������ģ���ɫ��������߼�Ҫע�������ί����

    [HideInInspector]
    public bool IsRigidity;//�Ƿ��ڽ�ֱ״̬�������д��ڽ�ֱ״̬��

    [HideInInspector]
    public float PreviousFightTime = 0f;//�ϴη���ս����ʱ�� ������������������л�

    

    public delegate void OnValueChangeHandler(int type);//��ֵ�仯ί�ж���
    public OnValueChangeHandler OnHPChange;//��ɫHP�ı�ί��
    public OnValueChangeHandler OnMPChange;//��ɫMP�ı�ί��
    #endregion

    #endregion

    #region ��ɫ���ݳ�ʼ��
    public void Init(RoleType roleType, RoleInfoBase roleInfo, IRoleAI ai)
    {
        CurrRoleType = roleType;
        CurrRoleInfo = roleInfo;
        CurrRoleAI = ai;

        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }

        m_IsInit = true;
    }
    #endregion

    #region ������������
    void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        CurrRoleFSMMgr = new RoleFSMMgr(this, OnRoleDieCallBack, OnRoleDestroyCallBack);
    }

    void Start () 
    {
        //��ɫ�����³��� ��A��Ѱ·�����ֵ
        if ((SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel || SceneMgr.Instance.CurrentSceneType == SceneType.WorldMap) && AstarPathController == null)
        {
            AstarPathController = GameObject.Find("A*").GetComponent<AstarPath>();
        }
        seeker = GetComponent<Seeker>();
        
        //��������� �������������
        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.Init();
            }
        }

        //�����ɫ�����߼���
        m_Hurt = new RoleHurt(CurrRoleFSMMgr);
        m_Hurt.OnRoleHurt = OnRoleHurtCallBack;

        //�����ɫ�����߼���
        Attack.SetFSM(CurrRoleFSMMgr);
	}

    

    #region ��ɫ״̬�ı�ص�
    /// <summary>
    /// ��ɫ���ٻص�
    /// </summary>
    private void OnRoleDestroyCallBack()
    {
        //��ɫ����һ��ʱ��� Ҫ�ѽ�ɫ���յ�������� ���չ������ɽ�ɫ�Լ�����
        if (OnDestroyHandler != null)
        {
            OnDestroyHandler(this.transform);
        }
    }

    /// <summary>
    /// ��ɫ�����ص�
    /// </summary>
    private void OnRoleDieCallBack()
    {
        //��ɫ���������ٱ����
        //��ɫ��������ý�ɫ���������
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = false;
        }

        //��ɫ������HeadBarҲҪ����
        if (roleHeadBarView != null)
        {
            Destroy(roleHeadBarView.gameObject);
        }

        //ִ������ģ�����ɫ������صķ���
        if (OnDieHandler != null)
        {
            OnDieHandler(this);
        }
    }

    /// <summary>
    /// ��ɫ���˻ص�
    /// </summary>
    private void OnRoleHurtCallBack()
    {
        if (roleHeadBarView != null)
        {
            roleHeadBarView.SetSliderHP((float)this.CurrRoleInfo.CurrHP/this.CurrRoleInfo.MaxHP);
        }
        if (OnHPChange != null)
        {
            OnHPChange(-1);
        }
    }
    #endregion

    #endregion

    #region ��ɫ���� ���ý�ɫ����ʱ�ڳ����е�λ�úͳ��� ��ʼ��HeadBar ��ʼ�����������
    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="pos">����������</param>
    public void Born(Vector3 pos)
    {
        BornPoint = pos;
        transform.position = pos;
        InitHeadBar();//ʵ����Ѫ�������� ����ֵ
        ToIdle();

        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraCtrl.Instance != null)
            {
                CameraCtrl.Instance.Init();
            }
            
        }
    }
    #endregion

    void Update () 
    {
        
        //�����ɫû��AI ֱ�ӷ���
        if (CurrRoleAI != null)
        {
            CurrRoleAI.DoAI();
        }

        if (CurrRoleFSMMgr != null)
        {
            CurrRoleFSMMgr.OnUpdate();
        }

        //��ɫÿ�γ��� ��ʼ��һ��״̬
        if (m_IsInit)
        {
            m_IsInit = false;

            if (CurrRoleInfo.CurrHP <= 0)
            {
                ToDie(isDied:true);
            }
            else
            {
                if (CurrRoleType == RoleType.Monster)//����ֻ��ս������
                {
                    ToIdle(RoleIdleState.IdleFight);
                }
                else
                {
                    ToIdle();//Ĭ��ΪIdle״̬
                }
            }
            
        }

        if (m_CharacterController == null) return;

        //�ý�ɫ���ŵ���
        //if (m_CharacterController.isGrounded == false)
        //{
        //    m_CharacterController.Move((transform.position + new Vector3(0, -1000, 0)) - transform.position);
        //}

        //�������������
        if (CurrRoleType == RoleType.MainPlayer)
        {
            CameraAutoFollow();
            AutoSmallMap();
        }

        
	}

    #region С��ͼ
    /// <summary>
    /// С��ͼ����߼�
    /// </summary>
    private void AutoSmallMap()
    {
        if (SmallMapHelper.Instance == null || UIMainCitySmallMapView.Instance == null) return;

        SmallMapHelper.Instance.gameObject.transform.position = transform.position;
        Transform trans = SmallMapHelper.Instance.gameObject.transform;
        UIMainCitySmallMapView.Instance.imageSmallMap.transform.localPosition = new Vector3(trans.localPosition.x * -512, trans.localPosition.z * -512, 1);
        UIMainCitySmallMapView.Instance.imageArrow.transform.localEulerAngles = new Vector3(0, 0, 360 - transform.eulerAngles.y);
    }
    #endregion

    #region HeadBar
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (CurrRoleInfo == null) return;
        if (m_HeadBarPos == null) return;

        m_HeadBar = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "MainCity", "RoleHeadBar");
        
        m_HeadBar.transform.SetParent(RoleHeadBarRoot.Instance.gameObject.transform);
        m_HeadBar.transform.localScale = Vector3.one;
        m_HeadBar.transform.localPosition = Vector3.zero;

        roleHeadBarView = m_HeadBar.GetComponent<RoleHeadBarView>();
        roleHeadBarView.Init(m_HeadBarPos, CurrRoleInfo.RoleNickName, isShowHPBar: (CurrRoleType == RoleType.Monster ? true : false), sliderValue:(float)this.CurrRoleInfo.CurrHP/this.CurrRoleInfo.MaxHP);
    }
    #endregion

    #region ���ƽ�ɫ����

    public void ToIdle(RoleIdleState state = RoleIdleState.IdleNormal)
    {
        CurrRoleFSMMgr.ToIdleState = state;
        CurrRoleFSMMgr.ChangeState(RoleState.Idle);
    }

    public void ToRun()//��ʱ������
    {
        CurrRoleFSMMgr.ChangeState(RoleState.Run);
    }

    public void MoveTo(Vector3 targetPos)
    {
        if (CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die) return;
        if (IsRigidity) return;//�����в����ƶ�
        if (targetPos == Vector3.zero) return;//���Ŀ��㲻��ԭ�� �����ƶ�

        TargetPos = targetPos;

        if (this.seeker != null)
        {
            //���������յ� ��ʼA*Ѱ·���� �������ʱ����ص�OnPathComplete����
            this.seeker.StartPath(this.transform.position, TargetPos, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            AStarPath = (ABPath)p;
            if (Vector3.Distance(AStarPath.endPoint, new Vector3(AStarPath.originalEndPoint.x, AStarPath.endPoint.y, AStarPath.originalEndPoint.z)) > 0.5f)
            {
                Debug.Log("Can not arrive target point!");
                AStarPath = null;
                return;
            }

            if (CurrRoleType == RoleType.MainPlayer)
            {
                //PVP������Ϣ��������
                SendPVPMove(TargetPos, AStarPath.vectorPath);
            }

            AStarCurrentWaypoint = 1;
            CurrRoleFSMMgr.ChangeState(RoleState.Run);
        }
        else
        {
            AppDebug.Log("Astar find path fail!");
            AStarPath = null;
        }
    }

    private void SendPVPMove(Vector3 targetPos, List<Vector3> path)
    {
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
        {
            float pathLength = 0f;

            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1) continue;

                float dis = Vector3.Distance(path[i], path[i+1]);
                pathLength += dis;
            }

            float needTime = pathLength / Speed;//����·����Ҫ��ʱ��

            WorldMap_CurrRoleMoveProto proto = new WorldMap_CurrRoleMoveProto();
            proto.TargetPosX = targetPos.x;
            proto.TargetPosY = targetPos.y;
            proto.TargetPosZ = targetPos.z;
            proto.ServerTime = GlobalInit.Instance.GetCurrServerTime();
            proto.NeedTime = (int)(needTime * 1000);

            NetWorkSocket.Instance.SendMsg(proto.ToArray());
        }
    }

    public void ToAttackByIndex(RoleAttackType type, int index)
    {
        Attack.ToAttackByIndex(type, index);
    }

    /// <summary>
    /// �����ý�ɫ�Ĺ����ӿ� ����ʵ���߼���RoleAttack����д���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public bool ToAttackBySkillId(RoleAttackType type, int skillId)
    {
        return Attack.ToAttack(type, skillId);
    }

    /// <summary>
    /// PVP�������㲥 �ù������ݳ�������������Ч ��������������
    /// </summary>
    public void PlayAttack(int skillId)
    {
        Attack.PlayAttack(skillId);
    }

    public void ToHurt(RoleTransferAttackInfo attackInfo)
    {
        StartCoroutine(m_Hurt.ToHurt(attackInfo));
    }

    public void ToDie(bool isDied = false)
    {
        IsDied = isDied;
        CurrRoleInfo.CurrHP = 0;
        CurrRoleFSMMgr.ChangeState(RoleState.Die);
    }

    public void ToSelect()
    {
        CurrRoleFSMMgr.ChangeState(RoleState.Select);
    }
    #endregion

    #region OnDestroy ����
    /// <summary>
    /// ����
    /// </summary>
    void OnDestroy()
    {
        if (m_HeadBar != null)
        {
            Destroy(m_HeadBar);
        }
    }
    #endregion

    #region CameraAutoFollow ������Զ�����
    /// <summary>
    /// ������Զ�����
    /// </summary>
    private void CameraAutoFollow()
    {
        if (CameraCtrl.Instance == null) return;

        CameraCtrl.Instance.transform.position = transform.position;
        CameraCtrl.Instance.AutoLookAt(transform.position);
    }
    #endregion

    void OnGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CurrAttackInfo.AttackRange);
    }

    public void RoleReborn()//��ɫ����
    {
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }

        PreviousFightTime = 0;
        CurrRoleInfo.CurrHP = CurrRoleInfo.MaxHP;
        CurrRoleInfo.CurrMP = CurrRoleInfo.MaxMP;
        LockEnemy = null;

        if (OnHPChange != null)
        {
            OnHPChange(1);
        }

        if (OnMPChange != null)
        {
            OnMPChange(1);
        }

        ToIdle();
    }

    /// <summary>
    /// PVP��ͬ����������ƶ��������ٶ� Ҫ��������ҵ�ʵ���ƶ��ٶȿ� Ŀ����Ϊ����׷���� �������ʵ�ʵ��ƶ�
    /// </summary>
    public float ModifySpeed { get; set; }
}
