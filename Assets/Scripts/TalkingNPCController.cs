using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public enum ChairState
{
    Idle,
    WalkingToChair,
    TurningBackToChair,
    StandingToSit,
    SittingDown,
    Sitting
}

public class NPCMovement : BaseNPCController
{
    private Vector3 chairPosition;
    private Quaternion chairRotation;
    private ChairState chairState = ChairState.Idle;

    protected override void Move()
    {
        if (!NetworkManager.normalGameStart)
        {
            moveChairTest();
        }
    }

    protected void moveChairTest()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (chairState == ChairState.Idle)
            {
                GameObject chair = Chair.getRandomChair();
                chairPosition = chair.transform.position + chair.transform.forward * 0.5f;
                chairRotation = chair.transform.rotation;
                chairState = startWalkingToChair();
            }
        }

        switch (chairState)
        {
            case ChairState.WalkingToChair:
            {
                chairState = walkToChair();
                break;
            }
            case ChairState.TurningBackToChair:
            {
                chairState = turnBackToChair();
                break;
            }
            case ChairState.StandingToSit:
            {
                chairState = standToSit();
                break;
            }
            case ChairState.SittingDown:
            {
                chairState = sittingDown();
                break;
            }
        }
    }

    protected ChairState startWalkingToChair()
    {
        agent.SetDestination(chairPosition);

        return ChairState.WalkingToChair;
    }

    protected ChairState walkToChair()
    {
        agent.SetDestination(chairPosition);

        return reachedDestination() ? ChairState.TurningBackToChair : ChairState.WalkingToChair;
    }

    protected bool stillTurning(Quaternion from, Quaternion to)
    {
        const double delta = 1.0;
        Quaternion diff = Quaternion.Inverse(from) * to;

        return diff.x > delta || diff.y > delta || diff.z > delta;
    }

    protected ChairState turnBackToChair()
    {
        Quaternion from = transform.rotation;
        Quaternion to = chairRotation;

        transform.rotation = Quaternion.Slerp(from, to, Time.deltaTime * 3f);

        return stillTurning(from, to) ? ChairState.TurningBackToChair : ChairState.StandingToSit;
    }

    protected ChairState standToSit()
    {
        animator.SetTrigger("IsAboutToSit");

        return ChairState.SittingDown;
    }

    protected ChairState sittingDown()
    {
        bool isSittingDown = this.animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Stand_To_Sit");

        if (isSittingDown)
        {
            return ChairState.SittingDown;
        }

        animator.SetTrigger("IsSitting");

        return ChairState.Sitting;
    }
}
