For build
`> build.bat`

For run becnhmark
`> run.bat`

# Some stasts with benchmarkdotnet

 Method | NumberOfAgents | MessagesToTreat |      Mean |    Error |   StdDev |
------- |--------------- |---------------- |----------:|---------:|---------:|
    App |              1 |             100 | 101.309 s | 0.1481 s | 0.1313 s |
    App |              8 |             100 |  17.696 s | 0.5510 s | 1.5984 s |
    App |             32 |             100 |   7.457 s | 0.3580 s | 1.0498 s |
    App |             64 |             100 |   6.462 s | 0.1327 s | 0.3871 s |
    App |            128 |             100 |   6.523 s | 0.1288 s | 0.3370 s |
    App |            512 |             100 |   6.538 s | 0.1305 s | 0.3483 s |
    App |           1024 |             100 |   6.565 s | 0.1438 s | 0.4148 s |
