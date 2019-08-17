open System
open System.IO
open System.Reflection

open Argu

open Microsoft.CognitiveServices.Speech

let VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString()
let PROGRAM_NAME = "sayit"
let CONFIG_FILE = "sayit.config"
let ENV_SUBID = "SAYIT_SUBID"
let ENV_SUBREGION = "SAYIT_SUBREGION"

type Env = Environment

type Args =
    | [<NoAppSettings>] Version
    | [<AltCommandLine("-v")>] Voice of voice:string
    | [<AltCommandLine("-s")>] Speed of speed:int
    | [<AltCommandLine("-l")>] Language of language:string
    | [<NoCommandLine>] SubscriptionId of subId:string
    | [<NoCommandLine>] SubscriptionRegion of subRegion:string
    | [<MainCommand>] Input of input:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Version _ -> "print sayit version."
            | Voice _ -> "specify the voice."
            | Speed _ -> "specify the speed."
            | Language _ -> "specify the language."
            | Input _ -> "the text to be pronounced"
            | SubscriptionId _ -> "the key of the Azure Cognitive Services resource"
            | SubscriptionRegion _ -> "the region code of the Azure Cognitive Services resource"

let printVersion() = printfn "sayit version %s" VERSION

let getConfigFilePath() =
    Env.GetFolderPath (Env.SpecialFolder.ApplicationData, Env.SpecialFolderOption.Create) +
        string Path.DirectorySeparatorChar +
        CONFIG_FILE

let getConfig() =
    printfn "%s" <| getConfigFilePath()
    let subId = Env.GetEnvironmentVariable ENV_SUBID
    let subRegion = Env.GetEnvironmentVariable ENV_SUBREGION
    (subId, subRegion)

let writeConfig (subKey:string, subReg:string, voice:string, speed:int) =
    let parser = ArgumentParser.Create<Args>()
    let xml = parser.PrintAppSettingsArguments [
        Args.SubscriptionId subKey ;
        Args.SubscriptionRegion subReg ;
        Args.Voice voice ;
        Args.Language "en-US" ;
        Args.Speed speed
    ]
    File.WriteAllText(getConfigFilePath(), xml, Text.Encoding.UTF8)

let configWizard() =
    Console.WriteLine "Please fill the following:"
    let ask (prompt:string) = Console.Write prompt ; Console.ReadLine()
    let subKey = ask "Subscription key: "
    let subReg = ask "Subscription region: "
    let voice = ask "Voice [en-US-GuyNeural]: "
    let speed = int(ask "Speed [1]: ")
    writeConfig (subKey, subReg, voice, speed)
    ("The configuration has been written to " + getConfigFilePath()) |> Console.WriteLine

[<EntryPoint>]
let main argv =
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<Args>(programName = PROGRAM_NAME, errorHandler = errorHandler)
    if not(File.Exists(getConfigFilePath())) then configWizard()
    let confReader = ConfigurationReader.FromAppSettingsFile(getConfigFilePath())
    let config = parser.ParseConfiguration(confReader)
    let arguments = parser.Parse(argv)

    if arguments.Contains Args.Version then printVersion(); Env.Exit 0
    if not(arguments.Contains Input) then printfn "%s" <| parser.PrintUsage(); Env.Exit 1

    printfn "%A" <| arguments.GetAllResults()
    let subid = config.GetResult SubscriptionId
    let subreg = config.GetResult SubscriptionRegion
    let voice = config.GetResult Voice
    printfn "%s - %s" subid subreg
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
