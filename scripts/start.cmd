start redis_start.cmd

start "Valuator 1" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001"
start "Valuator 2" /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5002"

start /d ..\nginx\ nginx.exe

start "Nats" /d ..\nats\ nats-server.exe

start "Rank calculator 1" /d ..\RankCalculator\ cmd /k dotnet run --no-build
start "Rank calculator 2" /d ..\RankCalculator\ cmd /k dotnet run --no-build

start "Events logger 1" /d ..\EventsLogger\EventsLogger\ cmd /k dotnet run --no-build
start "Events logger 2" /d ..\EventsLogger\EventsLogger\ cmd /k dotnet run --no-build