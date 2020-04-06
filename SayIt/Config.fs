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
    | [<NoAppSettings>] Version
    | [<NoAppSettings>] Setup    
    | [<AltCommandLine("-lv"); NoAppSettings>] List_Voices
    | [<AltCommandLine("-lf"); NoAppSettings>] List_Formats
    | [<AltCommandLine("-v")>] Voice of voice: string
    | [<AltCommandLine("-f")>] Format of format: string
    | [<AltCommandLine("-o"); NoAppSettings>] Output of output: string
    | [<NoCommandLine; Mandatory>] Key of key: string
    | [<NoCommandLine; Mandatory>] Region of region: string
    | [<MainCommand; Mandatory; NoAppSettings>] Input of input: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Version _ -> "print sayit version"
            | Setup _ -> "setup the configuration file"            
            | List_Voices _ -> "list available voice shorthands, with their corresponding voice ids"
            | List_Formats _ -> "list available output format shorthands, with their corresponding output format ids"
            | Voice _ -> "the voice shorthand, which maps to one of the available voice ids (see `sayit -lv` for details)"
            | Format _ -> "the format shorthand of the audio output, which maps to one fo the available format ids (see `sayit -lf` for details)"
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

let writeConfig (key: string, region: string, voice: VoiceType, format: FormatType) =
    let parser = ArgumentParser.Create<Args>()

    let xml =
        parser.PrintAppSettingsArguments
            [ Key key
              Region region
              Voice (voice.ToString()) 
              Format (format.ToString()) ]
    File.WriteAllText(getConfigFilePath(), xml, Text.Encoding.UTF8)

let configWizard() =
    Console.WriteLine "Please provide the following default configurations:"
    let ask (prompt: string) =
        Console.Write prompt
        Console.ReadLine()

    let subId = ask "Subscription key: "
    let subReg = ask "Region identifier: "

    let voice =
        try
            VoiceType.FromString(ask "Default voice [en]: ") 
        with
            | Failure _ -> 
                Console.WriteLine "Voice defaulted to \"en\"."
                En

    let format =
        try
            FormatType.FromString(ask "Default output format [mp324khz96kbps]: ")  
        with
            | Failure _ -> 
                Console.WriteLine "Output format defaulted to \"mp324khz96kbps\"."
                Mp324khz96kbps

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
    elif config.Contains List_Voices then
        listVoices()
        ReturnVal 0
    elif config.Contains List_Formats then
        listFormats()
        ReturnVal 0
    elif File.Exists(getConfigFilePath()) then
        let confReader = ConfigurationReader.FromAppSettingsFile(getConfigFilePath())
        Config (parser.Parse(argv, confReader, ignoreMissing = true))
    else
        Config config
