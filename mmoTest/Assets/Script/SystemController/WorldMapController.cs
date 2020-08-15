using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapController : SystemControllerBase<WorldMapController>, ISystemController 
{
    private UIWorldMapView m_UIWorldMapView;
    private UIWorldMapFailView m_UIWorldMapFailView;

    public override void Dispose()
    {
        base.Dispose();
    }

    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.WorldMap:
                OpenUIWorldMapView();
                break;
            case WindowUIType.WorldMapFail:
                OpenWorldMapFailView();
                break;
        }
    }

    private void OpenUIWorldMapView()
    {
        List<WorldMapEntity> worldMapEntityList = WorldMapDBModel.Instance.GetList();
        if (worldMapEntityList == null || worldMapEntityList.Count == 0)
        {
            return;
        }

        m_UIWorldMapView = UIViewUtil.Instance.OpenWindow(WindowUIType.WorldMap).GetComponent<UIWorldMapView>();

        TransferData data = new TransferData();

        List<TransferData> list = new List<TransferData>();
        for (int i = 0; i < worldMapEntityList.Count; i++)
        {
            WorldMapEntity entity = worldMapEntityList[i];
            if(entity.IsShowInMap == 0) continue;

            TransferData tempData = new TransferData();
            tempData.SetValue(ConstDefine.WorldMap_Id, entity.Id);
            tempData.SetValue(ConstDefine.WorldMap_Name, entity.Name);
            tempData.SetValue(ConstDefine.WorldMap_Icon, entity.IcoInMap);

            string[] arr = entity.PosInMap.Split('_');
            Vector2 posInMap = new Vector2();
            if (arr.Length == 2)
            {
                posInMap.x = float.Parse(arr[0]);
                posInMap.y = float.Parse(arr[1]);
            }
            Debug.Log(posInMap);
            tempData.SetValue(ConstDefine.WorldMap_PosInMap, posInMap);

            list.Add(tempData);
        }

        data.SetValue(ConstDefine.WorldMap_ItemList, list);

        m_UIWorldMapView.SetUI(data, OnWorldMapItemClick);
    }

    public void OnWorldMapItemClick(int worldMapId)
    {
        //SceneMgr.Instance.LoadToWorldMap(worldMapId);
        if (SceneMgr.Instance.CurrWorldMapId == worldMapId) return;
        CalculateTargetScenePath(SceneMgr.Instance.CurrWorldMapId, worldMapId);
    }

    private void OpenWorldMapFailView()
    {
        m_UIWorldMapFailView = UIViewUtil.Instance.OpenWindow(WindowUIType.WorldMapFail).GetComponent<UIWorldMapFailView>();
        m_UIWorldMapFailView.OnReborn = () => 
        {
            WorldMap_CurrRoleResurgenceProto proto = new WorldMap_CurrRoleResurgenceProto();
            proto.Type = 0;
            NetWorkSocket.Instance.SendMsg(proto.ToArray());
            m_UIWorldMapFailView.Close();
        };
        m_UIWorldMapFailView.OnReturnMainCity = () => 
        {
            PlayerController.Instance.LastInWorldMapPos = string.Empty;
            GlobalInit.Instance.MainPlayer.RoleReborn();
            SceneMgr.Instance.LoadToWorldMap(2);
        };
        m_UIWorldMapFailView.SetUI(EnemyNickName);
    }

    #region �����ͼ�糡��Ѱ· �ݹ�����·��
    private int m_BeginSceneId;//��ʼ����Id
    private int m_TargetSceneId;//Ŀ�곡��Id

    public Queue<int> SceneIdQueue;//������ҵ���·������
    private Dictionary<int, WorldMapSceneEntity> m_WorldMapSceneEntityDic;//�������е������ͼ�ϵĳ���
    private bool m_IsFindOver = false;//�Ƿ���ҽ���
    private WorldMapSceneEntity m_TargetScene;//Ŀ�곡��
    private List<int> m_WorldMapSceneList;//�洢�����·��

    public int CurrSceneId;//��ǰ�ĳ���Id
    public int ToSceneId;//Ҫǰ���ĳ���Id
    public bool IsAutoMove = false;//�Ƿ��Զ��ƶ�

    public Vector3 ToScenePos = Vector3.zero;//Ҫǰ����Ŀ�곡���е�λ�ã���ʱ���ԣ�

    /// <summary>
    /// ���㵽��Ŀ�곡����·��
    /// </summary>
    private void Calculate(int currSceneId)
    {
        if (!m_WorldMapSceneEntityDic.ContainsKey(currSceneId)) return;

        WorldMapSceneEntity entity = m_WorldMapSceneEntityDic[currSceneId];//�õ���ǰ����ʵ��
        //��ֹ�������
        string[] arr = entity.NearScene.Split('_');
        for (int i = 0; i < arr.Length; i++)
        {
            if (m_IsFindOver) continue;

            int sceneId = int.Parse(arr[i]);//�õ���������Id
            if (sceneId == m_BeginSceneId) continue;

            WorldMapSceneEntity findScene = m_WorldMapSceneEntityDic[sceneId];//���ݹ����ĳ���Id �õ�ʵ��
            

            if (findScene.IsVisit) continue;//������ʹ��Ͳ�������

            findScene.IsVisit = true;
            findScene.Parent = entity;

            if (findScene.Id == m_TargetSceneId)
            {
                m_IsFindOver = true;
                m_TargetScene = findScene;
                break;
            }
            else
            {
                Calculate(findScene.Id);//�ӵ�ǰ����Ϊ������ �ݹ����
            }
        }
    }

    /// <summary>
    /// ���㵽��Ŀ�곡����·��
    /// </summary>
    /// <param name="beginSceneId">��ʼ����Id</param>
    /// <param name="endSceneId">��������Id</param>
    public void CalculateTargetScenePath(int beginSceneId, int endSceneId)
    {
        List<WorldMapEntity> worldMapEntityList = WorldMapDBModel.Instance.GetList();
        if (m_WorldMapSceneEntityDic == null)
        {
            m_WorldMapSceneEntityDic = new Dictionary<int, WorldMapSceneEntity>();
            for (int i = 0; i < worldMapEntityList.Count; i++)
            {
                WorldMapSceneEntity sceneEntity = new WorldMapSceneEntity() { Id = worldMapEntityList[i].Id, NearScene = worldMapEntityList[i].NearScene, IsVisit = false };
                m_WorldMapSceneEntityDic[sceneEntity.Id] = sceneEntity;
            }
            SceneIdQueue = new Queue<int>();
            m_WorldMapSceneList = new List<int>();
        }

        m_BeginSceneId = beginSceneId;
        m_TargetSceneId = endSceneId;
        SceneIdQueue.Clear();
        m_IsFindOver = false;

        //ÿ�μ���·��ǰ �����ֵ��еĳ���Ϊû�з��ʹ�
        foreach (var pair in m_WorldMapSceneEntityDic)
        {
            pair.Value.IsVisit = false;
            pair.Value.Parent = null;
        }

        Calculate(m_BeginSceneId);//�ݹ���������ͼѰ·

        if (m_TargetScene != null)
        {
            m_WorldMapSceneList.Clear();
            GetParentScene(m_TargetScene);
        }

        for (int i = m_WorldMapSceneList.Count-1; i >=0 ; i--)//����ѭ���б� ���뵽������
        {
            SceneIdQueue.Enqueue(m_WorldMapSceneList[i]);
        }

        //�Ѿ����������·�� ���Կ�ʼ�ƶ�
        if (SceneIdQueue.Count >= 2)
        {
            IsAutoMove = true;
            CurrSceneId = SceneIdQueue.Dequeue();
            ToSceneId = SceneIdQueue.Dequeue();

            m_UIWorldMapView.Close();//�ر������ͼ����

            if (WorldMapSceneCtrl.Instance != null)
            {
                WorldMapSceneCtrl.Instance.AutoMove();
            }
        }
    }

    private void GetParentScene(WorldMapSceneEntity entity)
    {
        m_WorldMapSceneList.Add(entity.Id);
        if (entity.Parent != null)
        {
            GetParentScene(entity.Parent);
        }
    }

    /// <summary>
    /// ���������ͼѰ·��ʵ��
    /// </summary>
    public class WorldMapSceneEntity
    {
        public int Id;//���
        public string NearScene;//�����ĳ���
        public bool IsVisit;//�Ƿ���ʹ�
        public WorldMapSceneEntity Parent;//���ڵ�
    }
    #endregion

    /// <summary>
    /// ɱ���ҵ�����ǳ�
    /// </summary>
    public string EnemyNickName { get; set; }
}
