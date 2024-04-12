using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class EnemyNPCMovement : BaseNPCController
{

    protected override void Move()
    {
        if (!health.isDead)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
            {
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
        else if (agent.hasPath) // Check if the agent has an active path
        {
            agent.ResetPath(); // Cancel the agent's current path
        }
    }
}
