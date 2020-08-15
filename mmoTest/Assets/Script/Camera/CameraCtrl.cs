using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraCtrl : MonoBehaviour 
{
    public static CameraCtrl Instance;

    public Transform CameraUpAndDown;
    public Transform CameraZoomContainer;
    public Transform CameraContainer;

    private float RotateSpeed = 80f;
    private float ZoomSpeed = 20f;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 初始化 在主角出生后 摄像机要设置好相对于主角的位置
    /// </summary>
    public void Init()
    {
        CameraUpAndDown.localEulerAngles = new Vector3(Mathf.Clamp(CameraUpAndDown.localEulerAngles.x, 35f, 80f), 0, 0);
        CameraContainer.localPosition = new Vector3(0, 0, -5f);
    }

    //绕Y轴旋转 0左 1右
    public void SetCameraRotate(int type)
    {
        transform.Rotate(0, RotateSpeed * Time.deltaTime * (type == 0 ? -1 : 1), 0);
    }

    //绕X轴旋转 0上 1下
    public void SetCameraUpAndDown(int type)
    {
        CameraUpAndDown.Rotate(RotateSpeed * Time.deltaTime * (type == 1 ? -1 : 1), 0, 0);
        CameraUpAndDown.localEulerAngles = new Vector3(Mathf.Clamp(CameraUpAndDown.localEulerAngles.x, 10f, 90f), 0, 0);
    }

    //缩放 0拉近 1拉远
    public void SetCameraZoom(int type)
    {
        CameraContainer.Translate(Vector3.forward * Time.deltaTime * ZoomSpeed * (type == 0 ? 1 : -1));
        CameraContainer.localPosition = new Vector3(0, 0, Mathf.Clamp(CameraContainer.localPosition.z, -5f, 5f));
    }

    public void AutoLookAt(Vector3 pos)
    {
        CameraZoomContainer.LookAt(pos);
    }

    public void CameraShake(float delay = 0, float duration = 0.2f, float strength = 1, int vibrato = 10)
    {
        StartCoroutine(DOCameraShake(delay, duration, strength, vibrato));
    }

    private IEnumerator DOCameraShake(float delay = 0, float duration = 0.2f, float strength = 1, int vibrato = 10)
    {
        yield return new WaitForSeconds(delay);
        //Camera.main.transform.DOShakePosition(duration, strength, vibrato);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 14f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 12f);
    }
}
