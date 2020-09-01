# StoreWeb

[![GitHub stars](https://img.shields.io/github/stars/StoreDev/StoreWeb?style=social)](https://github.com/StoreDev/StoreWeb)
[![GitHub Workflow - Docker](https://img.shields.io/github/workflow/status/StoreDev/StoreWeb/docker?label=docker)](https://github.com/StoreDev/StoreWeb/actions?query=workflow%3Adocker)
[![GitHub Workflow - Build](https://img.shields.io/github/workflow/status/StoreDev/StoreWeb/build?label=build)](https://github.com/StoreDev/StoreWeb/actions?query=workflow%3Abuild)
[![GitHub Workflow - Deploy](https://img.shields.io/github/workflow/status/StoreDev/StoreWeb/heroku?label=Deploy+to+heroku)](https://github.com/StoreDev/StoreWeb/actions?query=workflow%3A%22Deploy+to+heroku%22)
[![Docker Pulls](https://img.shields.io/docker/pulls/storedev/store-web)](https://hub.docker.com/r/storedev/store-web)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/storedev/storeweb)](https://github.com/StoreDev/StoreWeb/releases)

StoreWeb is a webapp that makes use of [StoreLib](https://github.com/StoreDev/StoreLib).


## Usage

Clone the repo and build StoreWeb using either Visual Studio 2019, VS Code or dotnet CLI.

### Build

```sh
dotnet build
```

### Run

Run StoreWeb.dll using the ASP.NET Core 3.1 runtime

```
dotnet run
```

### Docker instructions:

#### Build it yourself

```
docker build -t storeweb_docker .
docker run -it --rm -p 8080:80 --name storeweb storeweb_docker
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
docker run -it --rm -p 8080:80 storedev/store-web
```


#### Dependencies
[StoreLib](https://github.com/StoreDev/StoreLib)


#### License 
[Mozilla Public License](https://www.mozilla.org/en-US/MPL/)
