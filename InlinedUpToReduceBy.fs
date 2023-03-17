module InlinedUpToReduceBy

open Shared
open System.Threading.Tasks

let reduceBy (projection:'T -> 'U) (reduction:'U -> 'U ->'U) (array: 'T[]) =
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

let inline reduce ([<InlineIfLambda>] reduction) (array: _[]) = array |> reduceBy id reduction           

let inline minBy ([<InlineIfLambda>] projection) (array: _[]) =
    array 
    |> reduceBy (fun x -> projection x, x) (fun a b -> if fst a < fst b then a else b)
    |> snd

let structFst struct(a,b) = a
let structSnd struct(a,b) = b

let inline minByViaStruct ([<InlineIfLambda>] projection) (array: _[]) =
    array 
    |> reduceBy (fun x -> struct(projection x, x)) (fun a b -> if structFst a < structFst b then a else b)
    |> structSnd

let inline min (array: _[]) =
    array |> reduce (fun a b -> if a < b then a else b)

let inline minViaMinBy (array: _[]) =
    array |> minByViaStruct id


let  minHandCrafted (array: _[]) =
    let chunks,chunkResults = chunkNonEmptyArrayAndPrepareEmptyResults array

    Parallel.For(
        0,
        chunks.Length,
        fun chunkIdx ->
            let chunk = chunks[chunkIdx]
            let mutable res = array[chunk.Offset]
            let lastIdx = chunk.Offset + chunk.Count - 1

            for i = chunk.Offset + 1 to lastIdx do
                let current = array[i]
                if current < res then
                    res <- current

            chunkResults[chunkIdx] <- res
    ) |> ignore

    chunkResults |> Array.min

let inline minByHandCrafted ([<InlineIfLambda>] projection) (array: _[]) =
    let chunks,chunkResults = chunkNonEmptyArrayAndPrepareEmptyResults array

    Parallel.For(
        0,
        chunks.Length,
        fun chunkIdx ->
            let chunk = chunks[chunkIdx]
            let mutable res = array[chunk.Offset]
            let mutable resProjected = projection res
            let lastIdx = chunk.Offset + chunk.Count - 1

            for i = chunk.Offset + 1 to lastIdx do
                let current = array[i]
                let curProjected = projection current
                if curProjected < resProjected then
                    res <- current
                    resProjected <- curProjected             

            chunkResults[chunkIdx] <- (resProjected,res)
    ) |> ignore

    chunkResults |> Array.minBy fst |> snd