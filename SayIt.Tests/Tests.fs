module Tests

open System
open System.IO
open System.Text.RegularExpressions
open Xunit
open FsUnit.Xunit
open Microsoft.FSharp.Reflection

open SayIt.Voices
open SayIt.Formats

module ``SayIt tests`` =

    type SayItTests() =

        [<Fact>]
        member __.``when asked for version it should print it and exit with 0``() =
            use sw = new StringWriter()
            Console.SetOut(sw)
            SayIt.Program.main ([| "--version" |]) |> should equal 0
            sw.ToString().Trim() |> should equal (sprintf "sayit version %s" SayIt.Config.VERSION)
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()))

        [<Fact>]
        member __.``voice shorthand is prefix of voice id``() =
            for t in (FSharpType.GetUnionCases typeof<VoiceType>) do
                let shorthand = VoiceType.FromString(t.Name.ToLower())
                let shorthandStr = t.Name.ToLower()
                let voiceId = getVoiceId (shorthand)
                voiceId |> should startWith shorthandStr


        [<Fact>]
        member __.``format shorthand should match the right format id``() =
            for t in (FSharpType.GetUnionCases typeof<FormatType>) do
                let shorthand = FormatType.FromString(t.Name.ToLower())
                let shorthandStr = shorthand.ToString()
                let id = getFormatId shorthand
                let idStr = id.ToString().ToLower()
                let idLen = String.length idStr
                let featuresSh = Regex.Replace((shorthandStr.[0..2]), @"[a-zA-Z]", String.Empty)
                let featuresId = Regex.Replace((idStr.[(idLen - 3)..(idLen - 1)]), @"[a-zA-Z]", String.Empty)
                shorthandStr.[0..2] |> should startWith (idStr.[(idLen - 3)..(idLen - 1)])
                featuresSh |> should equal featuresId


[<EntryPoint>]
let main argv = 0
