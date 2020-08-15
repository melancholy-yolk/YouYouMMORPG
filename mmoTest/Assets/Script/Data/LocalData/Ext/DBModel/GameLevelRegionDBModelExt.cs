using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelRegionDBModel 
{
    private List<GameLevelRegionEntity> regionList = new List<GameLevelRegionEntity>();

    //通过关卡Id 得到关卡区域列表
    public List<GameLevelRegionEntity> GetListByGameLevelId(int gameLevel)
    {
        regionList.Clear();

        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevel)
            {
                regionList.Add(m_List[i]);
            }
        }

        return regionList;
    }
}
