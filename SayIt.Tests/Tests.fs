module Tests

open System
open System.IO
open Xunit
open FsUnit.Xunit

module ``SayIt tests`` =

    type SayItTests () =

        [<Fact>] 
        member __.``when asked for version it should print it and exit with 0`` () =
            use sw = new StringWriter()
            Console.SetOut(sw)
            SayIt.Program.main([|"--version"|]) |> should equal 0
            sw.ToString() |> should equal (sprintf "sayit version %s\n" SayIt.Config.VERSION)
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()))


[<EntryPoint>]
let main argv = 0