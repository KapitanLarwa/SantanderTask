# HackerNews API

## Overview
This is a **.NET 9 Web API** that retrieves the best stories from **Hacker News**. The API fetches and caches the best stories for performance optimization using **Redis**, ensuring minimal latency and efficient parallel processing.

## Features
- **Asynchronous Processing**: Uses async/await for optimal performance.
- **Redis Caching**: Stores results for 5 minutes to reduce API calls.
- **Parallel Processing**: Fetches multiple stories simultaneously.
- **Unity Dependency Injection**: Ensures modularity and testability.
- **Swagger Integration**: API documentation is automatically generated.
- **Unit Tests**: Tests done using **XUnit** and **Moq**.

## Prerequisites
- .NET 9 SDK
- Redis (via Docker)
- Docker (for running Redis)

## Running Redis (Using Docker)
```sh
docker run --name redis -d -p 6379:6379 redis
```

## Running the API
1. **Clone the Repository**
   ```sh
   git clone https://github.com/KapitanLarwa/SantanderTask
   ```

2. **Set Up Environment Variables (Optional)**
   If needed, modify `appsettings.json` to set the Redis connection string.

3. **Run the Application**
   ```sh
   dotnet run --project HackerNewsApi
   ```
   ```sh
   dotnet run --project HackerNewsApi --urls=https://localhost:7229   
   ```

   The API will be available at `http://localhost:5024` (or `https://localhost:7229` for HTTPS).

4. **Access Swagger UI**
   Visit: `https://localhost:7229/swagger/index.html` or `http://localhost:5024/swagger/index.html` for http version

## API Endpoints
### Get Best Stories
```
GET /api/BestStories/{count}
```
- **Description**: Fetches the top best stories from Hacker News.
- **Response Example:**
  ```json
  [
    {
      "title": "A uBlock Origin update was rejected from the Chrome Web Store",
      "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
      "postedBy": "ismaildonmez",
      "time": "2019-10-12T13:43:01+00:00",
      "score": 1716,
      "commentCount": 572
    }
  ]
  ```

## Assumptions
- **Stories with missing data** (e.g., title or URL) will be excluded.
- **Cache expiration is 5 minutes**, ensuring updated results within reasonable time.
- **API response sorting is based on score**, in descending order.

## Enhancements (Given More Time)
- Implement **Rate Limiting** to prevent API abuse.
- Introduce **Database Persistence** for historical data tracking.
- Optimize Redis caching strategy with **Sliding Expiration**.
- Implement **Circuit Breaker** pattern to handle API failures.
- Add logging 
- moving DI out of Program.cs

## Running Unit Tests
```sh
dotnet test
```
- **Test Cases**:
  - Cache retrieval.
  - Fallback to API when cache is empty.
  - Error handling when API is unavailable.

## Conclusion
This project demonstrates high-performance API development using .NET 9, Redis caching, and parallel processing.

