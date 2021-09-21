# NewsApiAndGo

This application is written for the purpose of an exercise. It has two application in it. First application is written with .Net Core language, application takes news from Google NEWS API and pushes them to queue. Second application is written with GO language, it listens rabbitmq queue and when message is received application inserts news to the database. 
## Getting Started

Make sure you have installed rabbitmq and sql server.

```powershell
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=********” -p 1433:1433 --name sql1 -h sql1 -d mcr.microsoft.com/mssql/server:2019-latest
```

### Building 

For building go project, go to `../NewsListener/` then type `go build main.go`
For building .Net Core project, go to `../News.API` then type `dotnet build` 

### Runing

For running go project, go to `../NewsListener/` then type `go run main.go`
For building .Net Core project, go to `../News.API` then type `dotnet run` 
