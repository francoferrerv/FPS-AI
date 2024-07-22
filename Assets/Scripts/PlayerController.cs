using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class PlayerController: BaseController
{
    protected internal ThirdPersonController thirdPersonController;
    public bool paused { get; private set; }
    [Header("Characters")]
    public Avatar[] avatars;
    public GameObject[] models;
    [Header("Images for The Boss")]
    public GameObject ImagePlane;
    public Texture2D[] Images;
    [Header("Skybox")]
    public float SkyboxRotationDelta = 0.5f;
    [Header("Talking NPC")]
    public NpcCameraScreenshot npcCameraScreenshot;
    protected float currentSkyboxRotation = 0.0f;
    protected int currentCharacter;
    protected int characterCount;
    protected int imageCount;

    protected override void Start()
    {
        base.Start();
        thirdPersonController = this.GetComponent<ThirdPersonController>();
        RenderSettings.skybox.SetFloat("_Rotation", currentSkyboxRotation);
        imageCount = Images.Length;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }

        if (paused)
        {
            return;
        }

        HandleImageKeys();

        HandleCharacterSwapKeys();

        HandleKeysRelatedToSitting();

        HandleSkyboxRotationKeys();

        base.Update();
    }

    protected void HandleSkyboxRotationKeys()
    {
        if (Input.GetKey(KeyCode.I))
        {
            currentSkyboxRotation += SkyboxRotationDelta;
            currentSkyboxRotation %= 360;
            RenderSettings.skybox.SetFloat("_Rotation", currentSkyboxRotation);
        }

        if (Input.GetKey(KeyCode.O))
        {
            currentSkyboxRotation -= SkyboxRotationDelta;

            if (currentSkyboxRotation < 0)
            {
                currentSkyboxRotation = 360 - currentSkyboxRotation;
            }

            RenderSettings.skybox.SetFloat("_Rotation", currentSkyboxRotation);
        }

    }

    protected void HandleKeysRelatedToSitting()
    {
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
    }

    protected void SwapToPreviousCharacter()
    {
        DisableCurrentCharacter();
        currentCharacter--;

        if (currentCharacter < 0)
        {
            currentCharacter = characterCount - 1;
        }

        EnableCurrentCharacter();
    }

    protected void SwapToNextCharacter()
    {
        DisableCurrentCharacter();
        currentCharacter++;

        if (currentCharacter >= characterCount)
        {
            currentCharacter = 0;
        }

        EnableCurrentCharacter();
    }

    protected void HandleCharacterSwapKeys()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            SwapToPreviousCharacter();
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            SwapToNextCharacter();
        }
    }

    protected int GetAlphaNumber()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown((KeyCode)(48 + i)))
            {
                return i;
            }
        }

        return -1;
    }

    protected void DisableImagePlane(int imageNumber)
    {
        if (imageNumber >= 1 && imageNumber < 10)
        {
            Debug.Log($"disabling {ImagePlane.name}...");
            ImagePlane.SetActive(false);
        }
    }

    protected void SetPlaneScale(Texture2D texture)
    {
        const float maxScaleX = 0.35f;
        const float maxScaleY = 0.2f;
        const float maxRatio = maxScaleX / maxScaleY;
        float textureWidth = texture.width;
        float textureHeight = texture.height;
        float textureRatio = textureWidth / textureHeight;
        float scaleX = maxScaleX;
        float scaleY = maxScaleY;

        if (textureRatio < maxRatio)
        {
            scaleX = maxScaleY * textureRatio;
        }

        if (textureRatio > maxRatio)
        {
            scaleY = maxScaleX / textureRatio;
        }

        Debug.Log($"Scale x: {scaleX}, z: {scaleY}");
        ImagePlane.transform.localScale = new Vector3(scaleX, 1, scaleY);
    }

    protected void EnableImagePlane(int imageNumber)
    {
        if (imageNumber >= 1 && imageNumber <= imageCount)
        {
            Texture2D texture = Images[imageNumber - 1];
            Material material = ImagePlane.GetComponent<Renderer>().material;

            Debug.Log($"enabling {ImagePlane.name} with image {imageNumber - 1}...");
            SetPlaneScale(texture);
            material.mainTexture = texture;
            ImagePlane.SetActive(true);
        }
    }

    protected void HandleImageKeys()
    {
        int imageNumber = GetAlphaNumber();

        if (ImagePlane.activeSelf)
        {
            DisableImagePlane(imageNumber);
        }
        else
        {
            EnableImagePlane(imageNumber);
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            IEnumerator screenshotCoroutine = npcCameraScreenshot.CaptureScreenshot();
            StartCoroutine(screenshotCoroutine);
        }
    }

    protected void EnableCurrentCharacter()
    {
        Avatar avatar = avatars[currentCharacter];
        GameObject model = models[currentCharacter];

        Debug.Log($"enbling {model.name}...");
        model.SetActive(true);
        animator.avatar = avatar;
    }

    protected void DisableCurrentCharacter()
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
