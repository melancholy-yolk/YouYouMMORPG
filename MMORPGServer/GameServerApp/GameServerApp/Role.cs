using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServerApp
{
    /// <summary>
    /// 一个Role对象 对应一个与服务器建立连接的客户端
    /// </summary>
    public class Role
    {
        public ClientSocket Client_Socket { get; set; }
        public int AccountId { get; set; }//账号Id
        public int RoleId { get; set; }//角色Id
        public string NickName { get; set; }//角色昵称
        public int Level { get; set; }
        public int JobId { get; set; }

        public int MaxHP { get; set; }
        public int CurrHP { get; set; }
        public int MaxMP { get; set; }
        public int CurrMP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Hit { get; set; }
        public int Dodge { get; set; }
        public int Cri { get; set; }
        public int Res { get; set; }
        public int Fighting { get; set; }

        public int PreviousWorldMapId { get; set; }//上一个所在的场景编号 用于跳转场景的时候 给旧场景玩家发消息 告诉他们我们离开
        public int LastInWorldMapId { get; set; }//最后进入的世界地图编号
        public string LastInWorldMapPos { get; set; }//最后所在世界地图坐标
        
        public void UpdateLastInWorldMap()//更新玩家在世界地图的信息
        {
            Dictionary<string, object> param = new Dictionary<string, object>();

            param["@Id"] = RoleId;
            param["@LastInWorldMapId"] = LastInWorldMapId;
            param["@LastInWorldMapPos"] = LastInWorldMapPos;

            RoleCacheModel.Instance.Update("LastInWorldMapId=@LastInWorldMapId, LastInWorldMapPos=@LastInWorldMapPos", "Id=@Id", param);
        }
    }
}
