using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoleInfoEquipView : MonoBehaviour 
{
    [SerializeField]
    private Transform roleModelContainer;//角色模型容器
    [SerializeField]
    private Text textNickName;
    [SerializeField]
    private Text textLevel;
    [SerializeField]
    private Text textFighting;

    /// <summary>
    /// 职业编号
    /// </summary>
    private int m_JobId;
	
	void Start () 
    {
        //CloneRoleModel();
	}

    public void SetUI(TransferData data)
    {
        textNickName.text = data.GetValue<string>(ConstDefine.NickName);
        textLevel.text = "LV." + data.GetValue<int>(ConstDefine.Level);
        textFighting.text = data.GetValue<int>(ConstDefine.Fighting).ToString();

        m_JobId = data.GetValue<int>(ConstDefine.JobId);
    }

    /// <summary>
    /// 实例化出角色模型
    /// </summary>
    public void CloneRoleModel()
    {
        GameObject player = RoleMgr.Instance.LoadPlayer(m_JobId);
        player.SetParent(roleModelContainer);
        player.SetLayer("UI");
    }
}
