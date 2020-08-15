using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoleAI
{
    RoleCtrl CurrRole
    {
        get;
        set;
    }

    void DoAI();
}
