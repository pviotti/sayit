open System
open Microsoft.CognitiveServices.Speech

(* Get Azure subscription id and region from environment variables *)
let subId = Environment.GetEnvironmentVariable "SAYIT_SUBID"
let subRegion = Environment.GetEnvironmentVariable "SAYIT_SUBREGION"

[<EntryPoint>]
let main argv =
    Console.Write "type something you want to speak: "
    let line = Console.ReadLine()
    let config = SpeechConfig.FromSubscription(subId, subRegion)
    config.SpeechSynthesisVoiceName <- "en-US-GuyNeural"
    use synthetizer = new SpeechSynthesizer(config)
    let task = line |> synthetizer.SpeakTextAsync
    task.Wait()
    match task.Result.Reason with
        | ResultReason.Canceled ->
            let cancellation = SpeechSynthesisCancellationDetails.FromResult task.Result
            if CancellationReason.Error = cancellation.Reason then
                printfn "ERROR: ErrorCode=%A" cancellation.ErrorCode
                printfn "ERROR: ErrorDetails=%A" cancellation.ErrorDetails
        | _ -> ()
    0
