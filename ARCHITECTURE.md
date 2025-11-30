# 🛠 Backend Setup

## 1. Install Requirements
- .NET 8 SDK
- SQLite
- Node 18+ (for frontend)
- VSCode / Rider / Visual Studio

---

# ⚙️ Configuration

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cyviz.db"
  },
  "ApiKeys": {
    "Device": "device-secret-key",
    "Operator": "operator-secret-key"
  }
}
🗄 Database Setup
Run the migrations:

cd src/backend
dotnet ef migrations add InitialCreate -o Infrastructure/Database/Migrations
dotnet ef database update

▶️ Run the Backend

cd src/backend
dotnet run
Backend starts on:

https://localhost:5001
http://localhost:5000
💻 Run the Frontend

cd frontend
npm install
npm run dev
Frontend runs on:


http://localhost:3000
🔄 Running Device Simulators
A simulator lives in:

/src/backend/Simulators
Run:

dotnet run --project src/backend/Simulators/DeviceSimulator.csproj

Each simulator:

connects to /hubs/device

registers itself using API key

sends heartbeat every 5 sec

sends telemetry every 2 sec

executes commands when received

🧪 API Overview
GET Devices

GET /api/devices?$top=10&$after=device-10&status=Online&type=Codec&search=wall
GET Device Details

GET /api/devices/device-01
Update Device (ETag required)

PUT /api/devices/device-01
If-Match: "base64-rowversion"
Create Command

POST /api/devices/device-01/commands
Body:
{
  "idempotencyKey": "uuid",
  "command": "Ping"
}

🧩 SignalR Endpoints
Device hub (device → backend)

/hubs/device
Methods:
- RegisterDevice(deviceId)
- Heartbeat(deviceId)
- PushTelemetry(deviceId, json, timestamp)
- CommandCompleted(deviceId, commandId, resultJson)

Control hub (operator → backend)

/hubs/control
Methods:
- Subscribe(deviceId)
- Unsubscribe(deviceId)
📊 Metrics & Health

GET /health
GET /metrics
🙌 Contributing
PRs welcome — this architecture is intentionally modular so new protocol adapters, UI pages, or telemetry processors can be added easily.

📄 License
MIT License — free to use, copy, modify.

---

# ✅ **ARCHITECTURE.md (Full Content)**

```md
# Cyviz System Architecture

This document provides a complete overview of the architecture behind the Cyviz control system — including its concurrency model, pipelines, SignalR topology, protocol adapters, telemetry flow, and resilience guarantees.

---

# 🏛 High-Level Architecture Diagram


      ┌───────────────────────┐
      │     React Frontend    │
      │   (Operator Console)  │
      └──────────┬────────────┘
                 │ SignalR (WebSockets)
                 ▼
    ┌─────────────────────────────────────┐
    │         ControlHub (/hubs/control)  │
    └───────────────────┬─────────────────┘
                        │ Commands / Telemetry
                        ▼
┌──────────────────────────────────────────────────┐
│ ASP.NET Core Backend │
│ │
│ Controllers Services Workers Adapters │
│ │ │ │ │ │
└──────┼────────────┼───────────┼──────────┼──────┘
│ │ │ │
▼ ▼ ▼ ▼
Keyset Repo Snapshot Cache Worker N Protocol
(SQLite EF) (IMemoryCache) (hashed) Adapters

---

# 🧩 Domain Design

## **Device**
Tracks:
- id
- name
- type
- protocol
- status
- lastSeenUtc
- firmware
- location
- rowVersion (for ETag)

## **DeviceTelemetry**
Stores ≤50 telemetry records per device.

## **DeviceCommand**
- Idempotency key
- DeviceId
- Status (Pending → Completed/Failed)
- CreatedUtc / CompletedUtc

---

# 🔐 Concurrency Strategy

## 1. **ETag Concurrency Updating**
When updating a device:

If-Match: "base64rowversion"

The service checks:

```csharp
if (!device.RowVersion.SequenceEqual(ifMatchRowVersion))
    throw new PreconditionFailedException();
This prevents:

lost updates

overwrite races

mid-air collisions

2. Command Idempotency
Evidence in code:

var existing = await _commandRepo.GetByIdempotencyAsync(deviceId, key);
if (existing != null) return existing;
Ensures “exactly-once creation”.

