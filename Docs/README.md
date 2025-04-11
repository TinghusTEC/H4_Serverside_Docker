# README
## Build
```
docker build -t todo:v1 -f Dockerfile .
```

## Run
```
docker run -p 8000:8080 -p 8001:8081 -v C:\Users\mting\.aspnet\https\:/https/ -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_URLS="https://+:8081;http://+:8080" -e ASPNETCORE_Kestrel__Certificates__Default__Password="123456Abcd" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/TodoApp.pfx todo:v1
```