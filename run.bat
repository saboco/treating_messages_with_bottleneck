taskkill /FI "MODULES eq bottleneck.dll" /F
rmdir /Q /S bin\

dotnet publish .\bottleneck\bottleneck.fsproj -c release -o ..\bin\app\bottleneck
dotnet publish .\runner\runner.fsproj -c release -o ..\bin\app\runner
start /b dotnet bin\app\bottleneck\bottleneck.dll

dotnet bin\app\runner\runner.dll --filter BottleneckBenchMark
taskkill /FI "MODULES eq bottleneck.dll" /F