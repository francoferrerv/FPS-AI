using System;
using System.IO;
using UnityEngine;
using Whisper.net;

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
        recording = Microphone.Start(deviceName, loop, 60, 16000);
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

    async void SaveRecording()
    {
        float duration = Time.time - startTime;
        if (recording == null)
        {
            Debug.LogWarning("No recording to save!");
            return;
        }
        recording = TrimRecording(recording, duration);
        string filePath = Path.Combine(Application.persistentDataPath, "Recording.wav");
        SavWav.Save(filePath, recording); // Save the recording as a WAV file

        Debug.Log("Recording saved to: " + filePath);

        await Program.Main();
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
}
