using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;
using System.Data;
using UnityEditor.VersionControl;
using System.Net;

public class MicrophoneInput : MonoBehaviour
{
    private bool isRecording = false;
    private string deviceName;
    private AudioClip recording;
    private float startTime;
    private WebSocket ws;
    private string serverUrl = "wss://a8ca-186-129-180-196.ngrok-free.app/ws/audio";
    private AudioSource audioSource;
    private float sendTime;

    async void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // Check if microphone access is granted
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected!");
            return;
        }
        deviceName = Microphone.devices[0]; // Use the first available microphone

        // Initialize WebSocket
        ws = new WebSocket(serverUrl);

        // Set event handlers
        ws.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + message + ")");
            float receiveTime = Time.time;
            float roundTripTime = receiveTime - sendTime;
            Debug.Log("Received response. Round-trip time: " + roundTripTime + " seconds");
            ProcessReceivedAudio(bytes);

        };
        ws.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        ws.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        ws.OnClose += (e) =>
        {
            Debug.Log("Connection closed! " + e);
        };

        // Connect to the server
        await ws.Connect();


    }

    private void ProcessReceivedAudio(byte[] bytes)
    {
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "response2.wav"), bytes);
        AudioClip audioClip = ToAudioClip(Path.Combine(Application.persistentDataPath, "response2.wav"));
        Debug.Log("Audio file saved successfully at: " + Path.Combine(Application.persistentDataPath, "response.wav"));
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    async void Update()
    {

#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
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

    async void OnApplicationQuit()
    {
        // Close the WebSocket connection when the application quits
        if (ws != null)
        {
            ws.Close();
        }
    }

    public async void SendAudio(byte[] audio)
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.Send(audio);
            sendTime = Time.time;
            UnityEngine.Debug.Log("Sent audio");
            Debug.Log("Message sent at: " + sendTime + " seconds");
        }
        else
        {
            UnityEngine.Debug.Log("WebSocket is not connected");
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
            /*// Get the number of samples in the AudioClip
            int sampleCount = recording.samples * recording.channels;

            // Create a float array to hold the audio data
            float[] audioData = new float[sampleCount];

            // Get the audio data from the AudioClip
            recording.GetData(audioData, 0);

            // Convert the float array to a byte array
            byte[] byteArray = new byte[sampleCount * 4]; // 4 bytes per float
            Buffer.BlockCopy(audioData, 0, byteArray, 0, byteArray.Length);

            // Now you have the audio data in a byte array
            Debug.Log("Audio data converted to byte array.");*/


            /*byte[] wavData = ConvertAudioClipToWav(recording);
            Debug.Log("Audio data converted to WAV byte array.");*/


            string filePath = Path.Combine(Application.persistentDataPath, "Recording.wav");
            SavWav.Save(filePath, recording); // Save the recording as a WAV file

            Debug.Log("Recording saved to: " + filePath);
            byte[] audioBytes = File.ReadAllBytes(filePath);
            SendAudio(audioBytes);
        }
        else
        {
            Debug.LogError("AudioClip is null.");
        }
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

    public static AudioClip ToAudioClip(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);

        if (fileBytes.Length < 44)
        {
            throw new Exception("Invalid WAV file: Header too short");
        }

        int sampleRate = BitConverter.ToInt32(fileBytes, 24);
        int channels = BitConverter.ToInt16(fileBytes, 22);
        int bitDepth = BitConverter.ToInt16(fileBytes, 34);

        if (BitConverter.ToInt16(fileBytes, 20) != 1)
        {
            throw new Exception("Invalid WAV file: Only PCM format is supported");
        }

        if (bitDepth != 16)
        {
            throw new Exception("Invalid WAV file: Only 16-bit audio is supported");
        }

        int dataSize = BitConverter.ToInt32(fileBytes, 40);
        int samples = dataSize / 2;

        float[] audioData = new float[samples];
        int dataOffset = 44;
        for (int i = 0; i < samples; i++)
        {
            audioData[i] = BitConverter.ToInt16(fileBytes, dataOffset + i * 2) / 32768.0f;
        }

        AudioClip audioClip = AudioClip.Create("LoadedWav", samples, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }

}
