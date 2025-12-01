# Cyviz

# Cyviz Mini Monitoring & Management Platform

A distributed SignalR-based control system for managing and monitoring simulated edge devices that sit behind firewalls/NATs.

This project implements the core building blocks of the Cyviz interview challenge:

- Device inventory + telemetry + command API
- Command routing with retries, circuit breakers, idempotency
- Edge-initiated SignalR connections
- React dashboard with live updates
- SQLite persistence with row-version ETag concurrency
- Folder-based Clean Architecture layout

Some stretch goals (OpenTelemetry, chaos testing, full adapter implementations) are described but not fully implemented — details are marked throughout the documentation.

## For Architectural information, visit the Architure.md - 👉 [ARCHITECTURE.md](ARCHITECTURE.md)

## 📁 Project Structure

/src
/backend
/Api
/Application
/Domain
/Infrastructure
/SignalR
/Tests
/frontend
README.md
ARCHITECTURE.md

> **Note on folder-based architecture**  
> This project uses _folder-level_ domain / application / infrastructure boundaries.  
> In a fully-grown enterprise solution, these would become **separate class library projects**.  
> Due to the short timebox of this challenge, the same structure is preserved but simplified into folders to simulate what a real Clean Architecture/DDD decomposition would evolve into.

---

# 🚀 How to Run the Backend (ASP.NET Core 8)

### Requirements

- .NET 8 SDK
- No database setup required — SQLite file is created automatically
- Migrations run on application startup

### Start the API

```bash
cd src/backend/Api
dotnet run
By default the API will choose an available port (e.g. http://localhost:7622).

You can explicitly set the port:

dotnet run --urls "http://localhost:5001"
API Keys
The backend uses simple API key authentication:

Caller	Header	Value Example
Operator UI	X-Api-Key	operator-key-123
Device Edge	X-Device-Key	device-key-abc

The keys are defined in appsettings.json.

🖥️ How to Run the Frontend (React + Vite)
Requirements
Node 18+

NPM 9+

Start the Client

cd src/frontend
npm install
npm run dev
Important:
The frontend uses .env variables for API + SignalR URLs:

VITE_API_URL=http://localhost:7622/api
VITE_SIGNALR_URL=http://localhost:7622/controlhub
Your port may differ depending on .NET's dynamic port assignment.

⚙️ Chaos Testing (Partially Implemented)
The challenge recommends testing the system under latency and drop simulation.

This project includes stubs for chaos flags:

ini
Copy code
CHAOS_LATENCY=1.0-2.0s
CHAOS_DROP_RATE=0.1
Expected Behavior (Design-level)
Even if not fully wired throughout the application, enabling chaos should result in:

Random latency injections

Random message drop (e.g. 10%)

Circuit breaker transitions

Retry policy activation

System remains stable without deadlocks or crashes

Chaos handling is documented in ARCHITECTURE.md but not fully completed in code.

🧪 Testing
Unit Tests
Located under src/backend/Tests

Includes:

RetryPolicy tests

Idempotency checks

Basic domain logic tests

Integration Tests
Stubs included following the expected structure:

WebApplicationFactory based tests

Mocked SignalR scenarios depending on timebox

📜 Endpoints Overview
Method	Route	Description
GET	/api/devices	Keyset-paginated list of devices
GET	/api/devices/{id}	Device details + latest telemetry snapshot
PUT	/api/devices/{id}	Update device metadata (ETag required)
POST	/api/devices/{id}/commands	Enqueue device command (idempotency required)
GET	/api/devices/{id}/commands/{commandId}	Retrieve command status

More details in ARCHITECTURE.md.

```

For Architectural information, visit the Architure.md - 👉 [ARCHITECTURE.md](ARCHITECTURE.md)
