taskkill /f /im Valuator.exe
taskkill /f /im RankCalculator.exe
taskkill /f /im EventsLogger.exe
taskkill /f /im nats-server.exe
start /d "../nginx/" nginx.exe -s quit
start redis_stop.cmd