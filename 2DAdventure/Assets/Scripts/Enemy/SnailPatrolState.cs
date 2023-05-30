using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        //����player�л���chase״̬
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(E_NPCState.Skill);
        }

        if (!currentEnemy.physicscheck.isGround || currentEnemy.physicscheck.touchLeftWall && currentEnemy.faceDir.x < 0 || currentEnemy.physicscheck.touchRightWall && currentEnemy.faceDir.x > 0)
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
    }
}
