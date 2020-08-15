using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ��ɫ������ ����������Դ���
/// </summary>
public class RoleMgr : Singleton<RoleMgr> 
{
    private bool m_IsMainPlayerInit = false;//�����Ƿ��Ѿ���ʼ��

    /// <summary>
    /// ��ʼ������ ʵ��������ģ�� �����ǽű���ֵ
    /// </summary>
    public void InitMainPlayer()
    {
        if (m_IsMainPlayerInit) return;

        if (GlobalInit.Instance.MainPlayerInfo != null)
        {
            GameObject mainPlayerObj = GameObject.Instantiate<GameObject>(GlobalInit.Instance.jobPrefabDic[GlobalInit.Instance.MainPlayerInfo.JobId]);
            GameObject.DontDestroyOnLoad(mainPlayerObj);
            //��excel�ж�ȡ��ְҵ����ʹ�õ��չ�
            GlobalInit.Instance.MainPlayerInfo.SetPhySkillId(JobDBModel.Instance.GetEntity(GlobalInit.Instance.MainPlayerInfo.JobId).UsedPhyAttackIds);
            GlobalInit.Instance.MainPlayer = mainPlayerObj.GetComponent<RoleCtrl>();
            GlobalInit.Instance.MainPlayer.Init(RoleType.MainPlayer, GlobalInit.Instance.MainPlayerInfo, new RoleMainPlayerCityAI(GlobalInit.Instance.MainPlayer));
        }

        m_IsMainPlayerInit = true;
    }

    /// <summary>
    /// ��assetbundle�м���NPCԤ����
    /// </summary>
    public GameObject LoadNPC(string prefabName)
    {
        GameObject obj = AssetBundleMgr.Instance.Load(string.Format("Download/Prefab/RolePrefab/NPC/{0}.assetbundle", prefabName), prefabName);
        return GameObject.Instantiate<GameObject>(obj);
    }

    /// <summary>
    /// ���ǽ�ɫװ�����鴰�ڽ�ɫģ��ʵ����
    /// </summary>
    public GameObject LoadPlayer(int jobId)
    {
        GameObject obj = GlobalInit.Instance.jobPrefabDic[jobId];
        return GameObject.Instantiate<GameObject>(obj);
    }

    /// <summary>
    /// ���ع���Ԥ����
    /// </summary>
    public GameObject LoadSprite(int spriteId)
    {
        SpriteEntity entity = SpriteDBModel.Instance.GetEntity(spriteId);
        if (entity == null) return null;

        return AssetBundleMgr.Instance.Load(string.Format("Download/Prefab/RolePrefab/Monster/{0}.assetbundle", entity.PrefabName), entity.PrefabName);
    }

    public Sprite LoadHeadPic(string headPic)
    {
        return Resources.Load<Sprite>("HeadImg/" + headPic);
    }

    public Sprite LoadSkikllPic(string skillPic)
    {
        return Resources.Load<Sprite>("UI/SkillIcon/" + skillPic);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="roleNickName"></param>
    /// <param name="roleLevel"></param>
    /// <param name="roleJobId"></param>
    /// <returns></returns>
    internal RoleCtrl LoadOtherRole(int roleId, string roleNickName, int roleLevel, int roleJobId, int maxHP, int currHP, int maxMP, int currMP)
    {
        GameObject obj = Object.Instantiate<GameObject>(GlobalInit.Instance.jobPrefabDic[roleJobId]);
        RoleCtrl roleCtrl = obj.GetComponent<RoleCtrl>();

        RoleInfoMainPlayer roleInfo = new RoleInfoMainPlayer();
        roleInfo.RoleId = roleId;
        roleInfo.RoleNickName = roleNickName;
        roleInfo.Level = roleLevel;
        roleInfo.JobId = (byte)roleJobId;
        roleInfo.MaxHP = maxHP;
        roleInfo.CurrHP = currHP;
        roleInfo.MaxMP = maxMP;
        roleInfo.CurrMP = currMP;

        roleCtrl.Init(RoleType.OtherPlayer, roleInfo, new OtherRoleAI(roleCtrl));

        return roleCtrl;
    }
}
