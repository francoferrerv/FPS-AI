using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMeganController: TalkingNPCController
{
    public GameObject player;
    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        playerController = player.GetComponent<PlayerController>();
        sitOnClosestSeat();
    }

    protected override void Update()
    {
        if (playerController.paused)
        {
            return;
        }

        base.Update();
    }
}
