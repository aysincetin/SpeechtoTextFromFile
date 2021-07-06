using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace cog_service_demo
{
    class Program
    {
        public static async Task ContinuousRecognitionWithFileAsync()
        {
            var config = SpeechConfig.FromSubscription("yourKey", "yourRegion");
            config.SpeechRecognitionLanguage = "tr-TR";

            var stopRecognition = new TaskCompletionSource<int>();

            using (var audioInput = AudioConfig.FromWavFileInput("yourAudioPath.wav"))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {

                    /*  recognizer.Recognizing += (s, e) =>
                      {
                          Console.WriteLine(e.Result.Text);
                      };

                    */
                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            Console.WriteLine(e.Result.Text);
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }

                        stopRecognition.TrySetResult(0);
                    };
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    Task.WaitAny(new[] { stopRecognition.Task });

                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

                }
            }

        }

        async static Task Main(string[] args)
        {
           /* FileStream filestream = new FileStream("out.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);*/

            ContinuousRecognitionWithFileAsync().Wait();
            await ContinuousRecognitionWithFileAsync();
        }
    }
}