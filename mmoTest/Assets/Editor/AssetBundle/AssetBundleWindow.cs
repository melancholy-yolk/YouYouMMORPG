using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

/// <summary>
/// AssetBundle窗口
/// </summary>
public class AssetBundleWindow :  EditorWindow
{
    private AssetBundleDAL dal;//数据访问层(DAL)
    private List<AssetBundleEntity> m_List;
    private Dictionary<string, bool> dic;//<ab实体, 是否打包>
    private Vector2 pos;

    /// <summary>
    /// 需要打包的资源的类型标签
    /// </summary>
    private string[] arrTag = { "All", "Scene", "Role", "Effect", "Audio", "None"};
    private int tagIndex = 0;//标签的索引
    private int selectTagIndex = -1;//选择的标记的索引

    /// <summary>
    /// 打包目标平台
    /// </summary>
    private string[] arrBuildTarget = { "Windows", "Android", "iOS"};

    private int selectBuildTarget = -1;//选择的打包平台索引
#if UNITY_STANDALONE_WIN
    private BuildTarget target = BuildTarget.StandaloneWindows;
    private int buildTargetIndex = 0;//打包的平台索引
#elif UNITY_ANDROID
    private BuildTarget target = BuildTarget.Android;
    private int buildTargetIndex = 1;
#elif UNITY_IPHONE
    private BuildTarget target = BuildTarget.iOS;
    private int buildTargetIndex = 2;
#endif

    /// <summary>
    /// 构造函数
    /// </summary>
    public AssetBundleWindow()
    {
        
    }

    void OnEnable()
    {
        string xmlPath = Application.dataPath + @"\Editor\AssetBundle\AssetBundleConfig.xml";//xml配置文件所在的路径
        dal = new AssetBundleDAL(xmlPath);//实例化数据访问层
        m_List = dal.GetList();//从xml文件中读取得到要打包的ab实体列表

        dic = new Dictionary<string, bool>();
        foreach (var item in m_List)
        {
            dic[item.Key] = true;
        }
    }

