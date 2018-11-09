For build
`> build.bat`

For run becnhmark
`> run.bat`

# Some stasts with benchmarkdotnet

 |Method | NumberOfBottleneckAgents | NumberOfFetchAgents | BatchSize | BatchSizeForBottleneck | MessagesToTreat | WriteOnDisk |       Mean |      Error |    StdDev |     Median | Rank |
-|------ |------------------------- |-------------------- |---------- |----------------------- |---------------- |------------ |-----------:|-----------:|----------:|-----------:|-----:|
 |   App |                       64 |                   1 |       500 |                     10 |            1000 |       False |   757.1 ms | 3,006.7 ms | 780.83 ms |   375.3 ms |    1 |
 |   App |                       64 |                   1 |       500 |                     10 |            1000 |        True | 4,485.5 ms |   384.3 ms |  99.79 ms | 4,478.3 ms |    2 |
 |   App |                      128 |                   1 |       500 |                     10 |            1000 |       False |   734.5 ms | 3,157.8 ms | 820.08 ms |   364.2 ms |    1 |
 |   App |                      128 |                   1 |       500 |                     10 |            1000 |        True | 4,578.5 ms |   284.7 ms |  73.94 ms | 4,543.6 ms |    2 |
 |   App |                      256 |                   1 |       500 |                     10 |            1000 |       False |   724.7 ms | 1,828.9 ms | 474.96 ms |   396.6 ms |    1 |
 |   App |                      256 |                   1 |       500 |                     10 |            1000 |        True | 4,699.9 ms |   723.0 ms | 187.76 ms | 4,659.7 ms |    2 |
