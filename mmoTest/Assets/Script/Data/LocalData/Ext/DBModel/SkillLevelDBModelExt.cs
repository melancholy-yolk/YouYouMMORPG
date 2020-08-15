using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ܵȼ���չ��
/// </summary>
public partial class SkillLevelDBModel
{

    /// <summary>
    /// ���ݼ���Id�ͼ��ܵȼ� ����ʵ��
    /// </summary>
    public SkillLevelEntity GetEntityBySkillIdAndSkillLevel(int skillId, int skillLevel)
    {
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].SkillId == skillId && m_List[i].Level == skillLevel)
            {
                return m_List[i];
            }
        }
        return null;
    }

}
