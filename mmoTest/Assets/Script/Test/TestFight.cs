using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestFight : MonoBehaviour 
{
    public RoleCtrl TestRole;
    public RoleCtrl TestEnemy;
	
	void Start () 
    {
        
	}
	
	
	void Update () 
    {
        TestEnemy.transform.LookAt(TestRole.transform);

        if (Input.GetKeyDown(KeyCode.S))
        {
            Camera.main.transform.DOShakePosition(0.2f, 1, 100);
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(TestRole.transform.position, TestRole.ViewRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TestRole.transform.position, TestRole.CurrAttackInfo.AttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(TestRole.transform.position, TestRole.PatrolRange);
    }

    void OnGUI()
    {
        if (TestRole == null)
        {
            return;
        }

        int posY = 0;

        if (GUI.Button(new Rect(1, posY, 100, 30), "normal idle"))
        {
            TestRole.ToIdle();
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "fight idle"))
        {
            TestRole.ToIdle(RoleIdleState.IdleFight);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "run"))
        {
            TestRole.ToRun();
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "hurt"))
        {
            //TestRole.ToHurt(100, 0);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "die"))
        {
            TestRole.ToDie();
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "win"))
        {
            TestRole.ToSelect();
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "attack1"))
        {
            TestAttack(RoleAttackType.PhyAttack, 1);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "attack2"))
        {
            TestAttack(RoleAttackType.PhyAttack, 2);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "attack3"))
        {
            TestAttack(RoleAttackType.PhyAttack, 3);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill1"))
        {
            TestAttack(RoleAttackType.SkillAttack, 1);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill2"))
        {
            TestAttack(RoleAttackType.SkillAttack, 2);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill3"))
        {
            TestAttack(RoleAttackType.SkillAttack, 3);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill4"))
        {
            TestAttack(RoleAttackType.SkillAttack, 4);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill5"))
        {
            TestAttack(RoleAttackType.SkillAttack, 5);
        }

        posY += 30;
        if (GUI.Button(new Rect(1, posY, 100, 30), "skill6"))
        {
            TestAttack(RoleAttackType.SkillAttack, 6);
        }
        
    }

    private void TestAttack(RoleAttackType type, int index)
    {
        
        
        TestRole.ToAttackByIndex(type, index);
        if (TestEnemy != null)
        {
            TestEnemy.transform.position = Vector3.zero + new Vector3(0, 0, TestRole.CurrAttackInfo.AttackRange);
            //TestEnemy.ToHurt(0, TestRole.CurrAttackInfo.HurtDelay);
        }

        if (TestRole.CurrAttackInfo.IsCameraShake)
        {
            StartCoroutine(DOCameraShake(TestRole.CurrAttackInfo.CameraShakeDelay, 
                TestRole.CurrAttackInfo.CameraShakeDuration, 
                TestRole.CurrAttackInfo.CameraShakeStrength, 
                TestRole.CurrAttackInfo.CameraShakeVibrto));
        }
    }

    private IEnumerator DOCameraShake(float delay=0, float duration=0.2f, float strength=1, int vibrato=10)
    {
        yield return new WaitForSeconds(delay);
        Camera.main.transform.DOShakePosition(duration, strength, vibrato);
    }

}
