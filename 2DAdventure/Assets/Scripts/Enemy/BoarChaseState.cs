using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        //Debug.Log("Chase");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }

    public override void LogicUpdate()
    {
        if(currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(E_NPCState.Patrol);
        }
        if (!currentEnemy.physicscheck.isGround || currentEnemy.physicscheck.touchLeftWall && currentEnemy.faceDir.x < 0 || currentEnemy.physicscheck.touchRightWall && currentEnemy.faceDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }
    }
    public override void PhysicsUpdate()
    {
        
    }


    public override void OnExit()
    {
        currentEnemy.anim.SetBool("run", false);
    }

}
