module Tests

open Xunit

open Sayit.Program

[<Fact>]
let ``Failing run`` () =
    let ret = main([|"--version"|])
    Assert.Equal(0, ret)


[<EntryPoint>]
let main argv = 0