using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private IUserInput pi;//��������ӿ�
    public float horizontalSpeed = 100.0f;//�����ˮƽ��ת�ٶ�
    public float verticalSpeed = 80.0f;//�������ֱ��ת�ٶ�
    public float cameraDampValue = 0.1f;//��ֵƽ������

    private GameObject playerHandle;//���������ˮƽ������ת
    private GameObject cameraHandle;//���������������ת
    private float tempEulerX;//�����x���ʼ��ת
    private GameObject model;//��ɫģ��
    private GameObject cameraGO;//�����
    private Vector3 cameraDampVelocity;//���������ʱ����

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
            Vector3 tempModelEuler = model.transform.eulerAngles;//��¼ģ�ͳ�ʼ����λ��

            //�������Y����ת
            playerHandle.transform.Rotate(Vector3.up, pi.JRight * horizontalSpeed * Time.fixedDeltaTime);
            //ֱ����ת����ͬλ������
            // cameraHandle.transform.Rotate(Vector3.right, pi.JUp * -verticalSpeed * Time.deltaTime);
            // cameraHandle.transform.eulerAngles = new Vector3(Mathf.Clamp(cameraHandle.transform.eulerAngles.x, -40, 30),
            // 0, 0);
            tempEulerX -= pi.JUp * verticalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            model.transform.eulerAngles = tempModelEuler;//ģ�ͳ��򱣳ֲ���

        
            //�����λ�û�������
            cameraGO.transform.position = Vector3.SmoothDamp(cameraGO.transform.position, transform.position, ref cameraDampVelocity, cameraDampValue);
            cameraGO.transform.LookAt(cameraHandle.transform);
        
    }

}
