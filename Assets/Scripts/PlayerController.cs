using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class PlayerController: BaseController
{
    protected internal ThirdPersonController thirdPersonController;
    protected bool paused = false;

    protected override void Start()
    {
        base.Start();
        thirdPersonController = this.GetComponent<ThirdPersonController>();
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }

        if (!thirdPersonController.enabled)
        {
            base.Update();
        }
    }

    protected void Pause()
    {
        paused = !paused;
        thirdPersonController.enabled = !paused;

        if (paused)
        {
            state = NPCState.Idle;
            animator.SetBool("Grounded", true);
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);
            animator.SetFloat("Speed", 0.0f);
        }
    }
}
