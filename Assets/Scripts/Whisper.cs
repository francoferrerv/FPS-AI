
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Whisper.net;
using Whisper.net.Ggml;

public class Program
{
    // This examples shows how to use Whisper.net to create a transcription from an audio file with 16Khz sample rate.
    public static async Task Main()
    {
        // We declare three variables which we will use later, ggmlType, modelFileName and wavFileName
        var ggmlType = GgmlType.Tiny;
        var modelFileName = "ggml-tiny.en.bin";
        var filePath= Path.Combine(Directory.GetCurrentDirectory(), "Assets/whisper.net-for-unity-main/whisper.cpp/Models/" + modelFileName);

        // This section detects whether the "ggml-base.bin" file exists in our project disk. If it doesn't, it downloads it from the internet
        if (!File.Exists(filePath))
        {
            await DownloadModel(modelFileName, ggmlType);
        }
        Debug.Log("EXISTS");
        using var processor = WhisperProcessorBuilder.Create()
             .WithSegmentEventHandler(OnNewSegment)
             .WithFileModel(filePath)
             .WithTranslate()
             .WithLanguage("auto")
             .Build();
        Debug.Log("BUILT");
        void OnNewSegment(object sender, OnSegmentEventArgs e)
        {
            Console.WriteLine($"CSSS {e.Start} ==> {e.End} : {e.Segment}");
        }

        using var fileStream = File.OpenRead(Path.Combine(Application.persistentDataPath, "Recording.wav"));
        processor.Process(fileStream);
    }

    private static async Task DownloadModel(string fileName, GgmlType ggmlType)
    {
        Console.WriteLine($"Downloading Model {fileName}");
        using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(ggmlType);
        using var fileWriter = File.OpenWrite(fileName);
        await modelStream.CopyToAsync(fileWriter);
    }
}
