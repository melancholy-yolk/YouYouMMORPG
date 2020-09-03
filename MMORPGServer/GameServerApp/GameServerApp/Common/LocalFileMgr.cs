using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 本地文件管理
/// </summary>
public class LocalFileMgr : Singleton<LocalFileMgr>
{
    public readonly string LocalFilePath = "";

    /// <summary>
    /// 从文件读取字节流
    /// </summary>
    public byte[] GetBuffer(string path)
    {
        byte[] buffer = null;

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
        }
        return buffer;
    }

}