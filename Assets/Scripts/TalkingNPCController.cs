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
    Sitting,
    StandingUp
}

public class NPCMovement : BaseNPCController
{
    private Vector3 chairPosition;
    private Quaternion chairRotation;
    private Quaternion rotatingFrom;
    private float interpolationRatio;
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
                chairPosition = chair.transform.position + chair.transform.forward * 0.4f;
                chairRotation = chair.transform.rotation;
                chairState = startWalkingToChair();
            }
            else if (chairState == ChairState.Sitting)
            {
                chairState = standUp();
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
            case ChairState.StandingUp:
            {
                chairState = standingUp();
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

        if (reachedDestination())
        {
            rotatingFrom = transform.rotation;
            interpolationRatio = 0f;
            return ChairState.TurningBackToChair;
        }

        return ChairState.WalkingToChair;
    }

    protected bool stillTurning(Quaternion from, Quaternion to)
    {
        return interpolationRatio < 1.0f;
    }

    protected ChairState turnBackToChair()
    {
        Quaternion from = rotatingFrom;
        Quaternion to = chairRotation;

        interpolationRatio += Time.deltaTime;
        transform.rotation = Quaternion.Slerp(from, to, interpolationRatio);

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

    protected ChairState standUp()
    {
        animator.SetTrigger("IsStandingUp");

        return ChairState.StandingUp;
    }

    protected ChairState standingUp()
    {
        bool isStandingUp = this.animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Stand_Up");

        if (isStandingUp)
        {
            return ChairState.StandingUp;
        }

        return ChairState.Idle;
    }
}
