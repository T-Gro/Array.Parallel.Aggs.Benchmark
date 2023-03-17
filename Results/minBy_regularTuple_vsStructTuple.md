// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1413/22H2/2022Update/SunValley2)
11th Gen Intel Core i9-11950H 2.60GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.300-preview.23122.5
  [Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2 DEBUG
  DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


|                   Method | NumberOfItems |            Mean |         Error |        StdDev | Completed Work Items | Lock Contentions |       Gen0 |    Gen1 |   Allocated |
|------------------------- |-------------- |----------------:|--------------:|--------------:|---------------------:|-----------------:|-----------:|--------:|------------:|
|              NormalMinBy |          1000 |        544.2 ns |      10.89 ns |      20.19 ns |                    - |                - |          - |       - |           - |
| InlinedUpToReduceByMinBy |          1000 |     10,987.4 ns |     219.46 ns |     277.54 ns |               8.1883 |           0.0003 |     2.9297 |  0.0305 |     36720 B |
|           minByViaStruct |          1000 |      9,705.5 ns |      93.19 ns |      77.82 ns |               8.4847 |                - |     0.3815 |       - |      4915 B |
|         minByHandCrafted |          1000 |      7,445.9 ns |     146.45 ns |     129.82 ns |               6.6886 |                - |     0.3967 |       - |      5009 B |
|              NormalMinBy |       5000000 | 14,677,214.2 ns | 118,534.48 ns |  98,981.66 ns |                    - |                - |          - |       - |        16 B |
| InlinedUpToReduceByMinBy |       5000000 | 11,616,323.7 ns | 221,131.48 ns | 287,533.38 ns |              17.0000 |           0.0156 | 12765.6250 | 15.6250 | 160006143 B |
|           minByViaStruct |       5000000 |  7,595,507.6 ns | 143,070.27 ns | 250,576.04 ns |              15.8750 |           0.0156 |          - |       - |      6156 B |
|         minByHandCrafted |       5000000 |  7,091,567.8 ns | 141,145.75 ns | 144,946.29 ns |              15.4688 |           0.0859 |          - |       - |      6456 B |