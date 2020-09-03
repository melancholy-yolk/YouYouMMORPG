using LitJson;
using Mmcoy.Framework;
using Mmcoy.Framework.AbstractBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAccountServer.Controllers.api
{
    public class GameServerController : ApiController
    {
        // GET: api/GameServer
        public IEnumerable<string> Get()
        {
            //快速给数据库中插入假数据
            //for (int i = 1; i < 38; i++)
            //{
            //    GameServerCacheModel.Instance.Create(new GameServerEntity()
            //    {
            //        Status = EnumEntityStatus.Released,
            //        RunStatus = 1,
            //        IsCommand = false,
            //        IsNew = false,
            //        Name = "群雄争霸" + i,
            //        Ip = "127.0.0.1",
            //        Port = 1000 + i,
            //        CreateTime = DateTime.Now,
            //        UpdateTime = DateTime.Now
            //    });
            //}
            return new string[] { "value1", "value2" };
        }

        // GET: api/GameServer/5
        public List<RetGameServerEntity> Get(int id)
        {
            return null;
        }

        // POST: api/GameServer
        public object Post([FromBody]string value)
        {
            ReturnValue ret = new ReturnValue();

            JsonData jd = JsonMapper.ToObject(value);

            #region web安全性验证
            long t = Convert.ToInt64(jd["t"].ToString());//时间戳
            string DeviceIdentifier = jd["DeviceIdentifier"].ToString();//客户端设备Id
            string DeviceModel = jd["DeviceModel"].ToString();//客户端设备型号
            string sign = jd["sign"].ToString();//签名

            //1.判断时间戳 大于3秒
            if (Math.Abs(MFDSAUtil.GetTimestamp() - t) > 3)
            {
                ret.HasError = true;
                ret.ErrorMsg = "时间戳非法";
                return ret;
            }

            //2.验证签名
            string signServer = MFEncryptUtil.Md5(string.Format("{0}:{1}", t, DeviceIdentifier));
            if (!signServer.Equals(sign, StringComparison.CurrentCultureIgnoreCase))
            {
                ret.HasError = true;
                ret.ErrorMsg = "签名无效";
                return ret;
            }
            #endregion

            int type = Convert.ToInt32(jd["Type"].ToString());

            if (type == 0)//获取页签
            {
                return GameServerCacheModel.Instance.GetGameServerPageList();
            }
            else if(type == 1)//获取区服列表
            {
                int pageIndex = Convert.ToInt32(jd["PageIndex"].ToString());
                return GameServerCacheModel.Instance.GetGameServerList(pageIndex);
            }
            else if (type == 2)//更新最后登录信息
            {
                int userId = Convert.ToInt32(jd["UserId"].ToString());
                int lastServerId = Convert.ToInt32(jd["LastServerId"].ToString());
                string lastServerName = jd["LastServerName"].ToString();

                Dictionary<string, object> dic = new Dictionary<string,object>();
                dic["Id"] = userId;
                dic["LastLogOnServerId"] = lastServerId;
                dic["LastLogOnServerName"] = lastServerName;
                dic["LastLogOnServerTime"] = DateTime.Now;
                AccountCacheModel.Instance.Update("LastLogOnServerId=@LastLogOnServerId,LastLogOnServerName=@LastLogOnServerName,LastLogOnServerTime=@LastLogOnServerTime", "Id=@Id", dic);
            }

            return ret;
        }

        // PUT: api/GameServer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GameServer/5
        public void Delete(int id)
        {
        }
    }
}
