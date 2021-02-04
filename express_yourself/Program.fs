// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

let parseEval s =
    Parse.parse s |> Expr.eval Map.empty

[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    let inp = "3 * 3 + 3 * 4 * 5"
    printfn "%s is %i" inp (parseEval inp)

    let inp2 = "3 - 3 - 3 - 4 - 5"
    let ast = Parse.parse inp2
    printfn "%s is %s and evaluates to %i" inp2 (Expr.toString ast) (parseEval inp2)

    let inp3 = "let x = 0 in (if (x + 1) then x + 2 else 3) + 4"
    printfn "%s is %i" inp3 (parseEval inp3)

    let inp4 = "(if (let x = 34 in x + x) then 69 else 420 + 1)"
    printfn "%s is %i" inp4 (parseEval inp4)

    0 // return an integer exit code
