module Sayit.Config

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Reflection

open Argu

let VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString()
let PROGRAM_NAME = "sayit"
let CONFIG_FILE = "sayit.config"

type Env = Environment

// Create discriminated unions from string - http://fssnip.net/9l
let toString (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

let fromString<'a> (s: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
    | [| case |] -> Some(FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _ -> None

type VoiceType =
    | En
    | It
    | Fr
    override this.ToString() = toString this
    static member FromString s = fromString<VoiceType> s

type Args =
    | [<NoAppSettings>] Version
    | [<AltCommandLine("-v")>] Voice of VoiceType
    | [<AltCommandLine("-o"); NoAppSettings>] Output of output: string
    | [<NoCommandLine; Mandatory>] SubscriptionId of subId: string
    | [<NoCommandLine; Mandatory>] SubscriptionRegion of subRegion: string
    | [<MainCommand; Mandatory>] Input of input: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Version _ -> "print sayit version."
            | Voice _ -> "specify the voice."
            | Output _ -> "output file."
            | Input _ -> "the text to be pronounced"
            | SubscriptionId _ -> "the subscription id of the Azure Cognitive Services resource"
            | SubscriptionRegion _ -> "the region code of the Azure Cognitive Services resource"

let printVersion() = printfn "sayit version %s" VERSION

let getConfigFilePath() =
    Env.GetFolderPath(Env.SpecialFolder.ApplicationData, Env.SpecialFolderOption.Create)
    + string Path.DirectorySeparatorChar + CONFIG_FILE

let writeConfig (subKey: string, subReg: string, voice: VoiceType) =
    let parser = ArgumentParser.Create<Args>()

    let xml =
        parser.PrintAppSettingsArguments
            [ Args.SubscriptionId subKey
              Args.SubscriptionRegion subReg
              Args.Voice voice ]
    File.WriteAllText(getConfigFilePath(), xml, Text.Encoding.UTF8)

let configWizard() =
    Console.WriteLine "Please provide the following default configurations:"
    let ask (prompt: string) =
        Console.Write prompt
        Console.ReadLine()

    let subId = ask "Subscription id: "
    let subReg = ask "Subscription region: "

    let voice =
        match VoiceType.FromString(ask "Default voice [en]: ") with
        | Some x -> x
        | None -> En

    writeConfig (subId, subReg, voice)
    ("The configuration has been written to " + getConfigFilePath()) |> Console.WriteLine

let getConfiguration argv =
    let errorHandler =
        ProcessExiter
            (colorizer = function
             | ErrorCode.HelpText -> None
             | _ -> Some ConsoleColor.Red)

    let parser = ArgumentParser.Create<Args>(programName = PROGRAM_NAME, errorHandler = errorHandler)
    if not (File.Exists(getConfigFilePath())) then configWizard()
    let confReader = ConfigurationReader.FromAppSettingsFile(getConfigFilePath())
    parser.Parse(argv, confReader, ignoreMissing = true)
