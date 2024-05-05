using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MicrophoneInput : MonoBehaviour
{
    private bool isRecording = false;
    private string deviceName;
    private AudioClip recording;
    private float startTime;
    void Start()
    {
        // Check if microphone access is granted
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }
        deviceName = Microphone.devices[0]; // Use the first available microphone

       
    }
    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Toggle recording state
            isRecording = !isRecording;

            if (isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }
    }

    void StartRecording()
    {
        startTime = Time.time;
        bool loop = false; // Do not loop the recording
        recording = Microphone.Start(deviceName, loop, 60, 44100);
        // Wait until recording has started
        while (!(Microphone.GetPosition(deviceName) > 0)) { }
        // Recording has started
        Debug.Log("Recording started from microphone: " + deviceName);
    }

    void StopRecording()
    {
        Microphone.End(deviceName); // Stop recording
        // Save or process the recorded audio data here
        Debug.Log("Recording stopped");
        SaveRecording();
    }

    void SaveRecording()
    {
        float duration = Time.time - startTime;
        if (recording == null)
        {
            Debug.LogWarning("No recording to save!");
            return;
        }
        recording = TrimRecording(recording, duration);
        if (recording != null)
        {
            // Get the number of samples in the AudioClip
            int sampleCount = recording.samples * recording.channels;

            // Create a float array to hold the audio data
            float[] audioData = new float[sampleCount];

            // Get the audio data from the AudioClip
            recording.GetData(audioData, 0);

            // Convert the float array to a byte array
            byte[] byteArray = new byte[sampleCount * 4]; // 4 bytes per float
            Buffer.BlockCopy(audioData, 0, byteArray, 0, byteArray.Length);

            // Now you have the audio data in a byte array
            Debug.Log("Audio data converted to byte array.");

            StartCoroutine(UploadFile(byteArray));
        }
        else
        {
            Debug.LogError("AudioClip is null.");
        }
        /*string filePath = Path.Combine(Application.persistentDataPath, "Recording.wav");
        SavWav.Save(filePath, recording); // Save the recording as a WAV file

        Debug.Log("Recording saved to: " + filePath);*/
    }

    AudioClip TrimRecording(AudioClip originalClip, float targetDuration)
    {
        int targetSamples = (int)(targetDuration * originalClip.frequency); // Calculate the target number of samples
        float[] data = new float[targetSamples]; // Create a new array to store the trimmed audio data
        originalClip.GetData(data, 0); // Copy the audio data from the original clip to the new array
        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", targetSamples, originalClip.channels, originalClip.frequency, false); // Create a new AudioClip with the trimmed data
        trimmedClip.SetData(data, 0); // Set the trimmed audio data to the new clip
        return trimmedClip;
    }

    IEnumerator UploadFile(byte[] wavData)
    {
        // Read the .wav file into a byte array
        //byte[] wavData = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "Recording.wav"));

        // Create a UnityWebRequest
        string url = "https://your-server-endpoint.com/upload";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(wavData);
        request.SetRequestHeader("Content-Type", "audio/wav");

        // Send the request
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("File uploaded successfully!");
        }
        else
        {
            Debug.LogError("Error uploading file: " + request.error);
        }
    }

}
