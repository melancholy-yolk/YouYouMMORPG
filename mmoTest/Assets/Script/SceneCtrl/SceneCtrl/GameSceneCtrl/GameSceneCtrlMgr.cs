using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCtrlMgr : MonoBehaviour 
{
    [SerializeField]
    private GameLevelSceneCtrl m_GameLevelSceneCtrl;//��Ϸ�ؿ�������������
    [SerializeField]
    private WorldMapSceneCtrl m_WorldMapSceneCtrl;//�����ͼ������������

    private Dictionary<SceneType, GameObject> m_Dic = new Dictionary<SceneType, GameObject>();

	void Awake () 
    {
        if (m_GameLevelSceneCtrl != null)
        {
            m_Dic[SceneType.GameLevel] = m_GameLevelSceneCtrl.gameObject;
        }

        if (m_WorldMapSceneCtrl != null)
        {
            m_Dic[SceneType.WorldMap] = m_WorldMapSceneCtrl.gameObject;
        }

        GameObject obj = m_Dic[SceneMgr.Instance.CurrentSceneType];
        if (obj != null)
        {
            obj.SetActive(true);
        }

        foreach (var item in m_Dic)
        {
            if (item.Key != SceneMgr.Instance.CurrentSceneType)
            {
                Destroy(item.Value);
            }
        }
	}

}
