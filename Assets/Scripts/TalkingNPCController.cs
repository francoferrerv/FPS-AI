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
    private Vector3 chairEulerAngles;
    private Vector3 initialNPCEulerAngles;
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
                chairEulerAngles = chair.transform.rotation.eulerAngles;
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
            initialNPCEulerAngles = transform.rotation.eulerAngles;
            interpolationRatio = 0f;
            animator.SetTrigger("IsTurningAroundChair");

            return ChairState.TurningBackToChair;
        }

        return ChairState.WalkingToChair;
    }

    protected bool stillTurning()
    {
        return interpolationRatio < 1.0f;
    }

    protected ChairState turnBackToChair()
    {
        float fromY = initialNPCEulerAngles.y;
        float to = chairEulerAngles.y;

        interpolationRatio += Time.deltaTime / 3;

        float x = initialNPCEulerAngles.x;
        float y = Mathf.Lerp(fromY, to, interpolationRatio);
        float z = initialNPCEulerAngles.z;

        transform.rotation = Quaternion.Euler(x, y, z);

        return stillTurning() ? ChairState.TurningBackToChair : ChairState.StandingToSit;
    }

    protected ChairState standToSit()
    {
        animator.SetTrigger("IsAboutToSit");

        return ChairState.SittingDown;
    }

    protected ChairState sittingDown()
    {
        bool isSittingDown = animator.GetBool("IsAboutToSit");

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
        bool isStandingUp = animator.GetBool("IsStandingUp");

        if (isStandingUp)
        {
            return ChairState.StandingUp;
        }

        return ChairState.Idle;
    }
}
