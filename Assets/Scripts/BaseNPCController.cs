using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;


public enum NPCState
{
    Idle,
    WalkingToChair,
    TurningAroundChair,
    SittingDown,
    Sitting,
    StandingUp
}

public class BaseNPCController : MonoBehaviour
{
    protected internal NavMeshAgent agent;
    protected internal NPCHealth health;
    public float damageDistance;
    [SerializeField]
    protected internal Animator animator;
    protected internal int damagePerHit = 1;

    private Vector3 chairPosition;
    private Vector3 chairEulerAngles;
    private Vector3 initialEulerAngles;
    private float interpolationRatio;
    protected NPCState state = NPCState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        health = this.GetComponent<NPCHealth>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateState();
    }

    void FixedUpdate()
    {
        animator.SetFloat("Horizontal", agent.velocity.x);
        animator.SetFloat("Vertical", agent.velocity.z);
        /*if (agent.velocity.y > 5f)
        {
            animator.SetTrigger("IsJumping");
        }*/
        //animator.SetBool("Running", Input.GetKey(KeyCode.LeftShift));
    }

    protected NPCState sitOnRandomChair()
    {
        GameObject chair = Chair.getRandomChair();

        chairPosition = chair.transform.position + chair.transform.forward * 0.4f;
        chairEulerAngles = chair.transform.rotation.eulerAngles;
        agent.SetDestination(chairPosition);

        return NPCState.WalkingToChair;
    }

    protected NPCState standUp()
    {
        animator.SetTrigger("IsStandingUp");

        return NPCState.StandingUp;
    }

    // Check if we've reached the destination
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

    private void UpdateState()
    {
        switch (state)
        {
            case NPCState.WalkingToChair:
            {
                state = walkToChair();
                break;
            }
            case NPCState.TurningAroundChair:
            {
                state = turnAroundChair();
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
    }

    private NPCState walkToChair()
    {
        if (reachedDestination())
        {
            initialEulerAngles = transform.rotation.eulerAngles;
            interpolationRatio = 0f;
            animator.SetTrigger("isTurningAroundChair");

            return NPCState.TurningAroundChair;
        }

        return NPCState.WalkingToChair;
    }

    private bool stillTurning()
    {
        return interpolationRatio < 1.0f;
    }

    private NPCState turnAroundChair()
    {
        float fromY = initialEulerAngles.y;
        float toY = chairEulerAngles.y;

        interpolationRatio += 10.0f / Mathf.Abs(toY-fromY);

        float x = initialEulerAngles.x;
        float y = Mathf.Lerp(fromY, toY, interpolationRatio);
        float z = initialEulerAngles.z;

        transform.rotation = Quaternion.Euler(x, y, z);

        if (stillTurning())
        {
            return NPCState.TurningAroundChair;
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
}
