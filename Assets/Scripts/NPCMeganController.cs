using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMeganController: NPCController
{
    public GameObject player;
    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        playerController = player.GetComponent<PlayerController>();
    }

    protected override void Update()
    {
        if (playerController.paused)
        {
            return;
        }

        base.Update();
        moveChairTest();
    }

    protected void moveChairTest()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C");
            if (state == State.Idle)
            {
                sitOnClosestSeat();
            }
            else if (state == State.Sitting)
            {
                standUp();
            }
        }
    }
}
