# Cyviz
# Cyviz Control System – Backend & Frontend

A full-stack system for controlling distributed AV/IoT devices through a resilient,
fault-tolerant backend with real-time telemetry and command execution.

This repository contains:

- **Backend** (ASP.NET Core 8, SQLite, SignalR, EF Core)
- **Frontend** (React 18 + Vite + TypeScript + MUI + TanStack Query)
- **Real-time Operator UI** (SignalR)
- **Edge Device Simulators** (WebSocket-based pseudo-device clients)

---

# 🚀 Features

### **Backend**
- Device CRUD + keyset pagination (`$after`, `$top`)
- Idempotent command execution (`idempotencyKey`)
- Background workers with:
  - Command queues (bounded, non-blocking)
  - Consistent hashing for device sharding
  - Retry policy with jitter (100ms → 300ms → 700ms)
  - Per-device circuit breakers
- SignalR hubs:
  - `/hubs/device` – edge → backend
  - `/hubs/control` – operator → backend
- Device snapshot cache (in-memory)
- Telemetry ingestion + retention (≤ 50 items/device)
- API-key authentication (device/operator keys)
- SQLite database with EF migrations
- Serilog structured logging

### **Frontend**
- React + TypeScript + Vite
- Material UI (MUI)
- TanStack Query for data fetching + caching
- SignalR client for real-time updates
- Charts via Recharts
- Device dashboard, telemetry, and command panel

### **Edge Simulators**
- Simulated devices sending:
  - Heartbeats (5s)
  - Telemetry (2s)
  - SignalR-based bidirectional command execution
- Multiple devices can be run concurrently