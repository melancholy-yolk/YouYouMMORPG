/// <summary>
/// 类名 : GameServerDBModel
/// 作者 : 北京-边涯
/// 说明 : 
/// 创建日期 : 2020-03-12 15:27:53
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
public partial class GameServerDBModel : MFAbstractSQLDBModel<GameServerEntity>
{
    #region GameServerDBModel 私有构造
    /// <summary>
    /// 私有构造
    /// </summary>
    private GameServerDBModel()
    {

    }
    #endregion

    #region 单例
    private static object lock_object = new object();
    private static GameServerDBModel instance = null;
    public static GameServerDBModel Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lock_object)
                {
                    if (instance == null)
                    {
                        instance = new GameServerDBModel();
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
        get { return "GameServer"; }
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
                _ColumnList = new List<string> { "Id", "Status", "RunStatus", "IsCommand", "IsNew", "Name", "Ip", "Port", "CreateTime", "UpdateTime" };
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
    protected override SqlParameter[] ValueParas(GameServerEntity entity)
    {
        SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", entity.Id) { DbType = DbType.Int32 },
                new SqlParameter("@Status", entity.Status) { DbType = DbType.Byte },
                new SqlParameter("@RunStatus", entity.RunStatus) { DbType = DbType.Byte },
                new SqlParameter("@IsCommand", entity.IsCommand) { DbType = DbType.Boolean },
                new SqlParameter("@IsNew", entity.IsNew) { DbType = DbType.Boolean },
                new SqlParameter("@Name", entity.Name) { DbType = DbType.String },
                new SqlParameter("@Ip", entity.Ip) { DbType = DbType.String },
                new SqlParameter("@Port", entity.Port) { DbType = DbType.Int32 },
                new SqlParameter("@CreateTime", entity.CreateTime) { DbType = DbType.DateTime },
                new SqlParameter("@UpdateTime", entity.UpdateTime) { DbType = DbType.DateTime },
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
    protected override GameServerEntity GetEntitySelfProperty(IDataReader reader, DataTable table)
    {
        GameServerEntity entity = new GameServerEntity();
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
                case "runstatus":
                    if (!(reader["RunStatus"] is DBNull))
                        entity.RunStatus = Convert.ToByte(reader["RunStatus"]);
                    break;
                case "iscommand":
                    if (!(reader["IsCommand"] is DBNull))
                        entity.IsCommand = Convert.ToBoolean(reader["IsCommand"]);
                    break;
                case "isnew":
                    if (!(reader["IsNew"] is DBNull))
                        entity.IsNew = Convert.ToBoolean(reader["IsNew"]);
                    break;
                case "name":
                    if (!(reader["Name"] is DBNull))
                        entity.Name = Convert.ToString(reader["Name"]);
                    break;
                case "ip":
                    if (!(reader["Ip"] is DBNull))
                        entity.Ip = Convert.ToString(reader["Ip"]);
                    break;
                case "port":
                    if (!(reader["Port"] is DBNull))
                        entity.Port = Convert.ToInt32(reader["Port"]);
                    break;
                case "createtime":
                    if (!(reader["CreateTime"] is DBNull))
                        entity.CreateTime = Convert.ToDateTime(reader["CreateTime"]);
                    break;
                case "updatetime":
                    if (!(reader["UpdateTime"] is DBNull))
                        entity.UpdateTime = Convert.ToDateTime(reader["UpdateTime"]);
                    break;
            }
        }
        return entity;
    }
    #endregion

    #endregion
}
