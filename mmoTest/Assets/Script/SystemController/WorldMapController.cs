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

    #region 世界地图跨场景寻路 递归搜索路径
    private int m_BeginSceneId;//起始场景Id
    private int m_TargetSceneId;//目标场景Id

    public Queue<int> SceneIdQueue;//保存查找到的路径队列
    private Dictionary<int, WorldMapSceneEntity> m_WorldMapSceneEntityDic;//保存所有的世界地图上的场景
    private bool m_IsFindOver = false;//是否查找结束
    private WorldMapSceneEntity m_TargetScene;//目标场景
    private List<int> m_WorldMapSceneList;//存储倒序的路径

    public int CurrSceneId;//当前的场景Id
    public int ToSceneId;//要前往的场景Id
    public bool IsAutoMove = false;//是否自动移动

    public Vector3 ToScenePos = Vector3.zero;//要前往的目标场景中的位置（临时测试）

    /// <summary>
    /// 计算到达目标场景的路径
    /// </summary>
    private void Calculate(int currSceneId)
    {
        if (!m_WorldMapSceneEntityDic.ContainsKey(currSceneId)) return;

        WorldMapSceneEntity entity = m_WorldMapSceneEntityDic[currSceneId];//拿到当前场景实体
        //拆分关联场景
        string[] arr = entity.NearScene.Split('_');
        for (int i = 0; i < arr.Length; i++)
        {
            if (m_IsFindOver) continue;

            int sceneId = int.Parse(arr[i]);//拿到关联场景Id
            if (sceneId == m_BeginSceneId) continue;

            WorldMapSceneEntity findScene = m_WorldMapSceneEntityDic[sceneId];//根据关联的场景Id 拿到实体
            

            if (findScene.IsVisit) continue;//如果访问过就不再找了

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
                Calculate(findScene.Id);//从当前场景为出发点 递归查找
            }
        }
    }

    /// <summary>
    /// 计算到达目标场景的路径
    /// </summary>
    /// <param name="beginSceneId">起始场景Id</param>
    /// <param name="endSceneId">结束场景Id</param>
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

        //每次计算路径前 重置字典中的场景为没有访问过
        foreach (var pair in m_WorldMapSceneEntityDic)
        {
            pair.Value.IsVisit = false;
            pair.Value.Parent = null;
        }

        Calculate(m_BeginSceneId);//递归计算世界地图寻路

        if (m_TargetScene != null)
        {
            m_WorldMapSceneList.Clear();
            GetParentScene(m_TargetScene);
        }

        for (int i = m_WorldMapSceneList.Count-1; i >=0 ; i--)//倒序循环列表 加入到队列中
        {
            SceneIdQueue.Enqueue(m_WorldMapSceneList[i]);
        }

        //已经计算出来了路径 可以开始移动
        if (SceneIdQueue.Count >= 2)
        {
            IsAutoMove = true;
            CurrSceneId = SceneIdQueue.Dequeue();
            ToSceneId = SceneIdQueue.Dequeue();

            m_UIWorldMapView.Close();//关闭世界地图窗口

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
    /// 进行世界地图寻路的实体
    /// </summary>
    public class WorldMapSceneEntity
    {
        public int Id;//编号
        public string NearScene;//关联的场景
        public bool IsVisit;//是否访问过
        public WorldMapSceneEntity Parent;//父节点
    }
    #endregion

    /// <summary>
    /// 杀死我的玩家昵称
    /// </summary>
    public string EnemyNickName { get; set; }
}
