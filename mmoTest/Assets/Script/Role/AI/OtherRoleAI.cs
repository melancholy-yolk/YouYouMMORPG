using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherRoleAI : IRoleAI 
{
    public RoleCtrl CurrRole { get; set; }

    private Vector3 m_TargetPos;
    private long m_ServerTime;
    private int m_NeedTime;

    public OtherRoleAI(RoleCtrl roleCtrl)
    {
        CurrRole = roleCtrl;
    }

    public void DoAI()
    {
        
    }

    public void MoveTo(Vector3 targetPos, long serverTime, int needTime)
    {
        m_TargetPos = targetPos;
        m_ServerTime = serverTime;
        m_NeedTime = needTime;

        CurrRole.Seeker.StartPath(CurrRole.transform.position, targetPos, OnAStarFinish);
    }

    private void OnAStarFinish(Path p)
    {
        //路径长度
        float pathLength = GameUtil.GetPathLength(p.vectorPath);
        //这个包的延迟时间 = 当前服务器时间 - 协议发过来的服务器时间
        long delayTime = GlobalInit.Instance.GetCurrServerTime() - m_ServerTime;
        //实际给其他玩家移动的时间 = 移动所需的总时间 - 延迟时间
        long realMoveTime = m_NeedTime - delayTime;
        if (realMoveTime <= 0) realMoveTime = 100;
        CurrRole.ModifySpeed = Mathf.Clamp(pathLength / (realMoveTime * 0.001f), 0, 20f);
        CurrRole.MoveTo(m_TargetPos);
    }
}
