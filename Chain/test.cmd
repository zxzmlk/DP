@echo off
start "Node 1" cmd /k dotnet run 7000 localhost 7001 --no-build
start "Node 2" cmd /k dotnet run 7001 localhost 7002 --no-build
start "Node 3" cmd /k dotnet run 7002 localhost 7003 true --no-build
start "Node 4" cmd /k dotnet run 7003 localhost 7004 --no-build
start "Node 5" cmd /k dotnet run 7004 localhost 7000 --no-build
