using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区服实体
/// </summary>
public class RetGameServerEntity
{
    public int Id;//编号
    public int RunStatus;//状态
    public bool IsCommand;//是否推荐
    public bool IsNew;//是否新服
    public string Name;//名称
    public string Ip;//ip地址
    public int Port;//端口

    public override string ToString()
    {
        return string.Format("{0}={1}={2}={3}={4}={5}={6}={7}", Id,RunStatus,IsCommand,IsNew,Name,Ip,Port);
    }
}
