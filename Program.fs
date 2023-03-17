open System

open System.Linq

open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Diagnosers
open BenchmarkDotNet.Diagnostics.Windows.Configs;
open BenchmarkDotNet.Order
open BenchmarkDotNet.Mathematics

type MiniRecord = {X:int; Y: float; YY : int64; YYY : int64}

type ElementType =
    | RecordWithInt = 0

type ProjectionType = Identity = 0 | Inverse = 1

[<MemoryDiagnoser>]
[<ThreadingDiagnoser>]
//[<DryJob>]  // Uncomment heere for quick local testing
type AggBenchMark()   = 
    let r = new Random(42)
    
    //[<ParamsAllValues(Priority = 3)>]   
    member val Type = ElementType.RecordWithInt with get,set

    //[<ParamsAllValues(Priority = 1)>]   
    member val ProjType = ProjectionType.Identity with get,set

    [<Params(1000,5_000_000, Priority = 0)>] 
    member val NumberOfItems = -1 with get,set

    member val ArrayWithItems = Array.zeroCreate<MiniRecord> 0 with get,set

    member this.GetRecords() = this.ArrayWithItems


    [<GlobalSetup>]
    member this.GlobalSetup () = 
        match this.Type with        
        | ElementType.RecordWithInt -> this.ArrayWithItems <- Array.init this.NumberOfItems (fun idx -> {X = idx |> string |> hash; Y = float idx; YY = 0L; YYY = 0L})

    [<Benchmark()>]
    member this.NormalMin () = 
        Array.min (this.GetRecords())

    //[<Benchmark()>]
    member this.NormalMinBy () = 
        Array.minBy (fun x -> x.X) (this.GetRecords())

    [<Benchmark()>]
    member this.InlinedUpToReduceByMin () = 
        InlinedUpToReduceBy.min (this.GetRecords())

    //[<Benchmark()>]
    member this.InlinedUpToReduceByMinBy () = 
        InlinedUpToReduceBy.minBy (fun x -> x.X) (this.GetRecords())

    //[<Benchmark()>]
    member this.minByViaStruct () = 
        InlinedUpToReduceBy.minByViaStruct (fun x -> x.X) (this.GetRecords())

    [<Benchmark()>]
    member this.fullyInlinedViaStruct () = 
        FullyInlined.minByViaStruct (fun x -> x.X) (this.GetRecords())

    [<Benchmark()>]
    member this.minViaMinBy () = 
        InlinedUpToReduceBy.minViaMinBy  (this.GetRecords())

    //[<Benchmark()>]
    member this.minByHandCrafted () = 
        InlinedUpToReduceBy.minByHandCrafted (fun x -> x.X) (this.GetRecords())

    [<Benchmark()>]
    member this.minHandCrafted () = 
        InlinedUpToReduceBy.minHandCrafted (this.GetRecords())


    //[<Benchmark()>]
    //member this.PlinqMin () = 
    //    (this.GetRecords()).Min()

    //[<Benchmark()>]
    //member this.PlinqMinBy () = 
    //   (this.GetRecords()).MinBy(fun x -> x.X)


    [<Benchmark()>]
    member this.FullyInlinedMin () = 
        FullyInlined.min (this.GetRecords())

    //[<Benchmark()>]
    //member this.FullyInlinedMinBy () = 
    //    FullyInlined.minBy (fun x -> x.X) (this.GetRecords())

    //[<Benchmark()>]
    //member this.FullyInlinedWithoutChunkingMin () = 
    //    FullyInlinedWithoutChunking.min (this.GetRecords())

    //[<Benchmark()>]
    //member this.FullyInlinedWithoutChunkingMinBY () = 
    //    FullyInlinedWithoutChunking.minBy (fun x -> x.X) (this.GetRecords())   

        
    [<Benchmark()>]
    member this.NotAtAllInlinedMIn () = 
        NotAtAllInlined.min (this.GetRecords())

    //[<Benchmark()>]
    //member this.NotAtAllInlinedMinBy () = 
    //    NotAtAllInlined.minBy (fun x -> x.X) (this.GetRecords())

        
    //[<Benchmark()>]
    //member this.NotAtAllInlinedWithoutChunkingMin () = 
    //    NotAtAllInlinedWithoutChunking.min (this.GetRecords())

    //[<Benchmark()>]
    //member this.NotAtAllInlinedWithoutChunkingMinBy () = 
    //    NotAtAllInlinedWithoutChunking.minBy (fun x -> x.X) (this.GetRecords())



[<EntryPoint>]
let main argv = 
    BenchmarkRunner.Run<AggBenchMark>() |> ignore   
    0