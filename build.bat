IF NOT EXIST paket.lock (
    START /WAIT .paket/paket.exe install
)
dotnet restore bottleneck/
dotnet build bottleneck/
