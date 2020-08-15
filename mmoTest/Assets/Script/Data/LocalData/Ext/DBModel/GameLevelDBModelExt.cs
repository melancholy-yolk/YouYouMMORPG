using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelDBModel
{
    private List<GameLevelEntity> retList = new List<GameLevelEntity>();

    /// <summary>
    /// �����½ڱ�Ż�ȡ�ؿ�����
    /// </summary>
    /// <returns></returns>
    public List<GameLevelEntity> GetListByChapterId(int chapterId)
    {
        if (m_List == null || m_List.Count == 0) return null;
        retList.Clear();
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].ChapterID == chapterId)
            {
                retList.Add(m_List[i]);
            }
        }
        return retList;
    }
}
