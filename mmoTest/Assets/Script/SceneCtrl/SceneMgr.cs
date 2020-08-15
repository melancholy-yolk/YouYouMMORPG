using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���𳡾����л�
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    #region ����
    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public SceneType CurrentSceneType 
    { 
        get; 
        private set; 
    }

    /// <summary>
    /// ��ǰ�����ͼId
    /// </summary>
    private int m_CurrWorldMapId;
    public int CurrWorldMapId 
    { 
        get 
        { 
            return m_CurrWorldMapId; 
        } 
    }
    public int TargetWorldMapTransPointId = 0;//Ŀ�������ͼ�Ĵ��͵�Id
    private int m_WillToWorldMapId;//��ɫ��ȥ��Ŀ�������ͼ�������

    /// <summary>
    /// ��ǰ�ؿ�Id
    /// </summary>
    private int m_CurrGameLevelId;
    public int CurrGameLevelId
    {
        get
        { return m_CurrGameLevelId; }
    }

    /// <summary>
    /// ��ǰ�ؿ��Ѷ�
    /// </summary>
    private GameLevelGrade m_CurrGameLevelGrade;
    public GameLevelGrade CurrGameLevelGrade
    {
        get
        { return m_CurrGameLevelGrade; }
    }

    /// <summary>
    /// ��ǰ�淨����
    /// </summary>
    public PlayType CurrPlayType
    {
        get;
        private set;
    }

    public bool IsFightingScene//��ǰ�����Ƿ����ս��(�����в�����ս��)
    {
        get;
        private set;
    }
    #endregion

    #region ���캯��
    /// <summary>
    /// ���캯������������������Ϣ
    /// </summary>
    public SceneMgr()
    {
        //���������ؽ�ɫ���������ͼ������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleEnterReturn, OnWorldMap_RoleEnterReturn);
    }
    #endregion

    /// <summary>
    /// ��ת����¼����
    /// </summary>
    public void LoadToLogOn()
    {
        CurrentSceneType = SceneType.LogOn;
        
        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }

    /// <summary>
    /// ��ת��ѡ���ɫ����
    /// </summary>
    public void LoadToSelectRole()
    {
        CurrentSceneType = SceneType.SelectRole;
        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }

    #region ��ת�������ͼ����
    /// <summary>
    /// ��ת�������ͼ����
    /// </summary>
    /// <param name="worldMapId"></param>
    public void LoadToWorldMap(int worldMapId)
    {
        if (m_CurrWorldMapId == worldMapId && CurrentSceneType == SceneType.WorldMap)
        {
            MessageController.Instance.Show("NOTICE", "You has been in target scene!");
            return;
        }

        WorldMapRoleEnter(worldMapId);//����������ͽ��������ͼ��������
    }

    /// <summary>
    /// ����������ͽ�ɫ���������ͼ������Ϣ
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
    /// ���������ؽ�ɫ���������ͼ������Ϣ
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

            m_CurrWorldMapId = m_WillToWorldMapId;//������ͬ������ת���� ����ǰ�����ͼId����Ϊ����Ҫȥ�ĳ���Id

            CurrentSceneType = SceneType.WorldMap;//���õ�ǰ��������
            CurrPlayType = PlayType.PVP;//���õ�ǰս��ģʽ

            WorldMapEntity entity = WorldMapDBModel.Instance.GetEntity(m_CurrWorldMapId);
            if (entity != null)
            {
                //�������� �Ϳ���PVPս��
                IsFightingScene = (entity.IsCity == 0);
            }

            SceneManager.LoadScene(ConstDefine.Scene_Loading);
        }
    }
    #endregion

    #region ��ת��PVE��Ϸ�ؿ�
    /// <summary>
    /// ��ת��PVE��Ϸ�ؿ�����
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public void LoadToGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        m_CurrGameLevelId = gameLevelId;//���õ�ǰ�ؿ�Id
        m_CurrGameLevelGrade = grade;//���õ�ǰ�ؿ��Ѷ�

        CurrentSceneType = SceneType.GameLevel;//���õ�ǰ��������
        CurrPlayType = PlayType.PVE;//���õ�ǰս��ģʽ

        SceneManager.LoadScene(ConstDefine.Scene_Loading);
    }
    #endregion
}
