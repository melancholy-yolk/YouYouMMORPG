using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʵ��
/// </summary>
public class RetGameServerEntity
{
    public int Id;//���
    public int RunStatus;//״̬
    public bool IsCommand;//�Ƿ��Ƽ�
    public bool IsNew;//�Ƿ��·�
    public string Name;//����
    public string Ip;//ip��ַ
    public int Port;//�˿�

    public override string ToString()
    {
        return string.Format("{0}={1}={2}={3}={4}={5}={6}={7}", Id,RunStatus,IsCommand,IsNew,Name,Ip,Port);
    }
}
