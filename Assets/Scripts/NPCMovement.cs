using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{

    private NavMeshAgent agent;
    private NPCHealth health;
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
                }
            }
        }
    }
}
