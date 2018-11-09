For build
`> build.bat`

For run becnhmark
`> run.bat`

# Some stasts with benchmarkdotnet

| Method | NumberOfBottleneckAgents | NumberOfFetchAgents | BatchSize | BatchSizeForBottleneck | MessagesToTreat | WriteOnDisk |       Mean |      Error |    StdDev |     Median | Rank |
|-------:|-------------------------:|--------------------:|----------:|-----------------------:|----------------:|------------:|-----------:|-----------:|----------:|-----------:|-----:|
|    App |                       64 |                   1 |       500 |                     10 |            1000 |       False |   757.1 ms | 3,006.7 ms | 780.83 ms |   375.3 ms |    1 |
|    App |                      128 |                   1 |       500 |                     10 |            1000 |       False |   734.5 ms | 3,157.8 ms | 820.08 ms |   364.2 ms |    1 |
|    App |                      256 |                   1 |       500 |                     10 |            1000 |       False |   724.7 ms | 1,828.9 ms | 474.96 ms |   396.6 ms |    1 |
|                                                                                                                                                                                        |
|    App |                       64 |                   1 |       500 |                     10 |            1000 |        True | 4,485.5 ms |   384.3 ms |  99.79 ms | 4,478.3 ms |    1 |
|    App |                      128 |                   1 |       500 |                     10 |            1000 |        True | 4,578.5 ms |   284.7 ms |  73.94 ms | 4,543.6 ms |    2 |
|    App |                      256 |                   1 |       500 |                     10 |            1000 |        True | 4,699.9 ms |   723.0 ms | 187.76 ms | 4,659.7 ms |    3 |

---

| Method | NumberOfBottleneckAgents | NumberOfFetchAgents | BatchSize | BatchSizeForBottleneck | MessagesToTreat | WriteOnDisk |        Mean |       Error |      StdDev |      Median | Rank |
|-------:|-------------------------:|--------------------:|----------:|-----------------------:|----------------:|------------:|------------:|------------:|------------:|------------:|-----:|
|    App |                      256 |                   1 |       500 |                     10 |            1000 |       False |    399.9 ms |    201.6 ms |    52.36 ms |    414.6 ms |    1 |
|    App |                      128 |                   1 |       500 |                     10 |            1000 |       False |    461.3 ms |    316.5 ms |    82.20 ms |    429.1 ms |    2 |
|    App |                       64 |                   1 |       500 |                     10 |            1000 |       False |    845.4 ms |  4,694.5 ms | 1,219.13 ms |    302.8 ms |    3 |
|                                                                                                                                                                                             |
|    App |                      256 |                   1 |       500 |                     10 |           10000 |       False |  2,992.3 ms |  2,643.2 ms |   686.44 ms |  2,747.6 ms |    1 |
|    App |                       64 |                   1 |       500 |                     10 |           10000 |       False |  3,068.4 ms |    894.2 ms |   232.22 ms |  3,121.9 ms |    2 |
|    App |                      128 |                   1 |       500 |                     10 |           10000 |       False |  5,152.9 ms | 17,781.1 ms | 4,617.70 ms |  3,094.4 ms |    3 |
|                                                                                                                                                                                             |
|    App |                       64 |                   1 |       500 |                     10 |            1000 |        True |  4,517.3 ms |    357.4 ms |    92.81 ms |  4,482.7 ms |    1 |
|    App |                      256 |                   1 |       500 |                     10 |            1000 |        True |  4,968.7 ms |  1,765.8 ms |   458.58 ms |  4,971.3 ms |    2 |
|    App |                      128 |                   1 |       500 |                     10 |            1000 |        True |  5,004.4 ms |  1,176.2 ms |   305.46 ms |  4,932.8 ms |    3 |
|                                                                                                                                                                                             |
|    App |                      256 |                   1 |       500 |                     10 |           10000 |        True | 46,978.7 ms |  8,697.2 ms | 2,258.63 ms | 47,458.4 ms |    1 |
|    App |                       64 |                   1 |       500 |                     10 |           10000 |        True | 47,251.4 ms |  4,177.4 ms | 1,084.85 ms | 47,182.7 ms |    2 |
|    App |                      128 |                   1 |       500 |                     10 |           10000 |        True | 49,106.7 ms |  1,943.6 ms |   504.74 ms | 49,407.5 ms |    3 |
