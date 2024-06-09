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
    TurningAroundChair,
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

    protected override void Update()
    {
        base.Update();

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
            case ChairState.TurningAroundChair:
            {
                chairState = turnAroundChair();
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
        GameObject chair = Chair.getRandomChair();

        chairPosition = chair.transform.position + chair.transform.forward * 0.4f;
        chairEulerAngles = chair.transform.rotation.eulerAngles;
        agent.SetDestination(chairPosition);

        return ChairState.WalkingToChair;
    }

    protected ChairState walkToChair()
    {
        if (reachedDestination())
        {
            initialNPCEulerAngles = transform.rotation.eulerAngles;
            interpolationRatio = 0f;
            animator.SetTrigger("isTurningAroundChair");

            return ChairState.TurningAroundChair;
        }

        return ChairState.WalkingToChair;
    }

    protected bool stillTurning()
    {
        return interpolationRatio < 1.0f;
    }

    protected ChairState turnAroundChair()
    {
        float fromY = initialNPCEulerAngles.y;
        float toY = chairEulerAngles.y;

        interpolationRatio += 10.0f / Mathf.Abs(toY-fromY);

        float x = initialNPCEulerAngles.x;
        float y = Mathf.Lerp(fromY, toY, interpolationRatio);
        float z = initialNPCEulerAngles.z;

        transform.rotation = Quaternion.Euler(x, y, z);

        if (stillTurning())
        {
            return ChairState.TurningAroundChair;
        }

        animator.SetTrigger("IsSittingDown");

        return ChairState.SittingDown;
    }

    protected ChairState sittingDown()
    {
        bool isSittingDown = animator.GetBool("IsSittingDown");

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
