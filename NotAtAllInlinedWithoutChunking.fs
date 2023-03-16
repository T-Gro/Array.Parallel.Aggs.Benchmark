module NotAtAllInlinedWithoutChunking

open System.Threading.Tasks
open System.Collections.Concurrent

let reduceBy (projection:'T -> 'U) (reduction:'U -> 'U ->'U) (array: 'T[]) =

    let finalResults = new ConcurrentBag<_>()

    Parallel.For(
        0,
        array.Length,
        (fun _ -> None),
        (fun i pState pLocal ->
            let projected = projection array[i]
            match pLocal with
            | None ->  Some projected
            | Some x -> reduction x projected |> Some),
        (fun tState -> match tState with | None -> () | Some value -> finalResults.Add(value) )) |> ignore

    let chunkResults = finalResults.ToArray()
    let mutable finalResult = chunkResults[0]
    for i = 1 to chunkResults.Length - 1 do
        finalResult <- reduction finalResult chunkResults[i]

    finalResult

[<CompiledName("Reduce")>]
let reduce (reduction) (array: _[]) = array |> reduceBy id reduction           

[<CompiledName("MinBy")>]
let minBy (projection) (array: _[]) =
    array 
    |> reduceBy (fun x -> projection x, x) (fun a b -> if fst a < fst b then a else b)
    |> snd

[<CompiledName("Min")>]
let min (array: _[]) =
    array |> reduce (fun a b -> if a < b then a else b)