3. Consistent Hashing
Each device is always sent to a single worker:

workerIndex = hash(deviceId) % workerCount
Guarantees:

No race on the same device

No double-processing

Ordered execution per device

4. Bounded Command Channels
Each worker has:

Channel<DeviceCommand>(boundedCapacity: 50)
If full:

Reject with HTTP 429

Retry-After header

5. Retry Policy + Jitter
Implemented in:

IRetryPolicy.ExecuteAsync
With timed retries:

100ms + [0..100]

300ms + [0..300]

700ms + [0..700]

6. Circuit Breaker (per device)
States:

Closed → Open → Half-Open

Triggers:

5 consecutive failures

Opens for 10 seconds

🔄 Worker Pipeline
Sequence
Command enqueued into hashed worker channel

Worker receives command

CircuitBreaker.CanExecute()

Send via protocol adapter

RetryPolicy wraps execution

Update command status

Broadcast via ControlHub

📡 SignalR Message Flow
Device → Backend
Heartbeat

Telemetry

CommandCompleted

Backend → Device
ExecuteCommand (via EdgeSignalRAdapter)

Backend → Operator
Telemetry update

CommandCompleted

CommandFailed

Operator → Backend
Subscribe(deviceId)

🧩 Protocol Adapter Architecture
Interface

public interface IDeviceProtocolAdapter {
    Task ConnectAsync(Device d, CancellationToken ct);
    Task<CommandResult> SendCommandAsync(Device d, DeviceCommand c, CancellationToken ct);
    IAsyncEnumerable<Telemetry> StreamTelemetryAsync(Device d, CancellationToken ct);
}
Concrete Adapters
HttpJsonAdapter

TcpLineAdapter

EdgeSignalRAdapter

Adapter Resolver

public IDeviceProtocolAdapter ResolveAdapter(Device device);
🧠 Snapshot Cache
Stores:

DeviceId → LatestTelemetry
Purpose:

Fast device detail pages

Avoid querying SQLite for latest telemetry

Using IMemoryCache:

Sliding expiration: 5 minutes

📅 Keyset Pagination
Cursor-based (no OFFSET).

Example:

GET /api/devices?$top=10&$after=device-10
SQLite queries remain performant even at large datasets.

🚦 API-Key Authentication
Keys:
Device Key → connects to /hubs/device

Operator Key → UI & REST API

Middleware sets:

HttpContext.Items["CallerType"]
Used by authorization attributes:

[DeviceOnly]

[OperatorOnly]

🩺 Health + Metrics

GET /health
GET /metrics
Metrics expose:

command_latency_p95

telemetry_rate

reconnect_count

circuit_breakers_open

📊 Telemetry Flow
Device sends JSON blob via SignalR

Backend appends to SQLite

Keeps only last 50 via TrimHistory()

Publishes update to ControlHub

Stores snapshot in cache

🔧 Technology Choices (Justification)
ASP.NET Core 8
Fastest .NET runtime ever

First-class SignalR support

SQLite
Lightweight embedded DB

Perfect for edge/control systems

Easy migration + local development

SignalR
Automatic reconnect

WebSockets fallback

Broadcast groups (per device)

TanStack Query
Cache-first data fetching

Automatic background refresh

🧪 Testing Strategy
Unit tests for:

Circuit breaker

Retry policy

Pagination logic

Repositories (SQLite in-memory)

Integration tests for:

DeviceHub > Telemetry ingestion

Command Pipeline > Worker > Adapter

Load tests

10,000 telemetry packets/min

500-command burst stress test

📦 Deployment Considerations
Cloud Providers
Azure App Service (enable WebSockets)

AWS Elastic Beanstalk

Kubernetes + NGINX Ingress

SignalR Scaling
Add Redis backplane:

services.AddSignalR().AddStackExchangeRedis("connection");
📚 Future Enhancements
Protocol adapter plugins

Distributed worker queue (Kafka / RabbitMQ)

Role-based operator UI

Device enrollment workflow

Historical telemetry retention in TSDB (InfluxDB / TimescaleDB)

🏁 Summary
The Cyviz backend implements:

Real-time device control

Concurrent-safe command execution

Robust resilience (retry + CB + hashing)

Fast and scalable pagination

A complete WebSocket-based control loop