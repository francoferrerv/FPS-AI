using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class PlayerController: BaseController
{
    protected internal ThirdPersonController thirdPersonController;
    public bool paused { get; private set; }
    public Avatar[] avatars;
    public GameObject[] models;
    [Header("Images for The Boss")]
    public GameObject ImagePlane;
    protected int currentCharacter;
    protected int characterCount;

    protected override void Start()
    {
        base.Start();
        thirdPersonController = this.GetComponent<ThirdPersonController>();
        characterCount = avatars.Length;

        for (int i = 0; i < characterCount; i++)
        {
            GameObject model = models[i];

            if (model.activeSelf)
            {
                Debug.Log($"{model.name} is active");
                currentCharacter = i;
            }
        }
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            disableCurrentCharacter();
            currentCharacter--;

            if (currentCharacter < 0)
            {
                currentCharacter = characterCount - 1;
            }

            enableCurrentCharacter();
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            disableCurrentCharacter();
            currentCharacter++;

            if (currentCharacter >= characterCount)
            {
                currentCharacter = 0;
            }

            enableCurrentCharacter();
        }

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
                sitOnClosestSeat();

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
                standUp();
            }
        }

        base.Update();
    }

    protected void enableCurrentCharacter()
    {
        Avatar avatar = avatars[currentCharacter];
        GameObject model = models[currentCharacter];

        Debug.Log($"enbling {model.name}...");
        model.SetActive(true);
        animator.avatar = avatar;
    }

    protected void disableCurrentCharacter()
    {
        GameObject model = models[currentCharacter];

        Debug.Log($"disabling {model}...");
        model.SetActive(false);
    }

    protected void Pause()
    {
        paused = !paused;
        thirdPersonController.enabled = !paused;
    }
}
