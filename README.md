# StoreWeb
[![GitHub stars](https://img.shields.io/github/stars/StoreDev/StoreWeb?style=social)](https://github.com/StoreDev/StoreWeb)
[![GitHub Workflow - Docker](https://img.shields.io/github/workflow/status/StoreDev/StoreWeb/docker?label=docker)](https://github.com/StoreDev/StoreWeb/actions?query=workflow%3Adocker)
[![GitHub Workflow - Build](https://img.shields.io/github/workflow/status/StoreDev/StoreWeb/build?label=build)](https://github.com/StoreDev/StoreWeb/actions?query=workflow%3Abuild)
[![Docker Pulls](https://img.shields.io/docker/pulls/storedev/store-web)](https://hub.docker.com/r/storedev/store-web)

StoreWeb is a webapp that makes use of [StoreLib](https://github.com/StoreDev/StoreLib).


## Usage

Clone the repo and build StoreWeb using either Visual Studio 2022, VS Code or dotnet CLI.

### Build

```sh
dotnet build
```

### Run

Run StoreWeb.dll using the ASP.NET 7.0 runtime

```
dotnet run -p app
```

### Docker instructions:

#### Build it yourself

```
docker build -t storeweb_docker .
docker run -e PORT=80 -it --rm -p 80:80 --name storeweb storeweb_docker
```

##### Using docker compose

```sh
# Start the service
docker-compose up -d

# To stop the service
docker-compose down
```

#### Running latest stable release from Dockerhub

```
docker run -e PORT=80 -it --rm -p 80:80 storedev/store-web
```


#### Dependencies
[StoreLib](https://github.com/StoreDev/StoreLib)


#### License 
[Mozilla Public License](https://www.mozilla.org/en-US/MPL/)
