module SayIt.Program

open SayIt.Config

open System.Threading.Tasks
open Microsoft.CognitiveServices.Speech
open Microsoft.CognitiveServices.Speech.Audio


let getVoiceId (voice: VoiceType) =
    match voice with
    | En -> "en-US-GuyNeural"
    | It -> "it-IT-ElsaNeural"
    | Fr -> "fr-FR-Julie-Apollo"

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

let performSpeechSynthesis (config: Argu.ParseResults<Args>, speechConfig: SpeechConfig) =
    if config.Contains Output then
        let output = config.GetResult Output
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3)
        use fileOutput = AudioConfig.FromWavFileOutput(output)
        use synthetizer = new SpeechSynthesizer(speechConfig, fileOutput)
        handleSynthesisResult (synthetizer.SpeakTextAsync(config.GetResult Input))
    else
        use synthetizer = new SpeechSynthesizer(speechConfig)
        handleSynthesisResult (synthetizer.SpeakTextAsync(config.GetResult Input))

[<EntryPoint>]
let main argv =
    match Config.getConfiguration (argv) with
    | Config config ->
        let key = config.GetResult Key
        let region = config.GetResult Region
        let voice = getVoiceId (config.GetResult Voice)

        let speechConfig = SpeechConfig.FromSubscription(key, region)
        speechConfig.SpeechSynthesisVoiceName <- voice

        performSpeechSynthesis (config, speechConfig)
    | ReturnVal ret -> ret
