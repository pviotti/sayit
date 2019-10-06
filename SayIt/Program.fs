module Sayit.Program

open Sayit.Config

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
    match task.Result.Reason with
    | ResultReason.Canceled ->
        let cancellation = SpeechSynthesisCancellationDetails.FromResult task.Result
        if CancellationReason.Error = cancellation.Reason then
            match cancellation.ErrorCode with
            | CancellationErrorCode.ConnectionFailure -> 
                printfn "Error: please check your internet connection."
            | CancellationErrorCode.AuthenticationFailure -> 
                printfn "Error: please check your credentials."
            | _ ->
                printfn "Error: ErrorCode=%A\nErrorDetails=%A" cancellation.ErrorCode cancellation.ErrorDetails
        1
    | _ -> 0

let performSpeechSynthesis(config: Argu.ParseResults<Args>, speechConfig: SpeechConfig) =
    if config.Contains Version then
        Config.printVersion()
        0
    elif config.Contains Output then
        let output = config.GetResult Output
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3)
        use fileOutput = AudioConfig.FromWavFileOutput(output)
        use synthetizer = new SpeechSynthesizer(speechConfig, fileOutput)
        handleSynthesisResult (synthetizer.SpeakTextAsync (config.GetResult Input))
    else
        use synthetizer = new SpeechSynthesizer(speechConfig)
        handleSynthesisResult (synthetizer.SpeakTextAsync (config.GetResult Input))

[<EntryPoint>]
let main argv =
    let config = Config.getConfiguration (argv)

    let subKey = config.GetResult SubscriptionId
    let subRegion = config.GetResult SubscriptionRegion
    let voice = getVoiceId (config.GetResult Voice)

    let speechConfig = SpeechConfig.FromSubscription(subKey, subRegion)
    speechConfig.SpeechSynthesisVoiceName <- voice

    performSpeechSynthesis(config, speechConfig)