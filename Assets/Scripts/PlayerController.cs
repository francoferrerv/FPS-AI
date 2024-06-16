using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class PlayerController: BaseController
{
    protected internal ThirdPersonController thirdPersonController;
    public bool paused { get; private set; }

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

        if (paused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (state == State.Idle)
            {
                agent.enabled = true;
                state = sitOnClosestSeat();

                if (state == State.Idle)
                {
                    agent.enabled = false;
                }
                else
                {
                    thirdPersonController.enabled = false;
                }
            }
            else if (state == State.Sitting)
            {
                thirdPersonController.enabled = true;
                agent.enabled = false;
                state = standUp();
            }
        }

        base.Update();
    }

    protected void Pause()
    {
        paused = !paused;
        thirdPersonController.enabled = !paused;
    }
}
