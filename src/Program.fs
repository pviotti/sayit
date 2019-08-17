module Sayit.Program

open Sayit.Config

open Microsoft.CognitiveServices.Speech

[<EntryPoint>]
let main argv =
    let (config, arguments) = Config.getConfiguration(argv)

    let subid = config.GetResult SubscriptionId
    let subreg = config.GetResult SubscriptionRegion
    let voice = config.GetResult Voice
    //printfn "%s - %s" subid subreg
    let speechConfig = SpeechConfig.FromSubscription(subid, subreg)
    speechConfig.SpeechSynthesisVoiceName <- voice
    use synthetizer = new SpeechSynthesizer(speechConfig)
    let task = synthetizer.SpeakTextAsync <| arguments.GetResult Input
    task.Wait()
    match task.Result.Reason with
        | ResultReason.Canceled ->
            let cancellation = SpeechSynthesisCancellationDetails.FromResult task.Result
            if CancellationReason.Error = cancellation.Reason then
                printfn "ERROR: ErrorCode=%A" cancellation.ErrorCode
                printfn "ERROR: ErrorDetails=%A" cancellation.ErrorDetails
        | _ -> ()
    0
