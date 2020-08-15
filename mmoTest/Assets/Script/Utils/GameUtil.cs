
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
//using XLua;
using UnityEngine.UI;

//[LuaCallCSharp]
public class GameUtil
{
    #region ��ȡ�������
    //��
    static string[] surnameArray = {"˾��", "ŷ��", "��ľ", "�Ϲ�", "����", "�ĺ�", "ξ��", "����", "�ʸ�", "����", "Ľ��", "����", "����", "˾ͽ", "��ԯ", "����", "����", "���",
            "���", "�Ϲ�", "����", "����", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "֣", "��", "��", "��", "��", "��", "Ԭ", "��", "��",
            "��", "��", "��", "��", "��", "κ", "Ҷ", "��", "��", "��", "��", "��", "ʯ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "³", "��", "��",
            "��", "÷", "��", "��", "��", "��", "ͯ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "׿", "��", "��", "��", "��", "��",
            "ξ", "ʩ", "��", "��", "��", "��", "��", "��", "½", "��", "��", "��"};
    //��1�� 
    static string[] male1Array = {"��", "��", "��", "С", "ǧ", "��", "��", "һ", "��", "Ц", "˫", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "С", "��",
            "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ӣ", "��", "��", "ѩ", "��", "��", "��", "��", "��", "Ʈ", "��",
            "��", "��", "ɭ", "��", "˼", "��", "��", "Ԫ", "Ϧ", "��", "��", "��", "��", "��", "а", "��"};
    //��2��        
    static string[] male2Array = {"��", "��", "��", "��", "��", "��", "�", "��", "��", "��", "��", "��", "��", "��", "��", "Ȩ", "��", "��", "��", "��", "��", "ƽ", "��", "��", "ǿ",
            "��", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ȼ", "˳", "��", "��", "��", "��", "��", "־", "��", "��", "��", "��", "Ԫ", "ī", "��", "��", "֮",
            "��", "��", "��", "��", "��", "��", "��", "˪", "ɽ", "��", "�", "ʢ", "�", "��", "��", "��", "ҫ", "��", "��", "��", "��", "��", "�", "��"};
    //Ů1��            
    static string[] female1Array = {"˼", "��", "ҹ", "��", "С", "��", "��", "��", "ӳ", "��", "��", "��", "��", "֮", "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
            "�", "��", "��", "��", "��", "��", "��", "��", "Ц", "��", "Ԫ", "��", "��", "��", "��", "��", "Ľ", "��", "��", "��", "��", "��", "��", "��", "��", "��", "Ѱ",
            "ˮ", "��", "��", "ϧ", "ʫ", "��", "��", "��", "��", "��", "ѩ", "��", "��", "ӭ", "��", "��", "��", "��", "��", "��", "��", "��"};
    //Ů2��
    static string[] female2Array = {"��", "��", "��", "��", "��", "��", "��", "Ƽ", "��", "��", "��", "˪", "��", "˿", "��", "��", "¶", "��", "ܽ", "��", "��", "��", "��", "��", "��",
            "ѩ", "��", "��", "��", "��", "��", "��", "��", "�", "��", "��", "��", "ͮ", "��", "��", "��", "��", "��", "ɺ", "��", "��", "��", "��", "��", "��", "˫", "��",
            "��", "��", "��", "��", "��", "÷", "��", "ɽ", "��", "֮", "��", "��", "��", "��", "��", "��", "��", "ޱ", "��", "��", "��", "��", "��", "ҹ"};

