using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        //·¢ÏÖplayerÇÐ»»µ½chase×´Ì¬
        if(currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(E_NPCState.Chase);
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
        currentEnemy.anim.SetBool("walk", false) ;
    }
}
