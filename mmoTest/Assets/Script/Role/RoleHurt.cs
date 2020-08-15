using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoleHurt 
{
    public Action OnRoleHurt;

    private RoleFSMMgr m_CurrRoleFSMMgr = null;//有限状态机管理器
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

        //更新角色数据信息 减血
        m_CurrRoleFSMMgr.CurrRoleCtrl.CurrRoleInfo.CurrHP -= attackInfo.HurtValue;

        //弹出受伤数值 HUD
        //根据是否暴击 HUD弹出数字的颜色不同
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

        //播放受伤特效
        Transform obj = EffectMgr.Instance.PlayEffect("Download/Prefab/Effect/Common/", "Effect_Hurt");
        obj.position = m_CurrRoleFSMMgr.CurrRoleCtrl.transform.position;
        obj.rotation = m_CurrRoleFSMMgr.CurrRoleCtrl.transform.rotation;
        EffectMgr.Instance.DestroyEffect(obj, 2);

        if (OnRoleHurt != null) OnRoleHurt();//执行受伤事件绑定的委托

        //PVP
        if (SceneMgr.Instance.CurrPlayType == PlayType.PVP)
        {
            //PVP中 如果角色的血量小于等于0 要让角色的血量保持为1 目的是要等服务器通知角色死亡 角色才能播放死亡动画 进入死亡状态
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

        

        //屏幕泛红
        //TODO

        //如果当前角色不是僵直状态
        if (!m_CurrRoleFSMMgr.CurrRoleCtrl.IsRigidity)
        {
            m_CurrRoleFSMMgr.ChangeState(RoleState.Hurt);
        }
        
    }
}
