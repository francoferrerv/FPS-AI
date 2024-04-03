using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class NPCMovement : MonoBehaviour
{

    private NavMeshAgent agent;
    private NPCHealth health;
    public float damageDistance;
    [SerializeField]
    private Animator animator;
    private int damagePerHit = 1;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        health = this.GetComponent<NPCHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!health.isDead)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0) {
                if (players[0] != null) // Check if player reference is not null
                {
                    agent.SetDestination(players[0].transform.position); // Set NPC's destination to player's position
                    if (Vector3.Distance(transform.position, players[0].transform.position) < damageDistance)
                    {
                        players[0].GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damagePerHit, "NPC");

                    }
                }
            }
        }
        else
        {
            agent.SetDestination(transform.position);
        }
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
