using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMgr : Singleton<EffectMgr> 
{
    /// <summary>
    /// ��AssetBundle�м��ؽ�ɫ������Ч
    /// </summary>
    public GameObject LoadEffect(string path, string prefabName)
    {
        return AssetBundleMgr.Instance.Load(string.Format("{0}{1}.assetbundle", path, prefabName), prefabName);
    }

    private SpawnPool EffectPool;
    private MonoBehaviour Mono;
    private Dictionary<string, Transform> EffectDic = new Dictionary<string, Transform>();

    public void Init(MonoBehaviour mono)
    {
        Mono = mono;
        EffectPool = PoolManager.Pools.Create("Effect");
        EffectDic.Clear();
    }

    public Transform PlayEffect(string path, string effectName)
    {
        if (!EffectDic.ContainsKey(effectName))
        {
            EffectDic[effectName] = LoadEffect(path, effectName).transform;
            PrefabPool prefabPool = new PrefabPool(EffectDic[effectName]);
            prefabPool.preloadAmount = 0;

            prefabPool.cullDespawned = true;//�Ƿ���������Զ�����ģʽ
            prefabPool.cullAbove = 5;//������Զ����� ����ʼ�ձ���������������
            prefabPool.cullDelay = 2;//�೤ʱ������һ�� ��λ����
            prefabPool.cullMaxPerPass = 2;//ÿ��������

            EffectPool.CreatePrefabPool(prefabPool);
        }

        return EffectPool.Spawn(effectName);
    }

    public void DestroyEffect(Transform effect, float delay)
    {
        Mono.StartCoroutine(DestroyEffectCoroutine(effect, delay));
    }

    private IEnumerator DestroyEffectCoroutine(Transform effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        EffectPool.Despawn(effect);
    }

    public void Clear()
    {
        //EffectPool.Clear();
        
        EffectPool = null;
    }

}
