using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleHurt 
{
    public Action OnRoleHurt;

    private RoleFSMMgr m_CurrRoleFSMMgr = null;//����״̬��������
    public RoleHurt(RoleFSMMgr fsm)
    {
        m_CurrRoleFSMMgr = fsm;
    }

    public IEnumerator ToHurt(RoleTransferAttackInfo attackInfo)
    {
        if (m_CurrRoleFSMMgr == null) yield break;

        if (m_CurrRoleFSMMgr.CurrRoleStateEnum == RoleState.Die) yield break;

        SkillEntity skillEntity = SkillDBModel.Instance.GetEntity(attackInfo.SkillId);
        SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndSkillLevel(attackInfo.SkillId, attackInfo.SkillLevel);
        if (skillEntity == null || skillLevelEntity == null) yield break;

        yield return new WaitForSeconds(skillEntity.ShowHurtEffectDelaySecond);

        //���½�ɫ������Ϣ ��Ѫ
        m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP -= attackInfo.HurtValue;

        //����������ֵ HUD
        //�����Ƿ񱩻� HUD�������ֵ���ɫ��ͬ
        Color color = Color.red;
        int fontSize = 4;
        if (attackInfo.IsCri)
        {
            color = Color.yellow;
            fontSize = 8;
        }

        SceneUIManager.Instance.CurrentUIScene.HUDText.NewText(
            "-" + attackInfo.HurtValue, m_CurrRoleFSMMgr.CurrRoleCtrl.transform,
            color, size: fontSize, speed: 10f, yAcceleration: -1f, yAccelerationScaleFactor: 2.2f, movement: UnityEngine.Random.Range(0, 2) == 1 ? bl_Guidance.LeftDown : bl_Guidance.RightDown);

        //����������Ч
        Transform obj = EffectMgr.Instance.PlayEffect("Download/Prefab/Effect/Common/", "Effect_Hurt");
        obj.position = m_CurrRoleFSMMgr.CurrRoleCtrl.transform.position;
        obj.rotation = m_CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation;
        EffectMgr.Instance.DestroyEffect(obj, 2);

        if (OnRoleHurt != null) OnRoleHurt();//ִ�������¼��󶨵�ί��

        //PVP
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
        {
            //PVP�� �����ɫ��Ѫ��С�ڵ���0 Ҫ�ý�ɫ��Ѫ������Ϊ1 Ŀ����Ҫ�ȷ�����֪ͨ��ɫ���� ��ɫ���ܲ����������� ��������״̬
            if (m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP <= 0)
            {
                m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP = 1;

                yield break;
            }
        }
        else//PVE
        {
            if (m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP <= 0)
            {
                m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP = 0;
                m_CurrRoleFSMMgr.CurrRoleCtrl.ToDie();

                yield break;
            }
        }

        

        //��Ļ����
        //TODO

        //�����ǰ��ɫ���ǽ�ֱ״̬
        if (!m_CurrRoleFSMMgr.CurrRoleCtrl.IsRigidity)
        {
            m_CurrRoleFSMMgr.ChangeState(RoleState.Hurt);
        }
        
    }
}
