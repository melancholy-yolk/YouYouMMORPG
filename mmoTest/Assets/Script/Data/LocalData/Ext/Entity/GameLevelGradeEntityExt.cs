using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡等级实体扩展
/// </summary>
public partial class GameLevelGradeEntity 
{
    /// <summary>
    /// 当前关卡难度等级
    /// </summary>
    public GameLevelGrade CurrGrade
    {
        get { return (GameLevelGrade)Grade; }
    }

    private List<GoodsEntity> m_EquipList;

    public List<GoodsEntity> EquipList
    {
        get
        {
            if (m_EquipList == null)
            {
                m_EquipList = new List<GoodsEntity>();
                string[] arr = Equip.Split('|');
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = arr[i].Split('_');//id_probability_count
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity good = new GoodsEntity();
                            int id = 0, probability = 0, count = 0;
                            int.TryParse(arr2[0], out id);
                            int.TryParse(arr2[1], out probability);
                            int.TryParse(arr2[2], out count);
                            string name = string.Empty;
                            EquipEntity equip = EquipDBModel.Instance.GetEntity(id);
                            if (equip != null)
                            {
                                name = equip.Name;
                            }
                            good.Id = id;
                            good.Name = name;
                            good.Probability = probability;
                            good.Count = count;
                            m_EquipList.Add(good);
                        }
                    }
                }
            }
            return m_EquipList;
        }
    }

    private List<GoodsEntity> m_ItemList;

    public List<GoodsEntity> ItemList
    {
        get
        {
            if (m_ItemList == null)
            {
                m_ItemList = new List<GoodsEntity>();
                string[] arr = Item.Split('|');
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = arr[i].Split('_');
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity good = new GoodsEntity();
                            int id = 0, probability = 0, count = 0;
                            int.TryParse(arr2[0], out id);
                            int.TryParse(arr2[1], out probability);
                            int.TryParse(arr2[2], out count);
                            string name = string.Empty;
                            ItemEntity equip = ItemDBModel.Instance.GetEntity(id);
                            if (equip != null)
                            {
                                name = equip.Name;
                            }
                            good.Id = id;
                            good.Name = name;
                            good.Probability = probability;
                            good.Count = count;
                            m_ItemList.Add(good);
                        }
                    }
                }
            }
            return m_ItemList;
        }
    }

    private List<GoodsEntity> m_MaterialList;

    public List<GoodsEntity> MaterialList
    {
        get
        {
            if (m_MaterialList == null)
            {
                m_MaterialList = new List<GoodsEntity>();
                string[] arr = Material.Split('|');
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = arr[i].Split('_');
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity good = new GoodsEntity();
                            int id = 0, probability = 0, count = 0;
                            int.TryParse(arr2[0], out id);
                            int.TryParse(arr2[1], out probability);
                            int.TryParse(arr2[2], out count);
                            string name = string.Empty;
                            MaterialEntity equip = MaterialDBModel.Instance.GetEntity(id);
                            if (equip != null)
                            {
                                name = equip.Name;
                            }
                            good.Id = id;
                            good.Name = name;
                            good.Probability = probability;
                            good.Count = count;
                            m_MaterialList.Add(good);
                        }
                    }
                }
            }
            return m_MaterialList;
        }
    }

}
