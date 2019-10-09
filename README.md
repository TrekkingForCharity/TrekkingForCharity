# TrekkingForCharity
The main web site for Trekking for Charity


Dcoker watch
> `docker run --rm -it -p [port]:[port] -v [path-to-source]:/app/ -w /app/source/TrekkingForCharity.Web -e ASPNETCORE_URLS=http://+:[port] trekkingforcharity/aspcore-devbase:latest dotnet watch run`

docker run --rm -it -p 54352:80 -v E:\Github\TrekkingForCharity:/app/ -w /app/source/TrekkingForCharity.Web --name treks-debug trekkingforcharity/aspcore-devbase:latest dotnet watch run -c Debug --urls=http://+:80