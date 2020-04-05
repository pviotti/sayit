module SayIt.Config

open System
open System.IO
open System.Reflection

open Argu

open SayIt.Voices
open SayIt.Formats

let VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString()
let PROGRAM_NAME = "sayit"
let CONFIG_FILE = "sayit.config"

type Env = Environment

type Args =
    | [<NoAppSettings>] Setup
    | [<NoAppSettings>] Version
    | [<AltCommandLine("-lv"); NoAppSettings>] ListVoices
    | [<AltCommandLine("-lf"); NoAppSettings>] ListFormats
    | [<AltCommandLine("-v")>] Voice of VoiceType
    | [<AltCommandLine("-o"); NoAppSettings>] Output of output: string
    | [<AltCommandLine("-f")>] Format of FormatType
    | [<NoCommandLine; Mandatory>] Key of key: string
    | [<NoCommandLine; Mandatory>] Region of region: string
    | [<MainCommand; Mandatory; NoAppSettings>] Input of input: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Setup _ -> "setup the configuration file"
            | Version _ -> "print sayit version"
            | ListVoices _ -> "list available voice shorthands, with their corresponding voice ids"
            | ListFormats _ -> "list available output formats"
            | Voice _ -> "the voice shorthand, which maps to one of the available voice ids (see https://aka.ms/speech/tts-languages)"
            | Output _ -> "the path of the output file"
            | Format _ -> "the format of the audio output"
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

let writeConfig (key: string, region: string, voice: VoiceType, format: FormatType) =
    let parser = ArgumentParser.Create<Args>()

    let xml =
        parser.PrintAppSettingsArguments
            [ Key key
              Region region
              Voice voice 
              Format format ]
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

    let format =
        match FormatType.FromString(ask "Default output format [Mp324khz96kbps]: ") with
        | Some x -> x
        | None -> Mp324khz96kbps

    writeConfig (subId, subReg, voice, format)
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
    elif config.Contains ListVoices then
        listVoices()
        ReturnVal 0
    elif config.Contains ListFormats then
        listFormats()
        ReturnVal 0
    elif File.Exists(getConfigFilePath()) then
        let confReader = ConfigurationReader.FromAppSettingsFile(getConfigFilePath())
        Config (parser.Parse(argv, confReader, ignoreMissing = true))
    else
        Config config
