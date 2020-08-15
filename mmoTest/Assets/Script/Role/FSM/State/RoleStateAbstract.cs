using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleStateAbstract 
{
    public RoleFSMMgr CurrRoleFSMMgr { get; private set; }
    public AnimatorStateInfo CurrRoleAnimatorStateInfo { get; set; }
    protected bool IsChangedState;//�Ƿ��Ѿ����ù�CurrState��ֵ

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="roleFSMMgr"></param>
    public RoleStateAbstract(RoleFSMMgr roleFSMMgr)
    {
        CurrRoleFSMMgr = roleFSMMgr;
    }

    /// <summary>
    /// �����״̬
    /// </summary>
    public virtual void OnEnter() 
    {
        IsChangedState = false;
    }

    /// <summary>
    /// �ڸ�״̬��
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// �뿪��״̬
    /// </summary>
    public virtual void OnLeave() { }
}
