using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SpriteEntity 
{
    private int[] m_UsedPhyAttackArr;
    /// <summary>
    /// �����ʹ�õ��������������
    /// </summary>
    public int[] UsedPhyAttackArr
    {
        get
        {
            if (string.IsNullOrEmpty(this.UsedPhyAttack))
            {
                return null;
            }
            if (m_UsedPhyAttackArr == null)
            {
                string[] arr = this.UsedPhyAttack.Split('_');
                m_UsedPhyAttackArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    m_UsedPhyAttackArr[i] = arr[i].ToInt();
                }
            }
            return m_UsedPhyAttackArr;
        }
    }

    private int[] m_UsedSkillAttackArr;
    /// <summary>
    /// �����ʹ�õļ��ܹ����������
    /// </summary>
    public int[] UsedSkillAttackArr
    {
        get
        {
            if (string.IsNullOrEmpty(this.UsedSkillList))
            {
                return null;
            }
            if (m_UsedSkillAttackArr == null)
            {
                string[] arr = this.UsedSkillList.Split('_');
                m_UsedSkillAttackArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    m_UsedSkillAttackArr[i] = arr[i].ToInt();
                }
            }
            return m_UsedSkillAttackArr;
        }
    }

}
