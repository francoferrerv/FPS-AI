using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;
using Whisper.Samples;
using System.Data;

public class MicrophoneInput : MonoBehaviour
{
    private bool isRecording = false;
    private string deviceName;
    private AudioClip recording;
    private float startTime;
    private WebSocket ws;
    private string serverUrl = "wss://9099-190-193-42-113.ngrok-free.app/ws/audio";

    [System.Serializable]
    public class Data
    {
        public string role;
        public string content;
    }
    async void Start()
    {
        
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
        ws.OnMessage  += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + message + ")");
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
            /*Data data = new Data();
            data.role = role;
            data.content = content;
            string jsonString = JsonUtility.ToJson(data);
            var uwr = new UnityWebRequest("https://38ee-190-193-42-113.ngrok-free.app/conversation/", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);*/
        
            await ws.Send(audio);
            UnityEngine.Debug.Log("Sent audio");
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

    private byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            int sampleCount = clip.samples * clip.channels;
            int frequency = clip.frequency;

            // Write WAV header
            WriteWavHeader(stream, clip.channels, frequency, sampleCount);

            // Get the audio data
            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);

            // Convert to PCM data
            short[] intData = new short[samples.Length];
            byte[] bytesData = new byte[intData.Length * sizeof(short)];
            int rescaleFactor = 32767; // to convert float to Int16

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                BitConverter.GetBytes(intData[i]).CopyTo(bytesData, i * sizeof(short));
            }

            stream.Write(bytesData, 0, bytesData.Length);

            return stream.ToArray();
        }
    }

    private void WriteWavHeader(Stream stream, int channels, int sampleRate, int samples)
    {
        int byteRate = sampleRate * channels * 2; // 2 bytes per sample

        stream.Write(new byte[] { 0x52, 0x49, 0x46, 0x46 }, 0, 4); // "RIFF"
        stream.Write(BitConverter.GetBytes(36 + samples * 2), 0, 4); // ChunkSize
        stream.Write(new byte[] { 0x57, 0x41, 0x56, 0x45 }, 0, 4); // "WAVE"
        stream.Write(new byte[] { 0x66, 0x6D, 0x74, 0x20 }, 0, 4); // "fmt "
        stream.Write(BitConverter.GetBytes(16), 0, 4); // Subchunk1Size
        stream.Write(BitConverter.GetBytes((short)1), 0, 2); // AudioFormat
        stream.Write(BitConverter.GetBytes((short)channels), 0, 2); // NumChannels
        stream.Write(BitConverter.GetBytes(sampleRate), 0, 4); // SampleRate
        stream.Write(BitConverter.GetBytes(byteRate), 0, 4); // ByteRate
        stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2); // BlockAlign
        stream.Write(BitConverter.GetBytes((short)16), 0, 2); // BitsPerSample
        stream.Write(new byte[] { 0x64, 0x61, 0x74, 0x61 }, 0, 4); // "data"
        stream.Write(BitConverter.GetBytes(samples * 2), 0, 4); // Subchunk2Size
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
