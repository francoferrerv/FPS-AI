using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public enum ChairState
{
    Waiting,
    WalkingToChair,
    TurnBackToChair,
    Finished
}

public class NPCMovement : BaseNPCController
{
    private Vector3 chairPosition;
    private Quaternion chairRotation;
    private ChairState chairState = ChairState.Waiting;

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
            GameObject chair = Chair.getRandomChair();
            chairPosition = chair.transform.position + chair.transform.forward * 1.2f;
            chairRotation = chair.transform.rotation;
            chairState = startWalkingToChair();
        }

        switch (chairState)
        {
            case ChairState.WalkingToChair:
            {
                chairState = walkToChair();
                break;
            }
            case ChairState.TurnBackToChair:
            {
                chairState = turnBackToChair();
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

        return reachedDestination() ? ChairState.TurnBackToChair : ChairState.WalkingToChair;
    }

    protected ChairState turnBackToChair()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, chairRotation, Time.deltaTime * 3f);
        return ChairState.TurnBackToChair;
    }
}
