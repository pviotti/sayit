module Sayit.Program

open Sayit.Config

open Microsoft.CognitiveServices.Speech

let getVoiceId (voice:VoiceType) =
    match voice with
    | En -> "en-US-GuyNeural"
    | It -> "it-IT-ElsaNeural"
    | Fr -> "fr-FR-Julie-Apollo"

[<EntryPoint>]
let main argv =
    let config = Config.getConfiguration(argv)

    let subKey = config.GetResult SubscriptionId
    let subRegion = config.GetResult SubscriptionRegion
    let voice = getVoiceId (config.GetResult Voice)
    let input = config.GetResult Input

    let speechConfig = SpeechConfig.FromSubscription(subKey, subRegion)
    speechConfig.SpeechSynthesisVoiceName <- voice
    use synthetizer = new SpeechSynthesizer(speechConfig)
    let task = synthetizer.SpeakTextAsync input
    task.Wait()
    match task.Result.Reason with
        | ResultReason.Canceled ->
            let cancellation = SpeechSynthesisCancellationDetails.FromResult task.Result
            if CancellationReason.Error = cancellation.Reason then
                match cancellation.ErrorCode with
                | CancellationErrorCode.ConnectionFailure -> printfn "Error: please check your internet connection."
                | CancellationErrorCode.AuthenticationFailure -> printfn "Error: please check your credentials."
                | _ -> printfn "Error: ErrorCode=%A\nErrorDetails=%A" cancellation.ErrorCode cancellation.ErrorDetails
        | _ -> ()
    0
