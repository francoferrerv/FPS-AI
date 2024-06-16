using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TalkingNPCController: BaseController
{
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    private CharacterController controller;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
    }

    protected override void Update()
    {
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
