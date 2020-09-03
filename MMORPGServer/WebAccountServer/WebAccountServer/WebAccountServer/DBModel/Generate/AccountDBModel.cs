/// <summary>
/// 类名 : AccountDBModel
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2020-03-13 13:19:30
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using Mmcoy.Framework.AbstractBase;

/// <summary>
/// DBModel
/// </summary>
public partial class AccountDBModel : MFAbstractSQLDBModel<AccountEntity>
{
    #region AccountDBModel 私有构造
    /// <summary>
    /// 私有构造
    /// </summary>
    private AccountDBModel()
    {

    }
    #endregion

    #region 单例
    private static object lock_object = new object();
    private static AccountDBModel instance = null;
    public static AccountDBModel Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lock_object)
                {
                    if (instance == null)
                    {
                        instance = new AccountDBModel();
                    }
                }
            }
            return instance;
        }
    }
    #endregion

    #region 实现基类的属性和方法

    #region ConnectionString 数据库连接字符串
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    protected override string ConnectionString
    {
        get { return DBConn.DBAccount; }
    }
    #endregion

    #region TableName 表名
    /// <summary>
    /// 表名
    /// </summary>
    protected override string TableName
    {
        get { return "Account"; }
    }
    #endregion

    #region ColumnList 列名集合
    private IList<string> _ColumnList;
    /// <summary>
    /// 列名集合
    /// </summary>
    protected override IList<string> ColumnList
    {
        get
        {
            if (_ColumnList == null)
            {
                _ColumnList = new List<string> { "Id", "Status", "UserName", "Pwd", "Mobile", "Mail", "Money", "Channel", "LastLogOnServerId", "LastLogOnServerName", "LastLogOnServerTime", "LastLogOnRoleId", "LastLogOnRoleNickName", "LastLogOnRoleJobId", "CreateTime", "UpdateTime", "DeviceIdentifier", "DeviceModel" };
            }
            return _ColumnList;
        }
    }
    #endregion

    #region ValueParas 转换参数
    /// <summary>
    /// 转换参数
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected override SqlParameter[] ValueParas(AccountEntity entity)
    {
        SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", entity.Id) { DbType = DbType.Int32 },
                new SqlParameter("@Status", entity.Status) { DbType = DbType.Byte },
                new SqlParameter("@UserName", entity.UserName) { DbType = DbType.String },
                new SqlParameter("@Pwd", entity.Pwd) { DbType = DbType.String },
                new SqlParameter("@Mobile", entity.Mobile) { DbType = DbType.String },
                new SqlParameter("@Mail", entity.Mail) { DbType = DbType.String },
                new SqlParameter("@Money", entity.Money) { DbType = DbType.Int32 },
                new SqlParameter("@Channel", entity.Channel) { DbType = DbType.Int16 },
                new SqlParameter("@LastLogOnServerId", entity.LastLogOnServerId) { DbType = DbType.Int32 },
                new SqlParameter("@LastLogOnServerName", entity.LastLogOnServerName) { DbType = DbType.String },
                new SqlParameter("@LastLogOnServerTime", entity.LastLogOnServerTime) { DbType = DbType.DateTime },
                new SqlParameter("@LastLogOnRoleId", entity.LastLogOnRoleId) { DbType = DbType.Int32 },
                new SqlParameter("@LastLogOnRoleNickName", entity.LastLogOnRoleNickName) { DbType = DbType.String },
                new SqlParameter("@LastLogOnRoleJobId", entity.LastLogOnRoleJobId) { DbType = DbType.Int32 },
                new SqlParameter("@CreateTime", entity.CreateTime) { DbType = DbType.DateTime },
                new SqlParameter("@UpdateTime", entity.UpdateTime) { DbType = DbType.DateTime },
                new SqlParameter("@DeviceIdentifier", entity.DeviceIdentifier) { DbType = DbType.String },
                new SqlParameter("@DeviceModel", entity.DeviceModel) { DbType = DbType.String },
                new SqlParameter("@RetMsg", SqlDbType.NVarChar, 255),
                new SqlParameter("@ReturnValue", SqlDbType.Int)
            };
        return parameters;
    }
    #endregion

    #region GetEntitySelfProperty 封装对象
    /// <summary>
    /// 封装对象
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    protected override AccountEntity GetEntitySelfProperty(IDataReader reader, DataTable table)
    {
        AccountEntity entity = new AccountEntity();
        foreach (DataRow row in table.Rows)
        {
            var colName = row.Field<string>(0);
            if (reader[colName] is DBNull)
                continue;
            switch (colName.ToLower())
            {
                case "id":
                    if (!(reader["Id"] is DBNull))
                        entity.Id = Convert.ToInt32(reader["Id"]);
                    break;
                case "status":
                    if (!(reader["Status"] is DBNull))
                        entity.Status = (EnumEntityStatus)Convert.ToInt32(reader["Status"]);
                    break;
                case "username":
                    if (!(reader["UserName"] is DBNull))
                        entity.UserName = Convert.ToString(reader["UserName"]);
                    break;
                case "pwd":
                    if (!(reader["Pwd"] is DBNull))
                        entity.Pwd = Convert.ToString(reader["Pwd"]);
                    break;
                case "mobile":
                    if (!(reader["Mobile"] is DBNull))
                        entity.Mobile = Convert.ToString(reader["Mobile"]);
                    break;
                case "mail":
                    if (!(reader["Mail"] is DBNull))
                        entity.Mail = Convert.ToString(reader["Mail"]);
                    break;
                case "money":
                    if (!(reader["Money"] is DBNull))
                        entity.Money = Convert.ToInt32(reader["Money"]);
                    break;
                case "channel":
                    if (!(reader["Channel"] is DBNull))
                        entity.Channel = Convert.ToInt16(reader["Channel"]);
                    break;
                case "lastlogonserverid":
                    if (!(reader["LastLogOnServerId"] is DBNull))
                        entity.LastLogOnServerId = Convert.ToInt32(reader["LastLogOnServerId"]);
                    break;
                case "lastlogonservername":
                    if (!(reader["LastLogOnServerName"] is DBNull))
                        entity.LastLogOnServerName = Convert.ToString(reader["LastLogOnServerName"]);
                    break;
                case "lastlogonservertime":
                    if (!(reader["LastLogOnServerTime"] is DBNull))
                        entity.LastLogOnServerTime = Convert.ToDateTime(reader["LastLogOnServerTime"]);
                    break;
                case "lastlogonroleid":
                    if (!(reader["LastLogOnRoleId"] is DBNull))
                        entity.LastLogOnRoleId = Convert.ToInt32(reader["LastLogOnRoleId"]);
                    break;
                case "lastlogonrolenickname":
                    if (!(reader["LastLogOnRoleNickName"] is DBNull))
                        entity.LastLogOnRoleNickName = Convert.ToString(reader["LastLogOnRoleNickName"]);
                    break;
                case "lastlogonrolejobid":
                    if (!(reader["LastLogOnRoleJobId"] is DBNull))
                        entity.LastLogOnRoleJobId = Convert.ToInt32(reader["LastLogOnRoleJobId"]);
                    break;
                case "createtime":
                    if (!(reader["CreateTime"] is DBNull))
                        entity.CreateTime = Convert.ToDateTime(reader["CreateTime"]);
                    break;
                case "updatetime":
                    if (!(reader["UpdateTime"] is DBNull))
                        entity.UpdateTime = Convert.ToDateTime(reader["UpdateTime"]);
                    break;
                case "deviceidentifier":
                    if (!(reader["DeviceIdentifier"] is DBNull))
                        entity.DeviceIdentifier = Convert.ToString(reader["DeviceIdentifier"]);
                    break;
                case "devicemodel":
                    if (!(reader["DeviceModel"] is DBNull))
                        entity.DeviceModel = Convert.ToString(reader["DeviceModel"]);
                    break;
            }
        }
        return entity;
    }
    #endregion

    #endregion
}
