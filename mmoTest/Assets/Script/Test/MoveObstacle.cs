using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObstacle : MonoBehaviour 
{
    private bool flag;

	void Start () {
        transform.DOMove(transform.position + Vector3.up * 2, 1).SetAutoKill(false).Pause().OnComplete(() =>
        {
            flag = true;
            StartCoroutine(Delay());
            
        }).OnRewind(() => {
            flag = false;
            StartCoroutine(Delay());
            
        });
        transform.DOPlayForward();
	}

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3);
        if (flag)
        {
            transform.DOPlayBackwards();
        }
        else
        {
            transform.DOPlayForward();
        }
    }
}
