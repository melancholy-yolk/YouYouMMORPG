using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonsterAI : MonoBehaviour 
{
    public GameObject mainPlayer;
    public GameObject monster;
	
	void Start () 
    {
        for (int i = 0; i < 20; i++)
        {
            

            Transform trans = Instantiate<GameObject>(monster).transform;
            trans.position = GetRandomPos(mainPlayer.transform.position, 3f);
            trans.LookAt(mainPlayer.transform);
        }
	}
	
	
	void Update () 
    {
		
	}

    public Vector3 GetRandomPos(Vector3 targetPos, float distance)
    {
        Vector3 v = Vector3.forward;
        v = Quaternion.Euler(0, Random.Range(0, 360f), 0) * v;
        Vector3 pos = v * distance * Random.Range(0.8f, 1f);
        Vector3 newPos = targetPos + pos;
        return newPos;
    }

}
