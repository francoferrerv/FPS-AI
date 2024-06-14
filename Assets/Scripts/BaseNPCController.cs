using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState
{
    Idle,
    WalkingToSeat,
    TurningAroundSeat,
    SittingDown,
    Sitting,
    StandingUp
}

public class BaseNPCController : MonoBehaviour
{
    protected internal NavMeshAgent agent;
    protected internal Animator animator;
    private Vector3 seatEulerAngles;
    private Vector3 initialEulerAngles;
    private float interpolationRatio;
    protected NPCState state = NPCState.Idle;
    protected float speed;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateState();
    }

    protected void onReachedDestination()
    {
        animator.SetFloat("MotionSpeed", 0.0f);
    }

    protected NPCState sitOn(GameObject seat)
    {
        Vector3 targetPosition = seat.transform.position + seat.transform.forward * 1.0f;

        seatEulerAngles = seat.transform.rotation.eulerAngles;
        walkTo(targetPosition);

        return NPCState.WalkingToSeat;
    }

    protected NPCState sitOnClosestSeat()
    {
        GameObject seat = Seat.getClosestSeat(agent.transform.position);

        sitOn(seat);

        return NPCState.WalkingToSeat;
    }

    protected NPCState sitOnRandomSeat()
    {
        GameObject seat = Seat.getRandomSeat();

        sitOn(seat);

        return NPCState.WalkingToSeat;
    }

    protected NPCState standUp()
    {
        animator.SetTrigger("IsStandingUp");

        return NPCState.StandingUp;
    }

    protected void walkTo(Vector3 targetPosition)
    {
        const float walkingSpeed = 2.0f;

        agent.speed = walkingSpeed;
        agent.SetDestination(targetPosition);
        animator.SetFloat("Speed", walkingSpeed);
        animator.SetFloat("MotionSpeed", 1.0f);
    }

    private void UpdateState()
    {
        switch (state)
        {
            case NPCState.WalkingToSeat:
            {
                state = walkToSeat();
                break;
            }
            case NPCState.TurningAroundSeat:
            {
                state = turnAroundSeat();
                break;
            }
            case NPCState.SittingDown:
            {
                state = sittingDown();
                break;
            }
            case NPCState.StandingUp:
            {
                state = standingUp();
                break;
            }
        }

        Debug.Log(state);
    }

    private NPCState walkToSeat()
    {
        if (reachedDestination())
        {
            initialEulerAngles = transform.rotation.eulerAngles;
            interpolationRatio = 0f;
            onReachedDestination();
            animator.SetTrigger("isTurningAroundSeat");

            return NPCState.TurningAroundSeat;
        }

        return NPCState.WalkingToSeat;
    }

    private bool stillTurning()
    {
        return interpolationRatio < 1.0f;
    }

    private NPCState turnAroundSeat()
    {
        float fromY = initialEulerAngles.y;
        float toY = seatEulerAngles.y;

        if (Mathf.Abs(toY - fromY) > 180)
        {
            toY += fromY < toY ? -360 : 360;
        }

        interpolationRatio += 20.0f / Mathf.Abs(toY-fromY);

        float x = initialEulerAngles.x;
        float y = Mathf.Lerp(fromY, toY, interpolationRatio);
        float z = initialEulerAngles.z;

        transform.rotation = Quaternion.Euler(x, y, z);

        if (stillTurning())
        {
            return NPCState.TurningAroundSeat;
        }

        animator.SetTrigger("IsSittingDown");

        return NPCState.SittingDown;
    }

    private NPCState sittingDown()
    {
        bool isSittingDown = animator.GetBool("IsSittingDown");

        if (isSittingDown)
        {
            return NPCState.SittingDown;
        }

        animator.SetTrigger("IsSitting");

        return NPCState.Sitting;
    }

    private NPCState standingUp()
    {
        bool isStandingUp = animator.GetBool("IsStandingUp");

        if (isStandingUp)
        {
            return NPCState.StandingUp;
        }

        return NPCState.Idle;
    }

    private bool reachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
