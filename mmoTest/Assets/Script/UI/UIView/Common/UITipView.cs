using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public class UITipView : MonoBehaviour 
{
    public static UITipView Instance;

    private Queue<TipEntity> TipQueue;
    private float PreviousTipTime = 0;

    private SpawnPool TipPool;
    private Transform TipItemPrefab;
	
    void Awake()
    {
        Instance = this;
        TipQueue = new Queue<TipEntity>();
    }
	
    void Start()
    {
        TipItemPrefab = ResourcesManager.Instance.Load(ResourcesManager.ResourceType.UIItem, "Common/UITipItem", returnClone:false).GetComponent<Transform>();

        TipPool = PoolManager.Pools.Create("TipPool");
        TipPool.group.parent = transform;
        TipPool.group.localPosition = Vector3.zero;

        PrefabPool prefabPool = new PrefabPool(TipItemPrefab);
        prefabPool.preloadAmount = 5;//Ԥ��������

        prefabPool.cullDespawned = true;//�Ƿ���������Զ�����
        prefabPool.cullAbove = 10;//�����ʼ�ձ��ֶ���
        prefabPool.cullDelay = 2;//�೤ʱ������һ��
        prefabPool.cullMaxPerPass = 2;//ÿ��������

        TipPool.CreatePrefabPool(prefabPool);
    }

	void Update () 
    {
        if (Time.time > PreviousTipTime + 0.5f)
        {
            PreviousTipTime = Time.time;
            if (TipQueue.Count > 0)
            {
                TipEntity entity = TipQueue.Dequeue();
                Transform trans = TipPool.Spawn("UITipItem");
                UITipItemView view = trans.GetComponent<UITipItemView>();
                view.SetUI(entity);
                trans.gameObject.SetParent(transform);
                trans.DOLocalMoveY(150, 0.5f).OnComplete(() =>
                {
                    TipPool.Despawn(trans);
                });
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowTip(1, "+999");
        }
	}

    public void ShowTip(int type, string content)
    {
        TipQueue.Enqueue(new TipEntity() { Type = type, Content = content});
    }
}
