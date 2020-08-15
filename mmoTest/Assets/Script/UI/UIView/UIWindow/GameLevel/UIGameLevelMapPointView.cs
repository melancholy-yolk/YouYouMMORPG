using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameLevelMapPointView : MonoBehaviour 
{
    [SerializeField]
    private Image imagePass;
    [SerializeField]
    private Image imageNormal;
	
    public void SetUI(bool isPass)
    {
        if (isPass)
        {
            imagePass.gameObject.SetActive(true);
        }
        else
        {
            imageNormal.gameObject.SetActive(true);
        }
    }

	void Start ()
    {
		
	}
}