    /// <summary>
    /// ������ɫʱ�������
    /// </summary>
    public static string RandomName()
    {
        string CurName = "";  //��ǰ������

        string[] CopyArray1;
        string[] CopyArray2;

        bool isMale = UnityEngine.Random.Range(0, 2) == 0;

        //�жϽ�ɫ������Ů
        //if(��ɫ����) ���������鸴�Ƶ�CopyArray��
        if (isMale)
        {
            CopyArray1 = new string[male1Array.Length];
            CopyArray2 = new string[male2Array.Length];
            male1Array.CopyTo(CopyArray1, 0);
            male2Array.CopyTo(CopyArray2, 0);
        }
        else
        {
            CopyArray1 = new string[female1Array.Length];
            CopyArray2 = new string[female2Array.Length];
            female1Array.CopyTo(CopyArray1, 0);
            female2Array.CopyTo(CopyArray2, 0);
        }

        int LastNameNum = 0;  //��������
        int TempRan = UnityEngine.Random.Range(1, 11);
        if (TempRan % 3 == 0)
        {
            LastNameNum = 1;
        }
        else
        {
            LastNameNum = 2;
        }

        //�������+�������(����һ���ֻ���������)
        if (LastNameNum == 1)
        {
            int FirstNameIndex = UnityEngine.Random.Range(0, surnameArray.Length);
            int LastName1 = UnityEngine.Random.Range(0, CopyArray1.Length);
            CurName = surnameArray[FirstNameIndex] + CopyArray1[LastName1];
        }
        else if (LastNameNum == 2)
        {
            int FirstNameIndex = UnityEngine.Random.Range(0, surnameArray.Length);
            int LastName1 = UnityEngine.Random.Range(0, CopyArray1.Length);
            int LastName2 = UnityEngine.Random.Range(0, CopyArray2.Length);
            CurName = surnameArray[FirstNameIndex] + CopyArray1[LastName1] + CopyArray2[LastName2];
        }

        return CurName;
    }

    ///// <summary>
    ///// ��ȡͼƬ��Դ
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="picName"></param>
    ///// <returns></returns>
    //public static Sprite LoadSprite(SpriteSourceType type, string picName)
    //{
    //    string path = string.Empty;
    //    switch (type)
    //    {
    //        case SpriteSourceType.GameLevelIco:
    //            path = "UISource/GameLevel/GameLevelIco";
    //            break;
    //        case SpriteSourceType.GameLevelDetail:
    //            path = "UISource/GameLevel/GameLevelDetail";
    //            break;
    //        case SpriteSourceType.WorldMapIco:
    //            path = "UISource/WorldMap";
    //            break;
    //        case SpriteSourceType.WorldMapSmall:
    //            path = "UISource/SmallMap";
    //            break;
    //    }

    //    return Resources.Load(string.Format("{0}/{1}", path, picName), typeof(Sprite)) as Sprite;
    //}

    ///// <summary>
    ///// ��ȡ����ͼƬ
    ///// </summary>
    ///// <param name="goodsId"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public static Sprite LoadGoodsImg(int goodsId, GoodsType type)
    //{
    //    string pathName = string.Empty;
    //    switch (type)
    //    {
    //        case GoodsType.Equip:
    //            pathName = "EquipIco";
    //            break;
    //        case GoodsType.Item:
    //            pathName = "ItemIco";
    //            break;
    //        case GoodsType.Material:
    //            pathName = "MaterialIco";
    //            break;
    //    }

    //    return Resources.Load(string.Format("UISource/{0}/{1}", pathName, goodsId), typeof(Sprite)) as Sprite;
    //}
    #endregion

    #region ����Sprite
    /// <summary>
    /// ��ȡ����ؿ�����ͼ
    /// </summary>
    public static Texture LoadGameLevelMapPic(string picName)
    {
        return Resources.Load<Texture>("UI/GameLevel/GameLevelMap/" + picName);
    }

    /// <summary>
    /// ���عؿ���ͼ��
    /// </summary>
    public static Sprite LoadGameLevelMapItemIcon(string iconName)
    {
        return Resources.Load<Sprite>("UI/GameLevel/GameLevelMapItemIcon/" + iconName);
    }

    /// <summary>
    /// ���عؿ����鴰���ϵ�ͼƬ
    /// </summary>
    public static Sprite LoadGameLevelDetailPic(string picName)
    {
        return Resources.Load<Sprite>("UI/GameLevel/GameLevelDetail/" + picName);
    }

    /// <summary>
    /// ������Ʒͼ��
    /// </summary>
    /// <param name="goodsId">��Ʒ���ͣ�װ�� ��Ʒ ����</param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Sprite LoadGoodsIcon(int goodsId, GoodsType type)
    {
        return Resources.Load<Sprite>(string.Format("UI/Goods/{0}/{1}", type.ToString(), goodsId.ToString()));
    }

    public static Sprite LoadWorldMapItemIcon(string iconName)
    {
        return Resources.Load<Sprite>("UI/WorldMap/WorldMapItemIcon/" + iconName);
    }

