using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//键鼠输入模块管理
public class KeyboardInput : IUserInput
{
    [Header("==== key settings ====")]//按键
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyA = "left shift";//快跑按键
    public string keyB = "j";//后跳<0.1 翻滚<1.1 跳跃按键
    public string keyC = "k";
    public string keyD;

    public string KeyJUp;
    public string KeyJDown;
    public string KeyJLeft;
    public string KeyJRight;

    [Header("==== mouse setting ====")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;

    void Update()
    {
            JUp = Input.GetAxis("Mouse Y") * 3.0f * mouseSensitivityY;
            JRight = Input.GetAxis("Mouse X") * 2.5f * mouseSensitivityX;

        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);

        if (inputEnabled == false)
        {
            targetDup = 0;
            targetDright = 0;
        }

        //角色操控前后方向
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        //角色操控左右方向
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        //避免斜45°速度过快的bug 使用椭圆映射法
        Vector2 tempDirectionAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dup2 = tempDirectionAxis.y;
        float Dright2 = tempDirectionAxis.x;

        //模
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        //朝向向量
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        //疾跑信号
        run = Input.GetKey(KeyCode.LeftShift);

        attack = Input.GetMouseButtonDown(0);

        skill1 = Input.GetKey(KeyCode.Alpha1);
        skill2 = Input.GetKey(KeyCode.Alpha2);
        skill3 = Input.GetKey(KeyCode.Alpha3);
        skill4 = Input.GetKey(KeyCode.Alpha4);
        skill5 = Input.GetKey(KeyCode.Alpha5);
        skill6 = Input.GetKey(KeyCode.Alpha6);

        //跳跃信号
        //bool newJump = Input.GetKey(KeyCode.Space);
        //if (newJump != lastJump && newJump == true)
        //{
        //    jump = true;
        //    // print("jump trigger");
        //}
        //else
        //{
        //    jump = false;
        //}
        //lastJump = newJump;

        //攻击信号
        //bool newAttack = Input.GetKey(keyC);
        //if (newAttack != lastAttack && newAttack == true)
        //{
        //    rb = true;
        //}
        //else
        //{
        //    rb = false;
        //}
        //lastAttack = newAttack;
    }

    

}
