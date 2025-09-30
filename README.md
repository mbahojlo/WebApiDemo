# WebApiDemo

A .NET 8 Web API for calculating provider quotes based on topic values.  
Features Swagger UI for API exploration and comprehensive unit tests.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (recommended) or any compatible IDE

## Getting Started

### 1. Clone the repository

### 2. Configuration

#### Provider Topics

Create or edit `Config/config.json` to define provider-topic mappings.  

#### App Settings

Default `appsettings.json` is sufficient for development.

### 3. Build and Run

#### Using Visual Studio

- Open the solution in Visual Studio 2022.
- Press **F5** or select __Debug > Start Debugging__.

### 4. API Documentation

## API Usage

### Endpoint

`POST /api/demo/quotes`

- Simple request body will pop up in swagger UI

### Response

- The response contains calculated quotes for each provider based on the submitted topic values and the configuration in `config.json`.

## Running Tests

## Project Structure

- `Controllers/` - API controllers
- `Models/` - Data models and examples
- `Services/` - Business logic and quote calculation
- `Middleware/` - Custom middleware (e.g., request/response logging)
- `Config/config.json` - Provider/topic configuration
- `WebApiDemo.Tests/` - Unit tests
- `Logs/Log*.txt` - Logs from app execution