# coding-test

Project overview

This repository contains a small ASP.NET Core Web API used for fetching Hacker News "best" story IDs, retrieves individual items, caches results, and exposes a simple HTTP endpoint to return the top N stories ordered by score. The implementation is intentionally small to evaluate design, error handling, concurrency, and caching decisions.

Prerequisites
- .NET 10 SDK
- Docker (optional)
- curl or HTTP client

Configuration
Create an appsettings file for the web project (CodingTest). Example: `CodingTest/appsettings.json`

```json
{
  "HackerNewsApi": {
    "BaseUrl": "https://hacker-news.firebaseio.com/v0",
    "BestNewsEndpoint": "/beststories.json",
    "SpecificNewsEndpoint": "/item/{id}.json"
  },
  "Cache": {
    "BestNewsExpirationInSeconds": 60,
    "SpecificNewsExpirationInSeconds": 60
  }
}
```

Run with dotnet SDK
1. From the repository root, restore and build:

   dotnet restore
   dotnet build -c Release

2. Run the web project (example binds to http://localhost:5000):

   PowerShell:
   $env:ASPNETCORE_URLS = "http://localhost:5000"; dotnet run --project CodingTest -c Release

3. Test the endpoint (example: top 10 stories):

   curl http://localhost:5000/News/best/10

Run with Docker
1. Build the image (from repo root):

   docker build -t coding-test -f CodingTest/Dockerfile .

2. Run the container, expose port 8080 and set ASPNETCORE_URLS so Kestrel listens on container port 8080:

   docker run --rm -e ASPNETCORE_URLS="http://+:8080" -p 8080:8080 coding-test

3. Test the endpoint:

   curl http://localhost:8080/News/best/10

Notes
- The project expects HackerNews API configuration under the `HackerNewsApi` section. You can also provide these values via environment variables.
- The first request will take a little more to execute as we hit the external api, after that the caching make it very fast.

Next steps / improvements

- Distributed cache: replace the in-memory cache with a distributed cache (Redis, Memcached) to enable horizontal scaling across multiple instances and keep cache state consistent across replicas.
- Resilience and retries: add transient-fault-handling (Polly) policies—retries with backoff, circuit breaker, and timeouts—for external HTTP calls to Hacker News.
- Pagination: server-side pagination endpoints to reduce request volume.
- Configuration validation: add IOptions validation to fail fast when required settings are missing or invalid.
- Testing: add unit tests for services, integration tests for the API.
- Container & orchestration: add Kubernetes manifests/Helm charts, readiness probes, resource requests/limits, and HPA for horizontal scaling.
