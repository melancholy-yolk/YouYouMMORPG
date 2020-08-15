using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �����ͼ ���ǳ��������� �������ǳ�ʼ�� NPC��ʼ��
/// </summary>
public class WorldMapSceneCtrl : GameSceneCtrlBase
{
    public static WorldMapSceneCtrl Instance;//����

    #region ����
    [SerializeField]
    private Transform m_PlayerBornPos;//���ǳ�����

    private WorldMapEntity CurrWorldMapEntity;//��ǰ�����ͼ����excel����ʵ��

    private Dictionary<int, WorldMapTransCtrl> m_TransPointDic;//����˳��������еĴ��͵�
    private Dictionary<int, RoleCtrl> m_AllRoleDic;//��ǰ���������н�ɫ

    private WorldMap_PosProto m_WorldMap_PosProto = new WorldMap_PosProto();
    private float m_NextSendTime = 0f;//�������ͬ������λ�õļ�ʱ��
    #endregion

    public void MainPlayerLeaveScene()
    {
        m_AllRoleDic.Remove(GlobalInit.Instance.MainPlayer.CurrRoleInfo.RoleId);
    }

    #region ��ʼ�������ͼ����
    /// <summary>
    /// ����UI������ɻص� UI������ɺ� ��ʹ����������UI
    /// </summary>
    protected override void OnUISceneMainCityViewLoadComplete()
    {
        base.OnUISceneMainCityViewLoadComplete();

        if (GlobalInit.Instance == null) return;

        RoleMgr.Instance.InitMainPlayer();//ʵ��������ģ�� ��ʼ����������
        
        if (GlobalInit.Instance.MainPlayer != null)//�������ǳ�����
        {
            CurrWorldMapEntity = WorldMapDBModel.Instance.GetEntity(SceneMgr.Instance.CurrWorldMapId);

            InitTransPoint();//����excel������� ��ʼ�������еĴ��͵�

            #region ���������ڳ����е�λ��
            if (SceneMgr.Instance.TargetWorldMapTransPointId == 0)//δ�������Ŀ�곡�����͵�Id
            {
                
                //��������������˿ͻ���������������ͼ�����е�λ����Ϣ
                if (!string.IsNullOrEmpty(PlayerController.Instance.LastInWorldMapPos))
                {
                    #region �������ϴε�¼������ڵ�λ��
                    string[] arr = PlayerController.Instance.LastInWorldMapPos.Split('_');
                    Vector3 pos = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));

                    if (pos == Vector3.zero)
                    {
                        if (CurrWorldMapEntity.OriginRoleBirthPos != Vector3.zero)
                        {
                            GlobalInit.Instance.MainPlayer.Born(CurrWorldMapEntity.OriginRoleBirthPos);
                            GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, CurrWorldMapEntity.RoleBirthEulerAnglesY, 0);
                        }
                        else
                        {
                            GlobalInit.Instance.MainPlayer.Born(m_PlayerBornPos.position);
                        }
                    }
                    else
                    {
                        GlobalInit.Instance.MainPlayer.Born(pos);
                        GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, float.Parse(arr[3]), 0);
                    }
                    #endregion
                }
                else
                {
                    #region ������excel�����õ�λ��
                    if (CurrWorldMapEntity.OriginRoleBirthPos != Vector3.zero)
                    {
                        GlobalInit.Instance.MainPlayer.Born(CurrWorldMapEntity.OriginRoleBirthPos);
                        GlobalInit.Instance.MainPlayer.transform.eulerAngles = new Vector3(0, CurrWorldMapEntity.RoleBirthEulerAnglesY, 0);
                    }
                    else
                    {
                        GlobalInit.Instance.MainPlayer.Born(m_PlayerBornPos.position);
                    }
                    #endregion
                }
            }
            else//������Ŀ�곡�����͵�Id �����ڴ��͵㸽��
            {
                #region �����ڴ��͵��Ա�
                if (m_TransPointDic.ContainsKey(SceneMgr.Instance.TargetWorldMapTransPointId))
                {
                    Vector3 newBornPoint = m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.forward.normalized * 3 + m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.position;
                    Vector3 lookAtPoint = m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.forward.normalized * 4 + m_TransPointDic[SceneMgr.Instance.TargetWorldMapTransPointId].transform.position;
                    
                    GlobalInit.Instance.MainPlayer.Born(newBornPoint);
                    GlobalInit.Instance.MainPlayer.transform.LookAt(lookAtPoint);

                    SceneMgr.Instance.TargetWorldMapTransPointId = 0;
                }
                #endregion
            }
            #endregion

            SendRoleAlreadyEnter(SceneMgr.Instance.CurrWorldMapId, GlobalInit.Instance.MainPlayer.transform.position, GlobalInit.Instance.MainPlayer.transform.eulerAngles.y);

            //���ó�����פUI������(������Ϣ ����)
            PlayerController.Instance.SetMainCityRoleData();//controller--->view ������ʹ�����ݸ���ͼ��ֵ
        }

        if (DelegateDefine.Instance.OnSceneLoadOK != null) DelegateDefine.Instance.OnSceneLoadOK();//ִ�г���������ɻص�

        StartCoroutine(InitNPC());//��ȡexcel��� ˢNPC
        AutoMove();//�����ͼ�糡��Ѱ·
    }

    #region ��ʼ��NPC
    /// <summary>
    /// ����excel���� �˳��������е�NPC
    /// </summary>
    private IEnumerator InitNPC()
    {
        yield return null;//�ȴ�һ֡

        if (CurrWorldMapEntity.NPCWorldMapList == null) yield break;

        for (int i = 0; i < CurrWorldMapEntity.NPCWorldMapList.Count; i++)
        {
            NPCWorldMapData data = CurrWorldMapEntity.NPCWorldMapList[i];
            NPCEntity npcEntity = NPCDBModel.Instance.GetEntity(data.NPCId);

            string prefabName = npcEntity.PrefabName;
            GameObject obj = RoleMgr.Instance.LoadNPC(prefabName);
            obj.transform.position = data.NPCPosition;
            obj.transform.eulerAngles = new Vector3(0, data.NPCRotationY, 0);

            NPCCtrl npcCtrl = obj.GetComponent<NPCCtrl>();
            npcCtrl.Init(data);
        }
    }
    #endregion

    #region ��ʼ�����͵�
    /// <summary>
    /// ��ʼ�����͵�
    /// </summary>
    private void InitTransPoint()
    {
        if (m_TransPointDic == null)
        {
            m_TransPointDic = new Dictionary<int, WorldMapTransCtrl>();
        }
        for (int i = 0; i < CurrWorldMapEntity.TransPointList.Count; i++)
        {
            TransPointWorldMapData data = CurrWorldMapEntity.TransPointList[i];

            //ʵ�������͵�
            GameObject obj = ResourcesManager.Instance.LoadCommonPrefab("Effect_Trans");
            obj.transform.position = data.TransPointPos;
            obj.transform.localEulerAngles = new Vector3(0,data.RotationY,0);

            //���ô��͵�����
            WorldMapTransCtrl ctrl = obj.GetComponent<WorldMapTransCtrl>();
            if (ctrl != null)
            {
                ctrl.SetParam(data);
            }

            m_TransPointDic[data.TransPointId] = ctrl;
        }
    }
    #endregion

    #endregion

    #region ��������
    protected override void OnAwake()
    {
        base.OnAwake();

        Instance = this;
    }

    protected override void OnStart()
    {
        base.OnStart();

        if (GlobalInit.Instance.MainPlayer != null) GlobalInit.Instance.MainPlayer.Attack.IsAutoFight = false;//�ص������ͼʱ �رս�ɫ�Զ�ս��

        m_TransPointDic = new Dictionary<int, WorldMapTransCtrl>();
        m_AllRoleDic = new Dictionary<int, RoleCtrl>();

        AddEventListener();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time > m_NextSendTime)
        {
            m_NextSendTime += 1f;
            SendPlayerPos();
        }
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();

        RemoveEventListener();
    }
    #endregion

    #region �����ͼ�糡��Ѱ·�ƶ�
    /// <summary>
    /// �����ͼ�糡��Ѱ·�ƶ�
    /// </summary>
    public void AutoMove()
    {
        if (!WorldMapController.Instance.IsAutoMove) return;
        if (SceneMgr.Instance.CurrWorldMapId == WorldMapController.Instance.ToSceneId)
        {
            //������Ŀ�곡�� ǰ��������Ŀ��λ��
            AppDebug.Log("arrive target scene");
            if (WorldMapController.Instance.ToScenePos != Vector3.zero)
            {
                GlobalInit.Instance.MainPlayer.MoveTo(WorldMapController.Instance.ToScenePos);
            }
            
            WorldMapController.Instance.IsAutoMove = false;
            return;
        }
        //�ҵ���ǰ������ ����Ҫǰ���Ĵ��͵�
        foreach (var item in m_TransPointDic)
        {
            if (item.Value.TargetTransSceneId == WorldMapController.Instance.ToSceneId)
            {
                //������͵�������Ҫ�ߵ�
                //�������ƶ������͵�
                GlobalInit.Instance.MainPlayer.MoveTo(item.Value.transform.position);
            }
        }
        WorldMapController.Instance.CurrSceneId = WorldMapController.Instance.ToSceneId;
        if (WorldMapController.Instance.SceneIdQueue.Count > 0)
        {
            WorldMapController.Instance.ToSceneId = WorldMapController.Instance.SceneIdQueue.Dequeue();
        }
    }
    #endregion

    #region ÿ��1s����������ϴ����ǵ�ǰ�������ͼ�����е�λ����Ϣ
    private void SendPlayerPos()//�������ͬ������λ����Ϣ
    {
        if (GlobalInit.Instance != null && GlobalInit.Instance.MainPlayer != null)
        {
            m_WorldMap_PosProto.x = GlobalInit.Instance.MainPlayer.transform.position.x;
            m_WorldMap_PosProto.y = GlobalInit.Instance.MainPlayer.transform.position.y;
            m_WorldMap_PosProto.z = GlobalInit.Instance.MainPlayer.transform.position.z;
            m_WorldMap_PosProto.yAngle = GlobalInit.Instance.MainPlayer.transform.eulerAngles.y;
            NetWorkSocket.Instance.SendMsg(m_WorldMap_PosProto.ToArray());
        }
    }
    #endregion

    #region �ͻ��˷����Ѿ����������ͼ������Ϣ
    private WorldMap_RoleAlreadyEnterProto m_RoleAlreadyEnterProto = new WorldMap_RoleAlreadyEnterProto();
    private void SendRoleAlreadyEnter(int worldMapSceneId, Vector3 currRolePos, float currRoleYAngle)
    {
        m_AllRoleDic[GlobalInit.Instance.MainPlayer.CurrRoleInfo.RoleId] = GlobalInit.Instance.MainPlayer;//���Լ����뵱ǰ�����Ľ�ɫ�ֵ�

        m_RoleAlreadyEnterProto.TargetWorldMapSceneId = worldMapSceneId;
        m_RoleAlreadyEnterProto.RolePosX = currRolePos.x;
        m_RoleAlreadyEnterProto.RolePosY = currRolePos.y;
        m_RoleAlreadyEnterProto.RolePosZ = currRolePos.z;
        m_RoleAlreadyEnterProto.RoleYAngle = currRoleYAngle;

        NetWorkSocket.Instance.SendMsg(m_RoleAlreadyEnterProto.ToArray());
    }
    #endregion

    private void AddEventListener()
    {
        //���������ص�ǰ�����е��������
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMap_InitRole);

        //�������㲥��������ҽ����˳���
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMap_OtherRoleEnter);
        //�������㲥����������뿪�˳���
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMap_OtherRoleLeave);
        //�������㲥����������ƶ�
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMap_OtherRoleMove);
        //�������㲥���������ʹ�ü���
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMap_OtherRoleUseSkill);
        //�������㲥�����������
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMap_OtherRoleDie);
        //�������㲥������Ҹ���
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMap_OtherRoleResurgence);
        //�������㲥���������Ϣ����
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUpdateInfo, OnWorldMap_OtherRoleUpdateInfo);
    }
    
    /// <summary>
    /// ��ʼ���������Ѿ����ڵ��������
    /// </summary>
    private void OnWorldMap_InitRole(byte[] buffer)
    {
        WorldMap_InitRoleProto proto = WorldMap_InitRoleProto.GetProto(buffer);
        int roleCount = proto.RoleCount;
        Debug.Log("other role count=" + roleCount);

        List<WorldMap_InitRoleProto.RoleItem> list = proto.ItemList;
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            int roleId = list[i].RoleId;
            string roleNickName = list[i].RoleNickName;
            int roleLevel = list[i].RoleLevel;
            int roleJobId = list[i].RoleJobId;
            Vector3 rolePos = new Vector3(list[i].RolePosX, list[i].RolePosY, list[i].RolePosZ);
            float roleYAngle = list[i].RoleYAngle;
            int maxHP = list[i].RoleMaxHP;
            int currHP = list[i].RoleCurrHP;
            int maxMP = list[i].RoleMaxMP;
            int currMP = list[i].RoleCurrMP;
            Debug.Log("init scene role currhp==="+currHP);

            CreateOtherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, maxHP, currHP, maxMP, currMP);
        }
    }

    /// <summary>
    /// �������㲥ͬ��������������ҽ���
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleEnter(byte[] buffer)
    {
        WorldMap_OtherRoleEnterProto proto = WorldMap_OtherRoleEnterProto.GetProto(buffer);

        int roleId = proto.RoleId;
        string roleNickName = proto.RoleNickName;
        int roleLevel = proto.RoleLevel;
        int roleJobId = proto.RoleJobId;
        Vector3 rolePos = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
        float roleYAngle = proto.RoleYAngle;
        int maxHP = proto.RoleMaxHP;
        int currHP = proto.RoleCurrHP;
        int maxMP = proto.RoleMaxMP;
        int currMP = proto.RoleCurrMP;

        CreateOtherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, maxHP, currHP, maxMP, currMP);
    }

    /// <summary>
    /// �������㲥ͨ����������������뿪
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleLeave(byte[] buffer)
    {
        WorldMap_OtherRoleLeaveProto proto = WorldMap_OtherRoleLeaveProto.GetProto(buffer);
        int leaveRoleId = proto.RoleId;
        DestroyOtherRole(leaveRoleId);
    }

    /// <summary>
    /// �������㲥ͬ����������������ƶ�
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleMove(byte[] buffer)
    {
        WorldMap_OtherRoleMoveProto proto = WorldMap_OtherRoleMoveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        Vector3 targetPos = new Vector3(proto.TargetPosX, proto.TargetPosY, proto.TargetPosZ);
        long serverTime = proto.ServerTime;
        int needTime = proto.NeedTime;

        if (m_AllRoleDic.ContainsKey(roleId))
        {
            //m_AllRoleDic[roleId].MoveTo(targetPos);
            ((OtherRoleAI)m_AllRoleDic[roleId].CurrRoleAI).MoveTo(targetPos, serverTime, needTime);
        }
    }

    /// <summary>
    /// �������㲥�������ʹ�ü���
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_OtherRoleUseSkill(byte[] buffer)
    {
        WorldMap_OtherRoleUseSkillProto proto = WorldMap_OtherRoleUseSkillProto.GetProto(buffer);

        //1.��������
        //�ù�����ʹ�ü���
        //��������ߴ���
        if (m_AllRoleDic.ContainsKey(proto.AttackRoleId))
        {
            RoleCtrl attackRole = m_AllRoleDic[proto.AttackRoleId];

            //���������ߵ�λ��
            attackRole.transform.position = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
            attackRole.transform.eulerAngles = new Vector3(0, proto.RoleYAngle, 0);

            //�����߱��ݹ���
            attackRole.PlayAttack(proto.SkillId);
        }

        //2.����������
        if (proto.ItemList != null && proto.ItemList.Count > 0)
        {
            for (int i = 0; i < proto.ItemList.Count; i++)
            {
                WorldMap_OtherRoleUseSkillProto.BeAttackItem item = proto.ItemList[i];//�õ���������
                if (m_AllRoleDic.ContainsKey(item.BeAttackRoleId))
                {
                    RoleCtrl beAttackRole = m_AllRoleDic[item.BeAttackRoleId];

                    RoleTransferAttackInfo attackInfo = new RoleTransferAttackInfo();
                    attackInfo.AttackRoleId = proto.AttackRoleId;//���𹥻��߱��
                    attackInfo.AttackRolePos = Vector3.zero;//�����ߵ�λ��
                    attackInfo.BeAttackedRoleId = item.BeAttackRoleId;//�������߱��
                    attackInfo.HurtValue = item.ReduceHp;//�˺�ֵ
                    attackInfo.SkillId = proto.SkillId;//������ʹ�õļ��ܱ��
                    attackInfo.SkillLevel = proto.SkillLevel;//������ʹ�õļ��ܵȼ�
                    attackInfo.IsAbnormal = false;//�Ƿ񸽼��쳣״̬
                    attackInfo.IsCri = item.IsCri == 1;//�Ƿ񱩻�

                    //���������ߴ��ݹ�����Ϣ
                    beAttackRole.ToHurt(attackInfo);
                }
            }
        }
    }

    private void OnWorldMap_OtherRoleDie(byte[] buffer)
    {
        WorldMap_OtherRoleDieProto proto = WorldMap_OtherRoleDieProto.GetProto(buffer);
        if (proto.RoleIdList != null && proto.RoleIdList.Count > 0)
        {
            for (int i = 0; i < proto.RoleIdList.Count; i++)
            {
                int dieRoleId = proto.RoleIdList[i];
                
                if (m_AllRoleDic.ContainsKey(dieRoleId))
                {
                    m_AllRoleDic[dieRoleId].CurrRoleInfo.CurrHP = 0;
                    m_AllRoleDic[dieRoleId].ToDie();

                    if (m_AllRoleDic[dieRoleId].CurrRoleType == RoleType.MainPlayer)
                    {
                        WorldMapController.Instance.EnemyNickName = m_AllRoleDic[proto.AttackRoleId].CurrRoleInfo.RoleNickName;
                        WorldMapController.Instance.OpenView(WindowUIType.WorldMapFail);
                        Debug.Log("<color=green>other die===" + m_AllRoleDic[dieRoleId].CurrRoleInfo.RoleNickName + "</color>");
                    }
                }
            }
        }
    }

    private void OnWorldMap_OtherRoleResurgence(byte[] buffer)
    {
        WorldMap_OtherRoleResurgenceProto proto = WorldMap_OtherRoleResurgenceProto.GetProto(buffer);
        if (m_AllRoleDic.ContainsKey(proto.RoleId))
        {
            m_AllRoleDic[proto.RoleId].RoleReborn();
        }
    }

    private void OnWorldMap_OtherRoleUpdateInfo(byte[] buffer)
    {

    }

    #region ���������н�ɫ�Ĺ���ʵ���������٣�
    
    private void CreateOtherPlayer(int roleId, string roleNickName, int roleLevel, int roleJobId, Vector3 rolePos, float roleYAngle, int maxHP, int currHP, int maxMP, int currMP)
    {
        RoleCtrl roleCtrl = RoleMgr.Instance.LoadOtherRole(roleId, roleNickName, roleLevel, roleJobId, maxHP, currHP, maxMP, currMP);

        roleCtrl.Born(rolePos);
        roleCtrl.transform.eulerAngles = new Vector3(0, roleYAngle, 0);

        m_AllRoleDic[roleId] = roleCtrl;
    }

    private void DestroyOtherRole(int roleId)
    {
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            Destroy(m_AllRoleDic[roleId].gameObject);
            m_AllRoleDic.Remove(roleId);
        }
    }
    #endregion

    private void RemoveEventListener()
    {
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMap_InitRole);

        //�������㲥��������ҽ����˳���
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMap_OtherRoleEnter);
        //�������㲥����������뿪�˳���
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMap_OtherRoleLeave);
        //�������㲥����������ƶ�
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMap_OtherRoleMove);
        //�������㲥���������ʹ�ü���
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMap_OtherRoleUseSkill);
        //�������㲥�����������
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMap_OtherRoleDie);
        //�������㲥������Ҹ���
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMap_OtherRoleResurgence);
        //�������㲥���������Ϣ����
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUpdateInfo, OnWorldMap_OtherRoleUpdateInfo);
    }
}
