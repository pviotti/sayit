module SayIt.Voices

open Microsoft.FSharp.Reflection

// Create discriminated unions from string - http://fssnip.net/9l
let toString (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

let fromString<'a> (s: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
    | [| case |] -> Some(FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _ -> None

type VoiceType =
    | De
    | En
    | Es
    | Fr
    | Hi
    | It
    | Ja
    | Pr
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
    | Pr -> "pt-BR-FranciscaNeural"
    | Ru -> "ru-RU-Irina-Apollo"
    | Zh -> "zh-CN-XiaoxiaoNeural"
