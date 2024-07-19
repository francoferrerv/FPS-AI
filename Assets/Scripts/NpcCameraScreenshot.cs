using UnityEngine;
using System.Collections;
using System.IO;

public class NpcCameraScreenshot : MonoBehaviour
{
    public Camera npcCamera;
    public int resolutionWidth = 1344;
    public int resolutionHeight = 336;
    public Camera mainCamera;

    void Start()
    {
        if (npcCamera == null)
        {
            Debug.LogError("NPC Camera is not assigned.");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        npcCamera.gameObject.SetActive(false);  // Ensure NPC Camera is disabled by default
    }
    public IEnumerator CaptureScreenshot()
    {

        npcCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);
        // Create a RenderTexture with the desired resolution
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        npcCamera.targetTexture = rt;
        Texture2D screenshot = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);

        // Render the camera's view
        npcCamera.Render();
        yield return new WaitForEndOfFrame();

        // Read the RenderTexture into the Texture2D
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        screenshot.Apply();

        // Reset the camera's target texture and active texture
        npcCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Encode the texture to PNG and return the byte array
        byte[] screenshotBytes = screenshot.EncodeToPNG();
        Destroy(screenshot);

        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "NPCScreenshot.png"), screenshotBytes);
        Debug.Log("Screenshot saved! at " + Path.Combine(Application.persistentDataPath, "NPCScreenshot.png"));

        yield return screenshotBytes;

        mainCamera.gameObject.SetActive(true);
        npcCamera.gameObject.SetActive(false);
    }
}
