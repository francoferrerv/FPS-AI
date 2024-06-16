using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TalkingNPCController: BaseController
{
    public AudioClip[] FootstepAudioClips;
    public GameObject player;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private CharacterController controller;
    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
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
            if (state == NPCState.Idle)
            {
                state = sitOnClosestSeat();
            }
            else if (state == NPCState.Sitting)
            {
                state = standUp();
            }
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }
}
