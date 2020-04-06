module SayIt.Voices

open Microsoft.FSharp.Reflection
open SayIt.Utils

type VoiceType =
    | De
    | En
    | Es
    | Fr
    | Hi
    | It
    | Ja
    | Pt
    | Ru
    | Zh
    override this.ToString() = toString this
    static member FromString s = fromString<VoiceType> s

let getVoiceId (voice: VoiceType) =
    match voice with
    | De -> "de-DE-KatjaNeural"
    | En -> "en-US-GuyNeural"
    | Es -> "es-ES-Laura-Apollo"
    | Fr -> "fr-FR-Julie-Apollo"
    | Hi -> "hi-IN-Kalpana-Apollo"
    | It -> "it-IT-ElsaNeural"
    | Ja -> "ja-JP-Ayumi-Apollo"
    | Pt -> "pt-BR-FranciscaNeural"
    | Ru -> "ru-RU-Irina-Apollo"
    | Zh -> "zh-CN-XiaoxiaoNeural"

let listVoices() =
    let types = FSharpType.GetUnionCases typeof<VoiceType>
    printfn "Shorthand -> Id pairs for supported voices (see https://aka.ms/speech/tts-languages):"
    for t in types do
        let id = getVoiceId (VoiceType.FromString(t.Name.ToLower()))
        printfn "%s -> %s" (t.Name.ToLower()) id
