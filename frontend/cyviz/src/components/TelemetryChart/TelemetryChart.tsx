import type { HubConnection } from "@microsoft/signalr";
import { useEffect, useState, useContext } from "react";
import { LineChart, Line, XAxis, YAxis, Tooltip } from "recharts";
import { SignalRContext } from "../../providers/SignalRContext";

import type {
  TelemetryUpdateEvent,
  TelemetryPayload,
} from "../../types/telemetry";

interface ChartPoint {
  timestampUtc: string;
  cpu?: number;
  mem?: number;
  temp?: number;
}

export const TelemetryChart = ({ deviceId }: { deviceId: string }) => {
  const connection = useContext<HubConnection | null>(SignalRContext);
  const [points, setPoints] = useState<ChartPoint[]>([]);

  useEffect(() => {
    if (!connection) return;

    // Subscribe to backend hub
    connection.invoke("SubscribeToDevice", deviceId);

    const handler = (payload: TelemetryUpdateEvent) => {
      let parsed: TelemetryPayload = {};

      try {
        parsed = JSON.parse(payload.payload);
      } catch {
        console.warn("Invalid telemetry JSON:", payload.payload);
      }

      const point: ChartPoint = {
        timestampUtc: payload.timestampUtc,
        ...parsed,
      };

      setPoints((prev) => [...prev.slice(-20), point]); // keep last 20
    };

    connection.on("TelemetryUpdate", handler);

    return () => {
      connection.invoke("UnsubscribeFromDevice", deviceId);
      connection.off("TelemetryUpdate", handler);
    };
  }, [connection, deviceId]);

  return (
    <LineChart width={600} height={300} data={points}>
      <XAxis dataKey="timestampUtc" />
      <YAxis />
      <Tooltip />
      <Line type="monotone" dataKey="cpu" stroke="#1976d2" />
      <Line type="monotone" dataKey="mem" stroke="#2e7d32" />
      <Line type="monotone" dataKey="temp" stroke="#d32f2f" />
    </LineChart>
  );
};
