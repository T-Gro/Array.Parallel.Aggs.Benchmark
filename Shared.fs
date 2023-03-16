module Shared

open System


let maxPartitions = Environment.ProcessorCount // The maximum number of partitions to use


let  createPartitionsUpToWithMinChunkSize maxIdxExclusive minChunkSize (array: 'T[]) =
    [|
        let chunkSize =
            match maxIdxExclusive with
            | smallSize when smallSize < minChunkSize -> smallSize
            | biggerSize when biggerSize % maxPartitions = 0 -> biggerSize / maxPartitions
            | biggerSize -> (biggerSize / maxPartitions) + 1

        let mutable offset = 0

        while (offset + chunkSize) < maxIdxExclusive do
            yield new ArraySegment<'T>(array, offset, chunkSize)
            offset <- offset + chunkSize

        yield new ArraySegment<'T>(array, offset, maxIdxExclusive - offset)
    |]

let chunkNonEmptyArrayAndPrepareEmptyResults (array: 'T[]) =  
    if array.Length = 0 then
        invalidArg "array" "array cannot be empty"

    let chunks = createPartitionsUpToWithMinChunkSize array.Length 1 array
    chunks,Array.zeroCreate chunks.Length