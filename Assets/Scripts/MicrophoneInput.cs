using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;
using WebSocketSharp;
using System.IO;

namespace Whisper.Samples
{
    [System.Serializable]
    public class Data
    {
        public string role;
        public string content;
    }

    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MicrophoneInput : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;
        private WebSocket ws;
        private string serverUrl = "https://38ee-190-193-42-113.ngrok-free.app/conversation/";


        [Header("UI")]
        public Button button;
        public Text buttonText;
        public Text outputText;
        public Text timeText;
        public Dropdown languageDropdown;
        public Toggle translateToggle;
        public Toggle vadToggle;
        public ScrollRect scroll;

        private string _buffer;

        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;
            whisper.OnProgress += OnProgressHandler;

            microphoneRecord.OnRecordStop += OnRecordStop;

            //button.onClick.AddListener(OnButtonPressed);
            languageDropdown.value = languageDropdown.options
                .FindIndex(op => op.text == "es");
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            translateToggle.isOn = whisper.translateToEnglish;
            translateToggle.onValueChanged.AddListener(OnTranslateChanged);

            vadToggle.isOn = microphoneRecord.vadStop;
            vadToggle.onValueChanged.AddListener(OnVadChanged);
        }

        void Start()
        {
            // Initialize WebSocket
            ws = new WebSocket(serverUrl);

            // Set event handlers
            ws.OnMessage += OnMessageReceived;

            ws.OnOpen += (sender, e) => {
                UnityEngine.Debug.Log("WebSocket connection opened");
            };

            ws.OnClose += (sender, e) => {
                UnityEngine.Debug.Log("WebSocket connection closed");
            };

            // Connect to the server
            ws.Connect();
        }

        void OnApplicationQuit()
        {
            // Close the WebSocket connection when the application quits
            if (ws != null)
            {
                ws.Close();
            }
        }

        public void SendText(string role, string content)
        {
            if (ws != null && ws.IsAlive)
            {
                Data data = new Data();
                data.role = role;
                data.content = content;
                string jsonString = JsonUtility.ToJson(data);
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
                ws.Send(jsonToSend);
                UnityEngine.Debug.Log("Sent text: " + content);
            }
            else
            {
                UnityEngine.Debug.Log("WebSocket is not connected");
            }
        }

        private void OnMessageReceived(object sender, MessageEventArgs e)
        {
            UnityEngine.Debug.Log("Message from server received");
            StartCoroutine(PlayAudioClip(e.RawData));
        }

        private IEnumerator PlayAudioClip(byte[] audioData)
        {
            // Load audio clip from raw audio data
            string tempFilePath = System.IO.Path.Combine(Application.persistentDataPath, "temp_audio.wav");
            System.IO.File.WriteAllBytes(tempFilePath, audioData);

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + tempFilePath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    UnityEngine.Debug.Log(www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    AudioSource audioSource = GetComponent<AudioSource>();
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                // Call the method when 'R' key is pressed
                OnButtonPressed();
            }
        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                UnityEngine.Debug.Log("Started Recording");
                buttonText.text = "Stop";
            }
            else
            {
                microphoneRecord.StopRecord();
                UnityEngine.Debug.Log("Stopped Recording");
                buttonText.text = "Record";
            }
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = "Record";
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();

            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null || !outputText)
                return;

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);
            timeText.text = $"Time: {time} ms\nRate: {rate:F1}x";

            var text = res.Result;
            if (printLanguage)
                text += $"\n\nLanguage: {res.Language}";

            outputText.text = text;
            UiUtils.ScrollDown(scroll);

            SendText("user", outputText.text);
        }


        /*IEnumerator SendRequest(string role, string content)
        {
            Data data = new Data();
            data.role = role;
            data.content = content;
            string jsonString = JsonUtility.ToJson(data);
            var uwr = new UnityWebRequest("https://38ee-190-193-42-113.ngrok-free.app/conversation/", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                UnityEngine.Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                UnityEngine.Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }*/

        private void OnLanguageChanged(int ind)
        {
            var opt = languageDropdown.options[ind];
            whisper.language = opt.text;
        }

        private void OnTranslateChanged(bool translate)
        {
            whisper.translateToEnglish = translate;
        }

        private void OnProgressHandler(int progress)
        {
            if (!timeText)
                return;
            timeText.text = $"Progress: {progress}%";
        }

        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            outputText.text = _buffer + "...";
            UiUtils.ScrollDown(scroll);
        }
    }
}
