module Tests

open Xunit
open FsUnit.Xunit

open Sayit.Program

module ``SayIt general tests`` =

    type SayitTests () = 
        [<Fact>] 
        member __.``when asked for version it should print it and exit with 0`` () =
            Sayit.Program.main([|"--version"|]) |> should equal 0


[<EntryPoint>]
let main argv = 0