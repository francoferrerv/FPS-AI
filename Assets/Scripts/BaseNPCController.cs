using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class BaseNPCController : MonoBehaviour
{

    protected internal NavMeshAgent agent;
    protected internal NPCHealth health;
    public float damageDistance;
    [SerializeField]
    protected internal Animator animator;
    protected internal int damagePerHit = 1;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        health = this.GetComponent<NPCHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        
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
}
