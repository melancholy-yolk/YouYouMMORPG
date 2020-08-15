using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSceneCtrlBase : MonoBehaviour 
{
    protected UISceneMainCityView m_UISceneMainCityView;

    void Awake()
    {
        if (FingerEventCenter.Instance != null)
        {
            FingerEventCenter.Instance.OnFingerDrag += OnFingerDragCallBack;
            FingerEventCenter.Instance.OnPlayerClickGround += OnPlayerClickGroundCallBack;
            FingerEventCenter.Instance.OnZoom += OnZoomCallBack;
        }
        

        OnAwake();
    }
	
	void Start () 
    {
        //加载场景UI
        m_UISceneMainCityView = SceneUIManager.Instance.LoadSceneUI(SceneUIType.MainCity, OnUISceneMainCityViewLoadComplete).GetComponent<UISceneMainCityView>();

        EffectMgr.Instance.Init(this);//初始化特效对象池

        OnStart();
	}
	
	
	void Update () 
    {
        OnUpdate();
	}

    void OnDestroy()
    {
        if (FingerEventCenter.Instance != null)
        {
            FingerEventCenter.Instance.OnFingerDrag -= OnFingerDragCallBack;
            FingerEventCenter.Instance.OnPlayerClickGround -= OnPlayerClickGroundCallBack;
            FingerEventCenter.Instance.OnZoom -= OnZoomCallBack;
        }

        EffectMgr.Instance.Clear();
        BeforeOnDestroy();
    }

    private void OnPlayerClickGroundCallBack()
    {
        if (SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
        {
            if (GlobalInit.Instance.MainPlayer.Attack.IsAutoFight)
            {
                return;
            }
        }

        //防止UI穿透
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        WorldMapController.Instance.IsAutoMove = false;//玩家点击地面 取消自动寻路

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //点击角色
        RaycastHit[] hitArr = Physics.RaycastAll(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Role"));
        if (hitArr.Length > 0)
        {
            
            RoleCtrl hitRole = hitArr[0].collider.gameObject.GetComponent<RoleCtrl>();
            Debug.Log(hitArr[0].collider.gameObject.name);
            if (hitRole.CurrRoleType == RoleType.Monster)
            {
                GlobalInit.Instance.MainPlayer.LockEnemy = hitRole;
                return;
            }
            else if (hitRole.CurrRoleType == RoleType.OtherPlayer)
            {
                //如果点击到了其他玩家
                //需要判断其他玩家是敌是友

                //判断当前所在场景是否可以进行战斗 例如主城中不可以战斗
                if (SceneMgr.Instance.IsFightingScene)
                {
                    //实际的开发中需要根据具体逻辑 来辨别是敌是友
                    GlobalInit.Instance.MainPlayer.LockEnemy = hitRole;
                    return;
                }
            }
        }

        //点击地面
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Road")))
        {
            if (GlobalInit.Instance.MainPlayer != null)
            {
                //游戏关卡中 只有点击当前区域才有效
                if (SceneMgr.Instance.CurrentSceneType == SceneType.GameLevel)
                {
                    Vector3 point = new Vector3(hit.point.x, hit.point.y + 50, hit.point.z);
                    if (Physics.Raycast(point, Vector3.down, 100, 1 << LayerMask.NameToLayer("RegionMask")))
                    {
                        Debug.Log("hit invalid region!!!");
                        return;
                    }
                }

                GlobalInit.Instance.MainPlayer.LockEnemy = null;
                GlobalInit.Instance.MainPlayer.MoveTo(hit.point);
            }
        }

    }

    #region 摄像机交互
    private void OnZoomCallBack(FingerEventCenter.ZoomType obj)
    {
        //防止UI穿透
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (CameraCtrl.Instance == null) return;
        switch (obj)
        {
            case FingerEventCenter.ZoomType.In:
                CameraCtrl.Instance.SetCameraZoom(1);
                break;
            case FingerEventCenter.ZoomType.Out:
                CameraCtrl.Instance.SetCameraZoom(0);
                break;
        }
    }

    private void OnFingerDragCallBack(FingerEventCenter.FingerDragDir obj)
    {
        //防止UI穿透
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (CameraCtrl.Instance == null) return;
        switch (obj)
        {
            case FingerEventCenter.FingerDragDir.Left:
                CameraCtrl.Instance.SetCameraRotate(0);
                break;
            case FingerEventCenter.FingerDragDir.Rigth:
                CameraCtrl.Instance.SetCameraRotate(1);
                break;
            case FingerEventCenter.FingerDragDir.Up:
                CameraCtrl.Instance.SetCameraUpAndDown(1);
                break;
            case FingerEventCenter.FingerDragDir.Down:
                CameraCtrl.Instance.SetCameraUpAndDown(0);
                break;
        }
    }
    #endregion

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void BeforeOnDestroy() { }

    protected virtual void OnUISceneMainCityViewLoadComplete() { }
}
