using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ɼ���ɫ����������Ϣ
/// </summary>
[System.Serializable]
public class RoleAttackInfo 
{
    public int Index;
    public int SkillId;

    public string EffectName;//��Ч���� �����Ĵ�ab���м�����Ч
#if DEBUG_ROLE_STATE
    public GameObject EffectObject;//����ʱ ֱ���ڱ༭������ק
#endif
    public float EffectLifeTime;//��Ч����ʱ��

    public float AttackRange;//������Χ
    public float HurtDelay;//�ܻ��������ӳ�

    public bool IsCameraShake = false;//�Ƿ�����
    public float CameraShakeDelay;
    public float CameraShakeDuration;
    public float CameraShakeStrength;
    public int CameraShakeVibrto;
}
