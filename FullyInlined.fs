module FullyInlined

open Shared
open System.Threading.Tasks

let inline reduceBy ([<InlineIfLambda>]projection:'T -> 'U) ([<InlineIfLambda>] reduction:'U -> 'U ->'U) (array: 'T[]) =
    let chunks,chunkResults = chunkNonEmptyArrayAndPrepareEmptyResults array

    Parallel.For(
        0,
        chunks.Length,
        fun chunkIdx ->
            let chunk = chunks[chunkIdx]
            let mutable res = projection array[chunk.Offset]
            let lastIdx = chunk.Offset + chunk.Count - 1

            for i = chunk.Offset + 1 to lastIdx do
                let projected = projection array[i]
                res <- reduction res projected

            chunkResults[chunkIdx] <- res
    ) |> ignore

    let mutable finalResult = chunkResults[0]

    for i = 1 to chunkResults.Length - 1 do
        finalResult <- reduction finalResult chunkResults[i]

    finalResult

[<CompiledName("Reduce")>]
let inline reduce ([<InlineIfLambda>] reduction) (array: _[]) = array |> reduceBy id reduction           

[<CompiledName("MinBy")>]
let inline minBy ([<InlineIfLambda>] projection) (array: _[]) =
    array 
    |> reduceBy (fun x -> projection x, x) (fun a b -> if fst a < fst b then a else b)
    |> snd

[<CompiledName("Min")>]
let inline min (array: _[]) =
    array |> reduce (fun a b -> if a < b then a else b)