    /// <summary>
    /// 实时绘制
    /// </summary>
    void OnGUI()
    {
        if(m_List == null) return;

        #region 按钮行
        GUILayout.BeginHorizontal("box");

        selectTagIndex = EditorGUILayout.Popup(tagIndex, arrTag, GUILayout.Width(100));//根据tag筛选需要打包的ab实体
        if (selectTagIndex != tagIndex)
        {
            tagIndex = selectTagIndex;
            EditorApplication.delayCall = OnSelectTagCallBack;
        }

        selectBuildTarget = EditorGUILayout.Popup(buildTargetIndex, arrBuildTarget, GUILayout.Width(100));//选择打ab包平台下拉选框
        if (selectBuildTarget != buildTargetIndex)
        {
            buildTargetIndex = selectBuildTarget;
            EditorApplication.delayCall = OnSelectTargetCallBack;
        }

        if (GUILayout.Button("save settings", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnSaveAssetBundleCallBack;
        }

        if (GUILayout.Button("Clear AssetBundle", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnClearAssetBundleCallBack;
        }

        if (GUILayout.Button("Build AssetBundle", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnBuildAssetBundleCallBack;
        }

        if (GUILayout.Button("拷贝数据表", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCopyDataTable;
        }

        if (GUILayout.Button("生成版本文件", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCreateVersionFileCallBack;
        }

        GUILayout.EndHorizontal();
        #endregion

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("是否勾选", GUILayout.Width(100));
        GUILayout.Label("包名", GUILayout.Width(400));
        GUILayout.Label("标签", GUILayout.Width(200));
        GUILayout.Label("是否文件夹", GUILayout.Width(200));
        GUILayout.Label("是否初始资源", GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        pos = EditorGUILayout.BeginScrollView(pos);

        for (int i = 0; i < m_List.Count; i++ )
        {
            AssetBundleEntity entity = m_List[i];

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");

            dic[entity.Key] = GUILayout.Toggle(dic[entity.Key], "", GUILayout.Width(100));
            GUILayout.Label(entity.Name, GUILayout.Width(400));
            GUILayout.Label(entity.Tag, GUILayout.Width(200));
            GUILayout.Label(entity.IsFolder.ToString(), GUILayout.Width(200));
            GUILayout.Label(entity.IsFirstData.ToString(), GUILayout.Width(200));

            GUILayout.EndHorizontal();

            foreach(string path in entity.PathList)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Space(40);
                GUILayout.Label(path);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();

    }

    #region 生成版本文件
    /// <summary>
    /// 生成版本文件
    /// </summary>
    private void OnCreateVersionFileCallBack()
    {
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string strVersionFilePath = path + "/VersionFile.txt";

        //如果版本文件存在 则删除
        IOUtil.DeleteFile(strVersionFilePath);

        StringBuilder sbContent = new StringBuilder();

        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileInfo[] arrFiles = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < arrFiles.Length; i++)
        {
            FileInfo file = arrFiles[i];
            string fullName = file.FullName;//全名 包含路径扩展名

            //相对路径
            string name = fullName.Substring(fullName.IndexOf(arrBuildTarget[buildTargetIndex]) + arrBuildTarget[buildTargetIndex].Length + 1);

            if (name.Equals(arrBuildTarget[buildTargetIndex], StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            //文件的MD5
            string md5 = EncryptUtil.GetFileMD5(fullName);
            if (md5 == null) continue;

            string size = Math.Ceiling(file.Length / 1024f).ToString();//转换成KB单位

            //是否为初始数据(与xml配置对比获得)
            bool isFirstData = false;
            bool isBreak = false;

            for (int j = 0; j < m_List.Count; j++)
            {
                foreach (string xmlPath in m_List[j].PathList)
                {
                    string tempPath = xmlPath;
                    if (xmlPath.IndexOf(".") != -1)
                    {
                        tempPath = xmlPath.Substring(0, xmlPath.IndexOf("."));
                    }
                    if (name.IndexOf(tempPath, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        isFirstData = m_List[j].IsFirstData;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }

            if (name.IndexOf("DataTable") != -1)
            {
                isFirstData = true;
            }

            string strLine = string.Format("{0} {1} {2} {3}", name, md5, size, isFirstData ? 1 : 0);
            sbContent.AppendLine(strLine);
            
        }
        IOUtil.CreateTextFile(strVersionFilePath, sbContent.ToString());
        Debug.Log("create version file!");
    }
    #endregion 

    #region 将excel数据表拷贝到AssetBundles目录下
    /// <summary>
    /// 拷贝数据表
    /// </summary>
    private void OnCopyDataTable()
    {
        string fromPath = Application.dataPath + "/Download/DataTable";
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex] + "/Download/DataTable";
        IOUtil.CopyDirectory(fromPath, toPath);
        Debug.Log("copy complete!");
    }
    #endregion

    #region 使用代码设置资源的AssetBundle标签
    /// <summary>
    /// 保存设置按钮回调 自动化给文件夹及其子文件夹中的项设置打包标签
    /// </summary>
    private void OnSaveAssetBundleCallBack()
    {
        //拿到需要打包的对象
        List<AssetBundleEntity> list = new List<AssetBundleEntity>();
        foreach (AssetBundleEntity entity in m_List)
        {
            if (dic[entity.Key])//只给我们选中的 IsChecked为true的设置打包标签
            {
                entity.IsChecked = true;
                list.Add(entity);
            }
            else
            {
                entity.IsChecked = false;
                list.Add(entity);
            }
        }

        //循环设置文件夹包括子文件夹里面的项
        for (int i = 0; i < list.Count; i++)
        {
            AssetBundleEntity entity = list[i];
            if (entity.IsFolder)
            {
                //如果这个节点配置的是一个文件夹 那么需要遍历文件夹
                //需要拼凑出绝对路径
                string[] folderArr = new string[entity.PathList.Count];
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    folderArr[j] = Application.dataPath + "/" + entity.PathList[j];
                }
                SaveFolderSettings(folderArr, !entity.IsChecked);
            }
            else
            {
                //如果不是文件夹 只需要设置里面的项
                string fullPath = string.Empty;
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    //拼凑出绝对路径
                    fullPath = Application.dataPath + "/" + entity.PathList[j];
                    SetFileSetting(fullPath, !entity.IsChecked);
                }
                
            }
        }
    }

    /// <summary>
    /// 递归遍历子文件夹
    /// </summary>
    /// <param name="folderArr"></param>
    /// <param name="isSetNull"></param>
    private void SaveFolderSettings(string[] folderArr, bool isSetNull)
    {
        foreach (string folderPath in folderArr)
        {
            //1.先看这个文件夹下的文件
            string[] arrFile = Directory.GetFiles(folderPath);//文件夹下的文件
            //2.对文件进行设置
            foreach (string filePath in arrFile)
            {
                //进行设置
                SetFileSetting(filePath, isSetNull);
            }

            //3.看这个文件夹下的子文件夹
            string[] arrFolder = Directory.GetDirectories(folderPath);
            SaveFolderSettings(arrFolder, isSetNull);
        }
    }

    private void SetFileSetting(string filePath, bool isSetNull)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
        {
            int index = filePath.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
            //路径
            string newPath = filePath.Substring(index);

            //文件名
            string fileName = newPath.Replace("Assets/", "").Replace(fileInfo.Extension, "");

            //后缀
            string variant = fileInfo.Extension.Equals(".unity", StringComparison.CurrentCultureIgnoreCase) ? "unity3d" : "assetbundle";

            AssetImporter importer = AssetImporter.GetAtPath(newPath);
            importer.SetAssetBundleNameAndVariant(fileName, variant);

            if (isSetNull)
            {
                importer.SetAssetBundleNameAndVariant(null, null);
            }

            importer.SaveAndReimport();
        }
    }
    #endregion

    #region 打ab包
    /// <summary>
    /// 打AssetBundle包回调
    /// </summary>
    private void OnBuildAssetBundleCallBack()
    {
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        //打包方法就是一句话
        BuildPipeline.BuildAssetBundles(toPath, BuildAssetBundleOptions.None, target);

        //List<AssetBundleEntity> lstNeedBuild = new List<AssetBundleEntity>();
        //foreach(var item in m_List)
        //{
        //    if(dic[item.Key])
        //    {
        //        lstNeedBuild.Add(item);
        //    }
        //}

        //for (int i = 0; i < lstNeedBuild.Count; i++ )
        //{
        //    Debug.LogFormat("正在打包{0}/{1}", i+1, lstNeedBuild.Count);
        //    BuildAssetBundle(lstNeedBuild[i]);
        //}

        //Debug.Log("打包完毕");
    }
    #endregion

    #region 选择打包平台
    /// <summary>
    /// 选择平台回调
    /// </summary>
    private void OnSelectTargetCallBack()
    {
        switch(buildTargetIndex)
        {
            case 0:
                target = BuildTarget.StandaloneWindows;
                break;
            case 1:
                target = BuildTarget.Android;
                break;
            case 2:
                target = BuildTarget.iOS;
                break;
        }
        Debug.LogFormat("当前打包的平台: " + arrBuildTarget[buildTargetIndex]);
    }
    #endregion

    #region 通关标签快速选择要打包的资源
    /// <summary>
    /// 选择标签回调 将具有这个tag的AB实体选中
    /// </summary>
    private void OnSelectTagCallBack()
    {
        switch(tagIndex)
        {
            case 0://全选
                foreach(AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = true;
                }
                break;
            case 1://Scene
                foreach (AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = entity.Tag.Equals(arrTag[1], StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 2://Role
                foreach (AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = entity.Tag.Equals(arrTag[2], StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 3://Effect
                foreach (AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = entity.Tag.Equals(arrTag[3], StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 4://Audio
                foreach (AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = entity.Tag.Equals(arrTag[4], StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 5://Node
                foreach (AssetBundleEntity entity in m_List)
                {
                    dic[entity.Key] = false;
                }
                break;
        }
        Debug.LogFormat("当前选择的Tag: " + arrTag[tagIndex]);
    }
    #endregion

    /// <summary>
    /// 打包方法
    /// </summary>
    /// <param name="entity"></param>
    private void BuildAssetBundle(AssetBundleEntity entity)
    {
        //AssetBundleBuild[] arrBuild = new AssetBundleBuild[1];

        //AssetBundleBuild build = new AssetBundleBuild();

        //build.assetBundleName = string.Format("{0}.{1}", entity.Name, entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase) ? "unity3d" : "assetbundle");
        ////build.assetBundleVariant = (entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase) ? "unity3d" : "assetbundle");//后缀
        //build.assetNames = entity.PathList.ToArray();

        //arrBuild[0] = build;

        ////打包目标路径
        //string toPath = Application.streamingAssetsPath + "/AssetBundles/" + arrBuildTarget[buildTargetIndex] + entity.ToPath;

        //if (!Directory.Exists(toPath))
        //{
        //    Directory.CreateDirectory(toPath);
        //}

        //BuildPipeline.BuildAssetBundles(toPath, arrBuild, BuildAssetBundleOptions.None, target);
    }

    #region 清空AssetBundles目录
    /// <summary>
    /// 清空ab包资源
    /// </summary>
    private void OnClearAssetBundleCallBack()
    {
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        if(Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Debug.Log("清空完毕");
    }
    #endregion
}