    public static Sprite LoadSmallMap(string picName)
    {
        return Resources.Load<Sprite>("UI/SmallMap/" + picName);
    }
    #endregion

    

    /// <summary>
    /// ���빥�����ͺ���� ���ض���״̬������
    /// </summary>
    private static Dictionary<string, RoleAnimatorState> dic;
    public static RoleAnimatorState GetRoleAnimatorState(RoleAttackType type, int index)
    {
        if (dic == null)
        {
            dic = new Dictionary<string, RoleAnimatorState>();
            dic["PhyAttack1"] = RoleAnimatorState.PhyAttack1;
            dic["PhyAttack2"] = RoleAnimatorState.PhyAttack2;
            dic["PhyAttack3"] = RoleAnimatorState.PhyAttack3;
            dic["Skill1"] = RoleAnimatorState.Skill1;
            dic["Skill2"] = RoleAnimatorState.Skill2;
            dic["Skill3"] = RoleAnimatorState.Skill3;
            dic["Skill4"] = RoleAnimatorState.Skill4;
            dic["Skill5"] = RoleAnimatorState.Skill5;
            dic["Skill6"] = RoleAnimatorState.Skill6;
        }
        string key = string.Format("{0}{1}", type == RoleAttackType.PhyAttack ? "PhyAttack" : "Skill", index);
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return RoleAnimatorState.PhyAttack1;
    }

    /// <summary>
    /// ��ȡĿ�����Χ�������
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 currPos, Vector3 targetPos, float distance)
    {
        //����ָ��ֵ�����
        Vector3 v = (currPos - targetPos).normalized;
        //����90�㷶Χ����ת����
        v = Quaternion.Euler(0, UnityEngine.Random.Range(-90f, 90f), 0) * v;
        Vector3 pos = v * distance * UnityEngine.Random.Range(0.8f, 1f);
        Vector3 newPos = targetPos + pos;
        return newPos;
    }

    #region ���μ�� Ѱ�ҵ���
    /// <summary>
    /// ���ҽ�ɫ�̶���Χ�ڵ����е���
    /// </summary>
    /// <param name="pos">��ɫλ��</param>
    /// <param name="attackRange">��Χ</param>
    /// <param name="currRoleCtrl">��ɫ������</param>
    /// <returns></returns>
    public static List<Collider> FindEnemy(RoleCtrl currRoleCtrl, float attackRange)
    {
        //�����ǰû���������� ���������ĵ���
        //������Ϊ���� ���������ص���� Ҫע�⣺����Լ�һ���ᱻ��⵽
        Collider[] colliderArr = Physics.OverlapSphere(currRoleCtrl.transform.position, attackRange, 1 << LayerMask.NameToLayer("Role"));

        List<Collider> colliderList = new List<Collider>();
        colliderList.Clear();
        if (colliderArr != null && colliderArr.Length > 0)
        {
            for (int i = 0; i < colliderArr.Length; i++)
            {
                RoleCtrl ctrl = colliderArr[i].GetComponent<RoleCtrl>();
                if (ctrl != null)
                {
                    //�ų�����
                    if (ctrl.CurrRoleInfo.RoleId != currRoleCtrl.CurrRoleInfo.RoleId)
                    {
                        colliderList.Add(colliderArr[i]);
                    }
                }
            }
        }
        return colliderList;
    }
    #endregion

    #region ���յ��������ǵľ���������
    public static List<Collider> SortEnemyByDistance(List<Collider> list, RoleCtrl currRoleCtrl)
    {
        list.Sort((c1, c2) =>
        {
            int ret = 0;
            if (Vector3.Distance(c1.gameObject.transform.position, currRoleCtrl.transform.position) <
                Vector3.Distance(c2.gameObject.transform.position, currRoleCtrl.transform.position))
            {
                ret = -1;
            }
            else
            {
                ret = 1;
            }
            return ret;
        });
        return list;
    }
    #endregion

    public static float GetPathLength(List<Vector3> list)
    {
        float pathLen = 0f;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1) continue;

            float dis = Vector3.Distance(list[i], list[i+1]);
            pathLen += dis;
        }

        return pathLen;
    }
}