using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleHeadBarRoot : MonoBehaviour 
{
    public static RoleHeadBarRoot Instance;

    void Awake()
    {
        Instance = this;
    }
}
