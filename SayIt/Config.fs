module SayIt.Config

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
    | [<NoAppSettings>] Setup
    | [<NoAppSettings>] Version
    | [<AltCommandLine("-v")>] Voice of VoiceType
    | [<AltCommandLine("-o"); NoAppSettings>] Output of output: string
    | [<NoCommandLine; Mandatory>] Key of key: string
    | [<NoCommandLine; Mandatory>] Region of region: string
    | [<MainCommand; Mandatory>] Input of input: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Setup _ -> "setup the configuration file"
            | Version _ -> "print sayit version"
            | Voice _ -> "the voice"
            | Output _ -> "the path of the output file"
            | Input _ -> "the text to be pronounced"
            | Key _ -> "the subscription key of your Azure Cognitive Services resource"
            | Region _ -> "the region identifier of your Azure Cognitive Services resource (see: https://aka.ms/speech/sdkregion)"

type ConfigOrInt =
    | Config of Argu.ParseResults<Args>
    | ReturnVal of int

let printVersion() = printfn "sayit version %s" VERSION

let getConfigFilePath() =
    Env.GetFolderPath(Env.SpecialFolder.ApplicationData, Env.SpecialFolderOption.Create)
    + string Path.DirectorySeparatorChar + CONFIG_FILE

let writeConfig (key: string, region: string, voice: VoiceType) =
    let parser = ArgumentParser.Create<Args>()

    let xml =
        parser.PrintAppSettingsArguments
            [ Key key
              Region region
              Voice voice ]
    File.WriteAllText(getConfigFilePath(), xml, Text.Encoding.UTF8)

let configWizard() =
    Console.WriteLine "Please provide the following default configurations:"
    let ask (prompt: string) =
        Console.Write prompt
        Console.ReadLine()

    let subId = ask "Subscription key: "
    let subReg = ask "Region identifier: "

    let voice =
        match VoiceType.FromString(ask "Default voice [en]: ") with
        | Some x -> x
        | None -> En

    writeConfig (subId, subReg, voice)
    ("The configuration has been written to " + getConfigFilePath()) |> Console.WriteLine

let getConfiguration argv =
    let parser = ArgumentParser.Create<Args>(programName = PROGRAM_NAME, errorHandler = ProcessExiter())
    let config = parser.Parse(argv, ignoreMissing=true)

    if config.Contains Version then
        printVersion()
        ReturnVal 0
    elif config.Contains Setup then
        configWizard()
        ReturnVal 0
    elif File.Exists(getConfigFilePath()) then
        let confReader = ConfigurationReader.FromAppSettingsFile(getConfigFilePath())
        Config (parser.Parse(argv, confReader, ignoreMissing = true))
    else
        Config config
