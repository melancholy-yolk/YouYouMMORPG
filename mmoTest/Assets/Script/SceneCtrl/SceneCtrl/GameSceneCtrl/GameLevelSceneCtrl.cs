using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSceneCtrl : GameSceneCtrlBase
{
    public static GameLevelSceneCtrl Instance;

    #region ����
    [SerializeField]
    private GameLevelRegionCtrl[] AllRegion;//�˹ؿ����е����������

    [SerializeField]
    private AstarPath m_AstarPath;//�˹ؿ���A��Ѱ·���

    

    private int m_CurrGameLevelId;// ��ǰ�ؿ�Id
    private GameLevelGrade m_CurrGrade;// ��ǰ�ؿ����Ѷȵȼ�

    private float m_FightTime = 0f;//ս��ʱ��
    private bool m_IsFighting;//�Ƿ���ս����


    #region �ؿ�����
    private List<GameLevelRegionEntity> m_RegionList;// ��������õ������б�(�ؿ�����˳�� ����˳������ ��ʾ���ǽ��������˳��)
    private int m_AllMonsterCount;// ���ؿ���������
    private int[] m_MonsterIdArr;// ����ùؿ����еĹ���Id
    private SpawnPool m_MonsterPool;//�����
    #endregion

    #region ��ǰ������������
    private int m_CurrRegionIndex;// ��ǰ���������������
    private int m_CurrRegionId;
    private GameLevelRegionCtrl m_CurrRegionCtrl;// ��ǰ�����������

    private int m_CurrRegionMonsterTotalCount;// ��ǰ�����������
    private int m_CurrRegionCreatedMonsterCount;// ��ǰ�����Ѿ������Ĺ�������
    private int m_CurrRegionKilledMonsterCount;//��ǰ�����Ѿ�ɱ���Ĺ�������

    
    private Dictionary<int, int> m_RegionMonsterDic;//����ֵ䱣�����Ϣ�ǣ����ֹ� �ж���

    private float m_NextCreateMonsterTime = 0;//�������ˢ��һֻ��
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

        //����Ϸ�ؿ��������������ı�����ֵ
        m_CurrGameLevelId = SceneMgr.Instance.CurrGameLevelId;//��ǰ�ؿ����
        m_CurrGrade = SceneMgr.Instance.CurrGameLevelGrade;//��ǰ�ؿ��Ѷ�

        //����Ϸ�ؿ�UI�������ı�����ֵ
        GameLevelController.Instance.CurrGameLevelId = m_CurrGameLevelId;
        GameLevelController.Instance.CurrGameLevelGrade = m_CurrGrade;

        m_IsFighting = true;//��ʼ�ؿ�ս�� �˱�־λΪfalseʱ ��ʾ�ؿ�������ͨ�سɹ�/ʧ�ܣ�

        m_RegionMonsterDic = new Dictionary<int, int>();//��ǰ����������� <����Id,��������>
        spriteList = new List<int>();//��ǰ�������Id�б�

        GameLevelController.Instance.CurrGameLevelTotalExp = 0;//��ǰ�ؿ���õ��ܾ���
        GameLevelController.Instance.CurrGameLevelTotalGold = 0;//��ǰ�ؿ���õ��ܽ��
        GameLevelController.Instance.CurrGameLevelKillMonsterDic.Clear();
        GameLevelController.Instance.CurrGameLevelGetGoodsList.Clear();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        //��ǰ�ؿ���δͨ��
        if (m_IsFighting)
        {
            m_FightTime += Time.deltaTime;

            //���һ��ʱ��ˢ��
            if (m_CurrRegionCreatedMonsterCount < m_CurrRegionMonsterTotalCount)
            {
                if (Time.time > m_NextCreateMonsterTime)
                {
                    m_NextCreateMonsterTime = Time.time + 1f;
                    CreateMonster();
                }
            }
        }

    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
    #endregion

    #region ��Ϸ�ؿ����� ����UI������ɻص�
    /// <summary>
    /// ����UI������ɻص�
    /// </summary>
    protected override void OnUISceneMainCityViewLoadComplete()
    {
        base.OnUISceneMainCityViewLoadComplete();

        //���ó�����פUI
        PlayerController.Instance.SetMainCityRoleData();//controller--->view ������ʹ�����ݸ���ͼ��ֵ


        //������Ϸ�ؿ���ŷ���excel�����õ������б� ��Ϸ�ؿ���������Ⱥ�����˳��
        m_RegionList = GameLevelRegionDBModel.Instance.GetListByGameLevelId(m_CurrGameLevelId);

        //������������
        m_AllMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterTotalCount(m_CurrGameLevelId, m_CurrGrade);

        //�����������༯��
        m_MonsterIdArr = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterId(m_CurrGameLevelId, m_CurrGrade);

        #region �����
        //����һ�������
        m_MonsterPool = PoolManager.Pools.Create("Monster");
        m_MonsterPool.group.parent = null;
        m_MonsterPool.group.localPosition = Vector3.zero;

        //��ab���� ����ÿ�ֹ����Ԥ����
        for (int i = 0; i < m_MonsterIdArr.Length; i++)
        {
            PrefabPool prefabPool = new PrefabPool(RoleMgr.Instance.LoadSprite(m_MonsterIdArr[i]).transform);
            prefabPool.preloadAmount = 5;//Ԥ��������
            prefabPool.cullDespawned = true;//�Ƿ���������Զ�����
            prefabPool.cullAbove = 5;//�����ʼ�ձ��ֶ���
            prefabPool.cullDelay = 2;//�೤ʱ������һ��
            prefabPool.cullMaxPerPass = 2;//ÿ��������

            m_MonsterPool.CreatePrefabPool(prefabPool);
        }
        #endregion

        //Ĭ�Ͻ������Ϊ0������
        m_CurrRegionIndex = 0;
        EnterRegion(m_CurrRegionIndex);
    }
    #endregion

    #region ���� ��ǰ�����Ƿ��д��Ĺ�
    /// <summary>
    /// ��ǰ�����Ƿ��й�
    /// </summary>
    public bool CurrentRegionHasMonster
    {
        get
        {
            return m_CurrRegionKilledMonsterCount < m_CurrRegionMonsterTotalCount;
        }
    }
    #endregion

    #region ���� ��ǰ�����Ƿ��ǵ�ǰ�ؿ����һ����
    /// <summary>
    /// ��ǰ�����Ƿ������һ������
    /// </summary>
    public bool CurrentRegionIsLast
    {
        get
        {
            return m_CurrRegionIndex == m_RegionList.Count;
        }
    }
    #endregion

    #region ���� ��һ���������ǳ�����
    /// <summary>
    /// ��һ���������ǳ�����
    /// </summary>
    public Vector3 NextRegionPlayerBornPos
    {
        get
        {
            GameLevelRegionEntity entity = GetRegionEntityByIndex(m_CurrRegionIndex);//ͨ��excel������ʵ����������� �õ���Ӧ������ʵ��
            if (entity == null) return Vector3.zero;

            int regionId = entity.RegionId;//Ҫ������������������
            return GetRegionCtrlByRegionId(regionId).RoleBornPos.position;
        }
    }
    #endregion

    /// <summary>
    /// ��GameLevelRegion���еõ������Ⱥ�˳�����е������б� ͨ��������Ⱥ��������� ��ö�ӦregionEntity
    /// </summary>
    private GameLevelRegionEntity GetRegionEntityByIndex(int index)
    {
        GameLevelRegionEntity entity = null;
        for (int i = 0; i < m_RegionList.Count; i++)
        {
            if (i == index)
            {
                entity = m_RegionList[i];
            }
        }
        return entity;
    }

    /// <summary>
    /// ͨ������Id ������������
    /// </summary>
    private GameLevelRegionCtrl GetRegionCtrlByRegionId(int regionId)
    {
        GameLevelRegionCtrl ctrl = null;
        for (int i = 0; i < AllRegion.Length; i++)
        {
            if (AllRegion[i].RegionId == regionId)
            {
                ctrl = AllRegion[i];
            }
        }
        return ctrl;
    }

    #region ����ؿ���ĳ������ �������Ϊ�����Ⱥ���������
    /// <summary>
    /// �������� Ҫ�����£������������ ���������ڵ� �������� ��excel��ȡ��������
    /// </summary>
    private void EnterRegion(int regionIndex)
    {
        GameLevelRegionEntity entity = GetRegionEntityByIndex(regionIndex);//ͨ��excel������ʵ����������� �õ���Ӧ������ʵ��
        if (entity == null) return;

        int regionId = entity.RegionId;//Ҫ������������������
        m_CurrRegionCtrl = GetRegionCtrlByRegionId(regionId);
        if (m_CurrRegionCtrl == null) return;

        m_CurrRegionCreatedMonsterCount = 0;//��ǰ���������Ѿ�ˢ���Ĺ�������
        m_CurrRegionKilledMonsterCount = 0;//��ǰ���������Ѿ�ɱ���Ĺ�������

        //���ٱ�������ڵ�����
        if (m_CurrRegionCtrl.RegionMask != null)
        {
            Destroy(m_CurrRegionCtrl.RegionMask);
        }

        //����һ���������
        if (regionIndex != 0)
        {
            GameLevelDoorCtrl doorCtrl = m_CurrRegionCtrl.GetToNextRegionDoor(m_CurrRegionId);//�ϴε�������
            if (doorCtrl != null)
            {
                doorCtrl.gameObject.SetActive(false);
                if (doorCtrl.ConnectToDoor != null)
                {
                    doorCtrl.ConnectToDoor.gameObject.SetActive(false);
                }
            }
        }
        m_CurrRegionId = regionId;

        #region ����һ���ؿ��ĵ�һ������
        //����� ��ǰ�ؿ��ĵ�һ������
        if (regionIndex == 0)
        {
            if (GlobalInit.Instance.MainPlayer != null)
            {
                GlobalInit.Instance.MainPlayer.Born(m_CurrRegionCtrl.RoleBornPos.position);//������λ�����õ�����������ǳ�����λ��
                GlobalInit.Instance.MainPlayer.AstarPathController = m_AstarPath;//������Ѱ·����������
                GlobalInit.Instance.MainPlayer.ToIdle(RoleIdleState.IdleFight);//����Ĭ�Ͻ���ս������
                GlobalInit.Instance.MainPlayer.OnDieHandler = (RoleCtrl roleCtrl) => { StartCoroutine(ShowFailView()); };//�����������¼��󶨻ص�
            }
            if (DelegateDefine.Instance.OnSceneLoadOK != null) DelegateDefine.Instance.OnSceneLoadOK();//֪ͨ�³����������
        }
        #endregion

        //ˢ��
        m_CurrRegionMonsterTotalCount = GameLevelMonsterDBModel.Instance.GetRegionMonsterTotalCount(m_CurrGameLevelId, m_CurrGrade, entity.RegionId);//��ȡ��ǰ�����������
        List<GameLevelMonsterEntity> m_RegionMonsterList = GameLevelMonsterDBModel.Instance.GetGameLevelMonster(m_CurrGameLevelId, m_CurrGrade, entity.RegionId);//��ȡ��ǰ��������б�(һ�ֹ�һ��ʵ�� �������������ı�� ����)
        for (int i = 0; i < m_RegionMonsterList.Count; i++)
        {
            if (m_RegionMonsterDic.ContainsKey(m_RegionMonsterList[i].SpriteId))
            {
                m_RegionMonsterDic[m_RegionMonsterList[i].SpriteId] += m_RegionMonsterList[i].SpriteCount; 
            }
            else
            {
                m_RegionMonsterDic[m_RegionMonsterList[i].SpriteId] = m_RegionMonsterList[i].SpriteCount;
            }
        }
        
    }

    /// <summary>
    /// �������� ��ʾͨ��ʧ�ܴ���
    /// </summary>
    private IEnumerator ShowFailView()
    {
        yield return new WaitForSeconds(3f);
        GameLevelController.Instance.OpenView(WindowUIType.GameLevelFail);
    }
    #endregion

    #region ˢ��һ����
    private int m_Index = 0;//�������
    private int m_MonsterTempId = 0;//��ʱ����Id
    List<int> spriteList;//�������Id���б�

    /// <summary>
    /// ��������
    /// </summary>
    private void CreateMonster()
    {
        //if (m_CurrRegionCreatedMonsterCount >= 1) return;

        #region ����õ�һ������Id
        spriteList.Clear();
        foreach (var item in m_RegionMonsterDic)
        {
            spriteList.Add(item.Key);
        }
        m_Index = Random.Range(0, spriteList.Count);
        int monsterId = spriteList[m_Index];
        #endregion

        //�ӻ������ȡ������
        Transform trans = m_MonsterPool.Spawn(SpriteDBModel.Instance.GetEntity(monsterId).PrefabName);

        //���ѡ�������Ĺ���������е�һ�� ��Ϊ����ĳ�����
        Transform monsterBornPos = m_CurrRegionCtrl.MonsterBornPos[Random.Range(0, m_CurrRegionCtrl.MonsterBornPos.Length)];

        //��ʼ�������ɫ������
        RoleCtrl roleMonsterCtrl = trans.GetComponent<RoleCtrl>();

        SpriteEntity spriteEntity = SpriteDBModel.Instance.GetEntity(monsterId);//�ӱ��ع���excel���л�ȡ���˹����Ŷ�Ӧ�Ĺ���ʵ��

        RoleInfoMonster monsterInfo = new RoleInfoMonster(spriteEntity);//����Ľ�ɫ��Ϣ
        monsterInfo.RoleId = ++m_MonsterTempId;

        roleMonsterCtrl.ViewRange = spriteEntity.Range_View;//������Ұ��Χ
        roleMonsterCtrl.PatrolRange = 3;//����Ѳ�߷�Χ
        roleMonsterCtrl.Speed = spriteEntity.MoveSpeed;//�����ƶ��ٶ�

        roleMonsterCtrl.Init(RoleType.Monster, monsterInfo, new GameLevel_RoleMonsterAI(roleMonsterCtrl, monsterInfo));//��ʼ����ɫ������
        roleMonsterCtrl.OnDieHandler = OnDieHandler;//���ý�ɫ�����ص�
        roleMonsterCtrl.OnDestroyHandler = OnDestroyHandler;//���ý�ɫ���ٻص�


        roleMonsterCtrl.ToIdle(RoleIdleState.IdleFight);//����Ĭ��Ϊս������״̬
        roleMonsterCtrl.Born(monsterBornPos.TransformPoint(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)));//���ù������λ��

        m_RegionMonsterDic[monsterId]--;//Ҫ���ɵĴ��������������һ
        if (m_RegionMonsterDic[monsterId] <= 0)
        {
            m_RegionMonsterDic.Remove(monsterId);
        }

        m_CurrRegionCreatedMonsterCount++;//�Ѿ�ˢ���Ĺ���������һ
    }
    #endregion

    #region ÿɱ��һ���� ���д˻ص� �жϹؿ������Ƿ���ɹؿ�
    /// <summary>
    /// ��ɫ����ʱ�ص� 
    /// </summary>
    /// <param name="roleId"></param>
    private void OnDieHandler(RoleCtrl roleCtrl)
    {
        m_CurrRegionKilledMonsterCount++;//��ǰ������������������һ

        #region ������������
        RoleInfoMonster monsterInfo = (RoleInfoMonster)roleCtrl.CurrRoleInfo;
        //������侭��ͽ��
        GameLevelMonsterEntity entity = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterEntity(m_CurrGameLevelId, m_CurrGrade, m_CurrRegionId, monsterInfo.spriteEntity.Id);
        if (entity != null)
        {
            if (entity.Exp > 0)
            {
                UITipView.Instance.ShowTip(0, string.Format("+{0}", entity.Exp));
                GameLevelController.Instance.CurrGameLevelTotalExp += entity.Exp;
            }
            if (entity.Gold > 0)
            {
                UITipView.Instance.ShowTip(1, string.Format("+{0}", entity.Gold));
                GameLevelController.Instance.CurrGameLevelTotalGold += entity.Gold;
            }
        }
        #endregion

        //��ӵ�ǰ�ؿ�ɱ������
        if (GameLevelController.Instance.CurrGameLevelKillMonsterDic.ContainsKey(entity.SpriteId))
        {
            GameLevelController.Instance.CurrGameLevelKillMonsterDic[entity.SpriteId] += 1;
        }
        else
        {
            GameLevelController.Instance.CurrGameLevelKillMonsterDic[entity.SpriteId] = 1;
        }

        //��ǰ�������ȫ����ɱ�� ���Խ�����һ��������
        if (m_CurrRegionKilledMonsterCount >= m_CurrRegionCreatedMonsterCount)
        {
            m_CurrRegionIndex++;
            //�ж��Ƿ������һ������
            if (CurrentRegionIsLast)
            {
                m_IsFighting = false;//ͨ��
                GameLevelController.Instance.CurrGameLevelPassTime = m_FightTime;//����Ϸ�ؿ���UI��������ֵ

                TimeMgr.Instance.ChangeTimeScale(0.5f, 3f);
                //����ʤ������
                Debug.Log("Game Win!");
                StartCoroutine(ShowVictory());
                return;
            }

            m_CurrRegionMonsterTotalCount = 0;// ��ǰ�����������
            m_CurrRegionCreatedMonsterCount = 0;// ��ǰ�����Ѿ������Ĺ�������
            m_CurrRegionKilledMonsterCount = 0;//��ǰ�����Ѿ�ɱ���Ĺ�������

            //������һ������
            EnterRegion(m_CurrRegionIndex);
        }
    }
    #endregion

    #region ÿ���������� ��һ��ʱ����յ������
    /// <summary>
    /// ��ɫ����
    /// </summary>
    private void OnDestroyHandler(Transform obj)
    {
        //��ɫ����
        m_MonsterPool.Despawn(obj);
    }
    #endregion

    private IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(3f);
        GameLevelController.Instance.OpenView(WindowUIType.GameLevelVictory);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);

        if (AllRegion != null && AllRegion.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < AllRegion.Length; i++)
            {
                Gizmos.DrawLine(this.transform.position, AllRegion[i].transform.position);
            }
        }
        
       
    }
#endif

}
