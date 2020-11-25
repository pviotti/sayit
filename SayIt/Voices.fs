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
    | Es -> "es-ES-ElviraNeural"
    | Fr -> "fr-FR-DeniseNeural"
    | Hi -> "hi-IN-SwaraNeural"
    | It -> "it-IT-ElsaNeural"
    | Ja -> "ja-JP-NanamiNeural"
    | Pt -> "pt-BR-FranciscaNeural"
    | Ru -> "ru-RU-DariyaNeural"
    | Zh -> "zh-CN-XiaoxiaoNeural"

let listVoices() =
    let types = FSharpType.GetUnionCases typeof<VoiceType>
    printfn "Shorthand -> Id pairs for supported voices (see https://aka.ms/speech/tts-languages):"
    for t in types do
        let id = getVoiceId (VoiceType.FromString(t.Name.ToLower()))
        printfn " %s -> %s" (t.Name.ToLower()) id
