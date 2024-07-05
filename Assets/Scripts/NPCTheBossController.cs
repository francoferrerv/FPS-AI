using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class NPCTheBossController: TalkingNPCController
{
    public GameObject player;
    private PlayerController playerController;
    private CinemachineVirtualCamera camera;

    protected override void Start()
    {
        base.Start();
        playerController = player.GetComponent<PlayerController>();
        GameObject gameObject = GameObject.Find("MeganVirtualCamera");
        camera = gameObject.GetComponent<CinemachineVirtualCamera>();
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
