using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransPointWorldMapData 
{
    public Vector3 TransPointPos;
    public float RotationY;
    public int TransPointId;
    public int TargetSceneId;
    public int TargetSceneTransPointId;

    public TransPointWorldMapData(float x, float y, float z, float rotationY, int transPointId, int targetSceneId, int targetSceneTransPointId)
    {
        TransPointPos = new Vector3(x,y,z);
        RotationY = rotationY;
        TransPointId = transPointId;
        TargetSceneId = targetSceneId;
        TargetSceneTransPointId = targetSceneTransPointId;
    }
}
