using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelMonsterDBModel 
{
    /// <summary>
    /// ������Ϸ�ؿ���ź��Ѷȵȼ� ��ȡ��ǰ�ؿ��й����������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <returns></returns>
    public int GetGameLevelMonsterTotalCount(int gameLevelId, GameLevelGrade grade)
    {
        int count = 0;
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade)
            {
                count += m_List[i].SpriteCount;
            }
        }
        return count;
    }
    
    /// <summary>
    /// ������Ϸ�ؿ���� �Ѷȵȼ� ��ȡ��������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public int[] GetGameLevelMonsterId(int gameLevelId, GameLevelGrade grade)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade)
            {
                if (!list.Contains(m_List[i].SpriteId))
                {
                    list.Add(m_List[i].SpriteId);
                }
            }
        }

        return list.ToArray();
    }

    private List<GameLevelMonsterEntity> monsterEntityList = new List<GameLevelMonsterEntity>();

    /// <summary>
    /// ���ݹؿ���� �ؿ��Ѷ� ������ ��ȡ��ǰ�����е����й���
    /// </summary>
    public List<GameLevelMonsterEntity> GetGameLevelMonster(int gameLevelId, GameLevelGrade grade, int regionId)
    {
        monsterEntityList.Clear();
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade && m_List[i].RegionId == regionId)
            {
                monsterEntityList.Add(m_List[i]);
            }
        }
        return monsterEntityList;
    }

    /// <summary>
    /// �õ���ǰ�����ڹ�������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <param name="regionId"></param>
    /// <returns></returns>
    public int GetRegionMonsterTotalCount(int gameLevelId, GameLevelGrade grade, int regionId)
    {
        int count = 0;
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade && m_List[i].RegionId == regionId)
            {
                count += m_List[i].SpriteCount;
            }
        }
        return count;
    }

    /// <summary>
    /// ��ȡ��Ϸ�ؿ������ʵ��
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <param name="regionId"></param>
    /// <param name="spriteId"></param>
    /// <returns></returns>
    public GameLevelMonsterEntity GetGameLevelMonsterEntity(int gameLevelId, GameLevelGrade grade, int regionId, int spriteId)
    {
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade && m_List[i].RegionId == regionId && m_List[i].SpriteId == spriteId)
            {
                return m_List[i];
            }
        }
        return null;
    }
}
