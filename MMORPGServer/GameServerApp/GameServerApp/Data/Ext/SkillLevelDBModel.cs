using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能等级扩展类
/// </summary>
public partial class SkillLevelDBModel
{

    /// <summary>
    /// 根据技能Id和技能等级 返回实体
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
