# KptParser

[![Docker](https://github.com/imato23/kpt-parser/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/imato23/kpt-parser/actions/workflows/docker-publish.yml)

## Introduction

Imports favorites from the [KptCook](https://www.kptncook.com) app into the [Mealie](https://mealie.io) app.

## Installation

### Prerequisites

You need docker and docker-compose on your target system.

### Docker Compose file

Create the following docker compose (`docker-compose.yml`) file:

``` dockerfile
services:
  app:
    image: ghcr.io/imato23/kpt-parser/app:[Version]
    volumes:
      - ./config:/etc/kptparser
    env_file: docker-compose.env
```

You also need a docker compose environment (`docker-compose.env`) file:

``` dockerfile
KPTCOOK_API_URL=https://mobile.kptncook.com
KPTCOOK_API_KEY=[KptCook API key]
KPTCOOK_ACCESS_TOKEN=[KptCook access token]
```
How to obtain an access token and an API key is described at [ephes/kptncook](https://github.com/ephes/kptncook#environment). Many thanks to ephes.

``` dockerfile
MEALIE_API_URL=[API Url of your Mealie instance]
MEALIE_USERNAME=[The username of a Mealie user]
MEALIE_PASSWORD=[The password of the user]
MEALIE_CACHED_IDS_FILENAME=/etc/kptparser/cached-ids.json
```
The file, specified in the environment variable "MEALIE_CACHED_IDS_FILENAME", caches the already imported KptCook identifiers.
The file (can be initial empty) must exist in the path configured as binding in the docker-compose file.




