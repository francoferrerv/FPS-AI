using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class NPCMeganController: TalkingNPCController
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

    public override void TalkTo(ITalkingCharacter talkingCharacter)
    {
        base.TalkTo(talkingCharacter);
        camera.Priority = 11;
        camera.gameObject.SetActive(true);
    }

    public override void StopTalkingTo(ITalkingCharacter talkingCharacter)
    {
        base.StopTalkingTo(talkingCharacter);
        camera.Priority = 0;
        camera.gameObject.SetActive(false);
    }
}
