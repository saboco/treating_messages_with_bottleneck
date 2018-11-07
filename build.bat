IF NOT EXIST paket.lock (
    START /WAIT .paket/paket.exe install
)

dotnet restore runner/
dotnet build runner/

