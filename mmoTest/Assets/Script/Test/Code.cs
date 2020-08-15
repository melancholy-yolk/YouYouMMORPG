using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code : MonoBehaviour
{
    int signalCount = 0;
    int currentNum = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EmitSignal();
        }
    }

    private void OnSignal()
    {
        Debug.Log("signal");
    }

    public void EmitSignal()
    {
        signalCount++;
        
        if (signalCount == 1)
        {
            currentNum += 5;
            StartCoroutine(Signal());
        }
        else
        {
            currentNum = 5;
        }
    }

    IEnumerator Signal()
    {
        yield return new WaitForSeconds(1.0f);
        OnSignal();
        currentNum--;
        if (currentNum > 0)
        {
            StartCoroutine(Signal());
        }
        else
        {
            signalCount = 0;
        }
    }
}
