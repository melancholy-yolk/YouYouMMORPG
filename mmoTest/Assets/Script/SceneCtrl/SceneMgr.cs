using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 负责场景的切换
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    #region 变量
    /// <summary>
    /// 当前场景类型
    /// </summary>
    public SceneType CurrentSceneType 
    { 
        get; 
        private set; 
    }

    /// <summary>
    /// 当前世界地图Id
    /// </summary>
    private int m_CurrWorldMapId;
    public int CurrWorldMapId 
    { 
        get 
        { 
            return m_CurrWorldMapId; 
        } 
    }
    public int TargetWorldMapTransPointId = 0;//目标世界地图的传送点Id
    private int m_WillToWorldMapId;//角色想去的目标世界地图场景编号

    /// <summary>
    /// 当前关卡Id
    /// </summary>
    private int m_CurrGameLevelId;
    public int CurrGameLevelId
    {
        get
        { return m_CurrGameLevelId; }
    }

    /// <summary>
    /// 当前关卡难度
    /// </summary>
    private GameLevelGrade m_CurrGameLevelGrade;
    public GameLevelGrade CurrGameLevelGrade
    {
        get
        { return m_CurrGameLevelGrade; }
    }

    /// <summary>
    /// 当前玩法类型
    /// </summary>
    public PlayType CurrPlayType
    {
        get;
        private set;
    }

    public bool IsFightingScene//当前场景是否可以战斗(主城中不可以战斗)
    {
        get;
        private set;
    }
    #endregion

    #region 构造函数
    /// <summary>
    /// 构造函数监听服务器返回消息
    /// </summary>
    public SceneMgr()
    {
        //服务器返回角色进入世界地图场景消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleEnterReturn, OnWorldMap_RoleEnterReturn);
    }
    #endregion

    /// <summary>
    /// 跳转到登录场景
    /// </summary>
    public void LoadToLogOn()
    {
        CurrentSceneType = SceneType.LogOn;
        
        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }

    /// <summary>
    /// 跳转到选择角色场景
    /// </summary>
    public void LoadToSelectRole()
    {
        CurrentSceneType = SceneType.SelectRole;
        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }

    #region 跳转到世界地图场景
    /// <summary>
    /// 跳转到世界地图场景
    /// </summary>
    /// <param name="worldMapId"></param>
    public void LoadToWorldMap(int worldMapId)
    {
        if (m_CurrWorldMapId == worldMapId && CurrentSceneType == SceneType.WorldMap)
        {
            MessageController.Instance.Show("NOTICE", "You has been in target scene!");
            return;
        }

        WorldMapRoleEnter(worldMapId);//向服务器发送进入世界地图场景请求
    }

    /// <summary>
    /// 向服务器发送角色进入世界地图场景消息
    /// </summary>
    /// <param name="worldMapId"></param>
    private void WorldMapRoleEnter(int worldMapId)
    {
        m_WillToWorldMapId = worldMapId;

        WorldMap_RoleEnterProto proto = new WorldMap_RoleEnterProto();
        proto.WorldMapSceneId = m_WillToWorldMapId;

        NetWorkSocket.Instance.SendMsg(proto.ToArray());
    }

    /// <summary>
    /// 服务器返回角色进入世界地图场景消息
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMap_RoleEnterReturn(byte[] buffer)
    {
        WorldMap_RoleEnterReturnProto retProto = WorldMap_RoleEnterReturnProto.GetProto(buffer);

        if (retProto.IsSuccess)
        {
            if (WorldMapSceneCtrl.Instance != null)
            {
                WorldMapSceneCtrl.Instance.MainPlayerLeaveScene();
            }

            m_CurrWorldMapId = m_WillToWorldMapId;//服务器同意我跳转场景 将当前世界地图Id设置为我想要去的场景Id

            CurrentSceneType = SceneType.WorldMap;//设置当前场景类型
            CurrPlayType = PlayType.PVP;//设置当前战斗模式

            WorldMapEntity entity = WorldMapDBModel.Instance.GetEntity(m_CurrWorldMapId);
            if (entity != null)
            {
                //不是主城 就可以PVP战斗
                IsFightingScene = (entity.IsCity == 0);
            }

            SceneManager.LoadScene(ConstDefine.Scene_Loading);
        }
    }
    #endregion

    #region 跳转到PVE游戏关卡
    /// <summary>
    /// 跳转到PVE游戏关卡场景
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public void LoadToGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        m_CurrGameLevelId = gameLevelId;//设置当前关卡Id
        m_CurrGameLevelGrade = grade;//设置当前关卡难度

        CurrentSceneType = SceneType.GameLevel;//设置当前场景类型
        CurrPlayType = PlayType.PVE;//设置当前战斗模式

        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }
    #endregion
}
