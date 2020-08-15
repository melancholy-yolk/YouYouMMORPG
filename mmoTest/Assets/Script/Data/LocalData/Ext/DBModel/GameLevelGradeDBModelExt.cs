using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelGradeDBModel 
{
    /// <summary>
    /// ���ݹؿ���ź��Ѷȵȼ���ȡʵ��
    /// </summary>
    /// <param name="id"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public GameLevelGradeEntity GetEntityByIdAndGrade(int id, GameLevelGrade grade)
    {
        GameLevelGradeEntity entity = null;
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == id && m_List[i].Grade == (int)grade)
            {
                entity = m_List[i];
                break;
            }
        }
        return entity;
    }
}
