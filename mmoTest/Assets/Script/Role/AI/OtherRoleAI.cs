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
        //·������
        float pathLength = GameUtil.GetPathLength(p.vectorPath);
        //��������ӳ�ʱ�� = ��ǰ������ʱ�� - Э�鷢�����ķ�����ʱ��
        long delayTime = GlobalInit.Instance.GetCurrServerTime() - m_ServerTime;
        //ʵ�ʸ���������ƶ���ʱ�� = �ƶ��������ʱ�� - �ӳ�ʱ��
        long realMoveTime = m_NeedTime - delayTime;
        if (realMoveTime <= 0) realMoveTime = 100;
        CurrRole.ModifySpeed = Mathf.Clamp(pathLength / (realMoveTime * 0.001f), 0, 20f);
        CurrRole.MoveTo(m_TargetPos);
    }
}
