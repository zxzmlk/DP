taskkill /f /im Valuator.exe
taskkill /f /im RankCalculator.exe
taskkill /f /im nats-server.exe
start /d "../nginx/" nginx.exe -s quit