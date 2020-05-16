module SayIt.Formats

open Microsoft.CognitiveServices.Speech
open Microsoft.FSharp.Reflection
open SayIt.Utils

type FormatType =
    | Mp316khz32kbps
    | Mp316khz64kbps
    | Mp316khz128kbps
    | Mp324khz48kbps
    | Mp324khz96kbps
    | Mp324khz160kbps
    | Pcm8khz16b
    | Pcm16khz16b
    | Pcm24khz16b
    override this.ToString() = toString this
    static member FromString s = fromString<FormatType> s

let getFormatId (format: FormatType) =
    match format with
    | Mp316khz32kbps -> SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3
    | Mp316khz64kbps -> SpeechSynthesisOutputFormat.Audio16Khz64KBitRateMonoMp3
    | Mp316khz128kbps -> SpeechSynthesisOutputFormat.Audio16Khz128KBitRateMonoMp3
    | Mp324khz48kbps -> SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3
    | Mp324khz96kbps -> SpeechSynthesisOutputFormat.Audio24Khz96KBitRateMonoMp3
    | Mp324khz160kbps -> SpeechSynthesisOutputFormat.Audio24Khz160KBitRateMonoMp3
    | Pcm8khz16b -> SpeechSynthesisOutputFormat.Riff8Khz16BitMonoPcm
    | Pcm16khz16b -> SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm
    | Pcm24khz16b -> SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm

let listFormats() =
    let types = FSharpType.GetUnionCases typeof<FormatType>
    printfn "Shorthand -> Id pairs for supported output formats (see https://bit.ly/2UOjVpg):"
    for t in types do
        let id = getFormatId (FormatType.FromString(t.Name.ToLower()))
        printfn "%s -> %A" (t.Name.ToLower()) id
