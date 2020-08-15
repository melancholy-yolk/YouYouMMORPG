using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfoMonster : RoleInfoBase 
{
    public int Level;
    public SpriteEntity spriteEntity;

    public RoleInfoMonster() : base()
    {

    }

    public RoleInfoMonster(SpriteEntity entity)
    {
        spriteEntity = entity;

        RoleNickName = entity.Name;
        Level = entity.Level;

        MaxHP = entity.HP;
        MaxMP = entity.MP;

        CurrHP = entity.HP;
        CurrMP = entity.MP;

        Attack = entity.Attack;
        Defense = entity.Defense;
        Hit = entity.Hit;
        Dodge = entity.Dodge;
        Cri = entity.Cri;
        Res = entity.Res;

        Fighting = entity.Fighting;
    }

}
