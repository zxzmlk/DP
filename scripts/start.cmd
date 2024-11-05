  
start "Valuator 1" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start "Valuator 2" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start /d ..\nginx\ nginx.exe

start "Nats" /d ..\nats\ nats-server.exe

start "Rank calculator 1" /d ..\RankCalculator\ dotnet run --no-build
start "Rank calculator 2" /d ..\RankCalculator\ dotnet run --no-build