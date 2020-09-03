using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LitJson;
using Mmcoy.Framework;

namespace WebAccountServer.Controllers.api
{
    public class AccountController : ApiController
    {
        // GET: api/Account
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        public int Get(int id)
        {
            //AccountEntity entity = AccountDBModel.Instance.Get(id);
            //return entity;
            return id;
        }

        // POST: api/Account
        public ReturnValue Post([FromBody]string jsonStr)
        {
            ReturnValue ret = new ReturnValue();
            MFReturnValue<int> mfRet = null;

            JsonData jd = JsonMapper.ToObject(jsonStr);

            #region web安全验证
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
            string userName = jd["UserName"].ToString();
            string pwd = jd["Pwd"].ToString();
            

            if (type == 0)//注册
            {
                short channelId = Convert.ToInt16(jd["ChannelId"].ToString());
                mfRet = AccountCacheModel.Instance.Register(userName, pwd, channelId, DeviceIdentifier, DeviceModel);
                ret.HasError = mfRet.HasError;
                ret.ErrorMsg = mfRet.Message;

                int userId = mfRet.Value;
                AccountEntity account = AccountCacheModel.Instance.GetEntity(userId);
                ret.ReturnData = JsonMapper.ToJson(new RetAccountEntity(account));
            }
            else//登录
            {
                AccountEntity entity = AccountCacheModel.Instance.LogOn(userName, pwd, DeviceIdentifier, DeviceModel);
                if (entity != null)
                {
                    ret.HasError = false;
                    ret.ReturnData = JsonMapper.ToJson(new RetAccountEntity(entity));
                }
                else
                {
                    ret.HasError = true;
                    ret.ErrorMsg = "账户不存在";
                }
            }

            
            return ret;
        }

        // PUT: api/Account/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Account/5
        public void Delete(int id)
        {
        }
    }
}
