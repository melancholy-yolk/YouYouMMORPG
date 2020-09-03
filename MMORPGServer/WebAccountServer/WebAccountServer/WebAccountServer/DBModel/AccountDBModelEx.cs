using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mmcoy.Framework;
using System.Data.SqlClient;
using Mmcoy.Framework.AbstractBase;

public partial class AccountDBModel
{
    private const string connStr = "Data Source=127.0.0.1;Initial Catalog=DBAccount;Persist Security Info=True;User ID=sa;Password=113911cwb";

    public MFReturnValue<int> Register(string userName, string pwd, short channelID, string DeviceIdentifier, string DeviceModel)
    {
        MFReturnValue<int> retValue = new MFReturnValue<int>();

        //1.验证用户名是否存在
        //2.如果不存在 添加数据

        using (SqlConnection conn = new SqlConnection(DBConn.DBAccount))
        {
            conn.Open();

            //开始事务 只打开一次数据库
            SqlTransaction trans = conn.BeginTransaction();
            List<AccountEntity> lst = this.GetListWithTran(this.TableName, "Id", "UserName='" + userName + "'", isAutoStatus:false, trans: trans);
            //没有重名
            if (lst == null || lst.Count == 0)
            {
                AccountEntity entity = new AccountEntity();
                entity.Status = EnumEntityStatus.Released;
                entity.UserName = userName;
                entity.Pwd = MFEncryptUtil.Md5(pwd);
                entity.Channel = channelID;
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;
                entity.LastLogOnServerTime = DateTime.Now;
                entity.DeviceIdentifier = DeviceIdentifier;
                entity.DeviceModel = DeviceModel;

                MFReturnValue<object> ret = this.Create(trans, entity);//插入一条记录
                if (!ret.HasError)
                {
                    retValue.Value = (int)ret.OutputValues["Id"];
                    trans.Commit();
                }
                else
                {
                    retValue.HasError = true;
                    retValue.Message = "用户名已经存在!";
                    trans.Rollback();//回滚事务
                }
            }
            else
            {
                retValue.HasError = true;
                retValue.Message = "用户名已经存在!";
            }
        }

        return retValue;
    }

    public AccountEntity LogOn(string userName, string pwd, string DeviceIdentifier, string DeviceModel)
    {
        string condition = string.Format("[UserName]='{0}' and [Pwd]='{1}'", userName, MFEncryptUtil.Md5(pwd));
        AccountEntity entity = this.GetEntity(condition);

        if (entity != null)
        {
            entity.DeviceIdentifier = DeviceIdentifier;
            entity.DeviceModel = DeviceModel;
            this.Update(entity);
        }

        return entity;
    }

}