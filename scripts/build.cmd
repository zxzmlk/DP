rem start cd ..\RankCalculator\ & dotnet build
rem start cd ..\EventsLogger\EventsLogger\ & dotnet build
SET PATH_TO_ROOT=%~dp0
SET PATH_TO_APP=%~dp0..\Valuator\
SET PATH_TO_RANK_CALCULATOR=%~dp0..\RankCalculator\
SET PATH_TO_EVENTS_LOGGER=%~dp0..\EventsLogger\EventsLogger\

cd %PATH_TO_APP%
dotnet build                                                               

cd %PATH_TO_RANK_CALCULATOR%
dotnet build

cd %PATH_TO_EVENTS_LOGGER%
dotnet build

cd %PATH_TO_ROOT%