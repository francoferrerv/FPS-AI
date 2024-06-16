using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Idle,
    WalkingToSeat,
    TurningAroundSeat,
    Sitting
}

public class BaseController : MonoBehaviour
{
    protected internal NavMeshAgent agent;
    protected internal Animator animator;
    private Seat seat;
    private Vector3 initialEulerAngles;
    private float interpolationRatio;
    protected State state = State.Idle;
    protected float speed;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        animator.SetFloat("MotionSpeed", 1.0f);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateState();
    }

    protected void onReachedDestination()
    {
        animator.SetFloat("Speed", 0.0f);
    }

    protected void sit()
    {
        if (seat != null)
        {
            walkTo(seat.position);
            state = State.WalkingToSeat;

            return;
        }

        state = State.Idle;
    }

    protected void sitOnClosestSeat()
    {
        seat = Seat.getClosestSeat(agent.transform.position);
        sit();
    }

    protected void sitOnRandomSeat()
    {
        seat = Seat.getRandomSeat();
        sit();
    }

    protected void standUp()
    {
        animator.SetTrigger("IsStandingUp");
        seat.status.SetAvailability(seat.placeName, true);
        state = State.Idle;
    }

    protected void walkTo(Vector3 targetPosition)
    {
        const float walkingSpeed = 2.0f;

        agent.speed = walkingSpeed;
        agent.SetDestination(targetPosition);
        animator.SetFloat("Speed", walkingSpeed);
    }

    private void UpdateState()
    {
        switch (state)
        {
            case State.WalkingToSeat:
            {
                walkToSeat();
                break;
            }
            case State.TurningAroundSeat:
            {
                turnAroundSeat();
                break;
            }
        }
    }

    private void walkToSeat()
    {
        if (reachedDestination())
        {
            initialEulerAngles = transform.rotation.eulerAngles;
            interpolationRatio = 0f;
            onReachedDestination();
            animator.SetTrigger("isTurningAroundSeat");
            state = State.TurningAroundSeat;

            return;
        }

        state = State.WalkingToSeat;
    }

    private bool stillTurning()
    {
        return interpolationRatio < 1.0f;
    }

    private void turnAroundSeat()
    {
        float fromY = initialEulerAngles.y;
        float toY = seat.status.GetEulerAngles().y;

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
            state = State.TurningAroundSeat;

            return;
        }

        animator.SetTrigger("IsSittingDown");
        state = State.Sitting;
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
