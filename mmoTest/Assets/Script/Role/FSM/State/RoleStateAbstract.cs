using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoleStateAbstract 
{
    public RoleFSMMgr CurrRoleFSMMgr { get; private set; }
    public AnimatorStateInfo CurrRoleAnimatorStateInfo { get; set; }
    protected bool IsChangedState;//是否已经设置过CurrState的值

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr"></param>
    public RoleStateAbstract(RoleFSMMgr roleFSMMgr)
    {
        CurrRoleFSMMgr = roleFSMMgr;
    }

    /// <summary>
    /// 进入该状态
    /// </summary>
    public virtual void OnEnter() 
    {
        IsChangedState = false;
    }

    /// <summary>
    /// 在该状态中
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// 离开该状态
    /// </summary>
    public virtual void OnLeave() { }
}
