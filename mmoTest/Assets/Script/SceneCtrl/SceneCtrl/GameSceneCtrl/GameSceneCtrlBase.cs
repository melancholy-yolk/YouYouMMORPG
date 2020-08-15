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
        //���س���UI
        m_UISceneMainCityView = SceneUIManager.Instance.LoadSceneUI(SceneUIType.MainCity, OnUISceneMainCityViewLoadComplete).GetComponent<UISceneMainCityView>();

        EffectMgr.Instance.Init(this);//��ʼ����Ч�����

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

        //��ֹUI��͸
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        WorldMapController.Instance.IsAutoMove = false;//��ҵ������ ȡ���Զ�Ѱ·

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //�����ɫ
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
                //�����������������
                //��Ҫ�ж���������ǵ�����

                //�жϵ�ǰ���ڳ����Ƿ���Խ���ս�� ���������в�����ս��
                if (SceneMgr.Instance.IsFightingScene)
                {
                    //ʵ�ʵĿ�������Ҫ���ݾ����߼� ������ǵ�����
                    GlobalInit.Instance.MainPlayer.LockEnemy = hitRole;
                    return;
                }
            }
        }

        //�������
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("Road")))
        {
            if (GlobalInit.Instance.MainPlayer != null)
            {
                //��Ϸ�ؿ��� ֻ�е����ǰ�������Ч
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

    #region ���������
    private void OnZoomCallBack(FingerEventCenter.ZoomType obj)
    {
        //��ֹUI��͸
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
        //��ֹUI��͸
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
