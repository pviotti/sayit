module SayIt.Program

open SayIt.Config
open SayIt.Voices
open SayIt.Formats

open Argu
open System
open System.Threading.Tasks
open Microsoft.CognitiveServices.Speech
open Microsoft.CognitiveServices.Speech.Audio


let handleSynthesisResult (task: Task<SpeechSynthesisResult>) =
    task.Wait()
    use result = task.Result
    match result.Reason with
    | ResultReason.Canceled ->
        let cancellation = SpeechSynthesisCancellationDetails.FromResult result
        if CancellationReason.Error = cancellation.Reason then
            match cancellation.ErrorCode with
            | CancellationErrorCode.ConnectionFailure ->
                eprintfn "ERROR: connection failure.\nError details: %s" cancellation.ErrorDetails
            | CancellationErrorCode.AuthenticationFailure -> eprintfn "ERROR: please check your credentials."
            | _ ->
                eprintfn "ERROR: ErrorCode=%A\nError details: %s" cancellation.ErrorCode cancellation.ErrorDetails
        1
    | _ -> 0

let performSpeechSynthesis (config: Argu.ParseResults<Args>, speechConfig: SpeechConfig, input: string) =
    if config.Contains Output then
        let output = config.GetResult Output
        let outputFormat = getFormatId (config.PostProcessResult(Format, FormatType.FromString))
        speechConfig.SetSpeechSynthesisOutputFormat(outputFormat)
        use fileOutput = AudioConfig.FromWavFileOutput(output)
        use synthetizer = new SpeechSynthesizer(speechConfig, fileOutput)
        handleSynthesisResult (synthetizer.SpeakTextAsync(input))
    else
        use synthetizer = new SpeechSynthesizer(speechConfig)
        handleSynthesisResult (synthetizer.SpeakTextAsync(input))

[<EntryPoint>]
let main argv =
    match getConfiguration (argv) with
    | Config config ->

        let input =
            if config.Contains Input then
                config.GetResult Input
            elif Console.IsInputRedirected then
                let mutable input = String.Empty
                while Console.In.Peek() <> -1 do
                    input <- input + Console.ReadLine()
                input
            else
                config.Raise "ERROR: missing argument '<input>'." ErrorCode.CommandLine true

        let key = config.GetResult Key
        let region = config.GetResult Region
        let voice = getVoiceId (config.PostProcessResult(Voice, VoiceType.FromString))

        let speechConfig = SpeechConfig.FromSubscription(key, region)
        speechConfig.SpeechSynthesisVoiceName <- voice

        performSpeechSynthesis (config, speechConfig, input)
    | ReturnVal ret -> ret
