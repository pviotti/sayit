module SayIt.Utils

open Microsoft.FSharp.Reflection

// Create discriminated unions from string - http://fssnip.net/9l
let toString (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name.ToLower()

let fromString<'a> (s: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name.ToLower() = s) with
    | [| case |] -> FSharpValue.MakeUnion(case, [||]) :?> 'a
    | _ -> failwith (s + " not recognized as a valid parameter.")
