using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Camera : MonoBehaviour {
    [HideInInspector]
    public Camera m_Camera;
    public static UI_Camera Instance;

    void Awake()
    {
        Instance = this;
    }

	void Start () {
		m_Camera = GetComponent<Camera>();
	}
	
	
	
}
