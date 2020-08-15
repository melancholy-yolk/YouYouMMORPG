using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWorldMapData 
{
    public int NPCId { get; set; }//编号
    public Vector3 NPCPosition { get; set; }//位置
    public float NPCRotationY { get; set; }//Y轴旋转角度
    public string NPCPrologue { get; set; }//开场白
}
