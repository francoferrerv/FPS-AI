using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class PlayerController: BaseController
{
    public bool paused { get; private set; }

    protected override void Start()
    {
        base.Start();
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

        base.Update();
    }

    protected void Pause()
    {
        paused = !paused;
        thirdPersonController.enabled = !paused;
    }
}
