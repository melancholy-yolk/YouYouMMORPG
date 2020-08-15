using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private IUserInput pi;//交互输入接口
    public float horizontalSpeed = 100.0f;//摄像机水平旋转速度
    public float verticalSpeed = 80.0f;//摄像机垂直旋转速度
    public float cameraDampValue = 0.1f;//插值平滑参数

    private GameObject playerHandle;//控制摄像机水平左右旋转
    private GameObject cameraHandle;//控制摄像机上下旋转
    private float tempEulerX;//摄像机x轴初始旋转
    private GameObject model;//角色模型
    private GameObject cameraGO;//摄像机
    private Vector3 cameraDampVelocity;//缓冲计算临时变量

    void Start()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20.0f;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        IUserInput[] inputs = playerHandle.GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                pi = input;
                break;
            }
        }
        cameraGO = Camera.main.gameObject;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
            Vector3 tempModelEuler = model.transform.eulerAngles;//记录模型初始朝向位置

            //摄像机绕Y轴旋转
            playerHandle.transform.Rotate(Vector3.up, pi.JRight * horizontalSpeed * Time.fixedDeltaTime);
            //直接旋转会有同位角问题
            // cameraHandle.transform.Rotate(Vector3.right, pi.JUp * -verticalSpeed * Time.deltaTime);
            // cameraHandle.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraHandle.transform.eulerAngles.x, -40, 30),
            // 0, 0);
            tempEulerX -= pi.JUp * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;//模型朝向保持不变

        
            //摄像机位置缓动跟随
            cameraGO.transform.position = Vector3.SmoothDamp(cameraGO.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue);
            cameraGO.transform.LookAt(cameraHandle.transform);
        
    }

}
