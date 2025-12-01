import type { HubConnection } from "@microsoft/signalr";
import { useEffect, useState, useContext, useRef } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { SignalRContext } from "../../providers/SignalRContext";
import type {
  TelemetryUpdateEvent,
  TelemetryPayload,
} from "../../types/telemetry";
import { toast } from "react-hot-toast";

// Recharts data point
interface ChartPoint {
  timestamp: number; // numeric timestamp for faster processing
  cpu?: number;
  mem?: number;
  temp?: number;
}

const MAX_MS = 60_000; // keep last 60 seconds

export const TelemetryChart = ({ deviceId }: { deviceId: string }) => {
  const connection = useContext<HubConnection | null>(SignalRContext);
  const [points, setPoints] = useState<ChartPoint[]>([]);

  const lastCleanup = useRef<number>(0);

  useEffect(() => {
    lastCleanup.current = Date.now();
  }, []);

  useEffect(() => {
    if (!connection) return;

    connection.invoke("SubscribeToDevice", deviceId);

    const handler = (payload: TelemetryUpdateEvent) => {
      let parsed: TelemetryPayload = {};

      try {
        parsed = JSON.parse(payload.payload);
      } catch {
        console.warn("Invalid telemetry JSON:", payload.payload);
        toast.error("Invalid telemetry payload ⚠️", {
          duration: 2000
        });
      }

      const timestamp = new Date(payload.timestampUtc).getTime();

      const point: ChartPoint = {
        timestamp,
        ...parsed,
      };

      setPoints((prev) => {
        const now = Date.now();
        const cutoff = now - MAX_MS;

        const filtered = prev.filter((p) => p.timestamp >= cutoff);
        return [...filtered, point];
      });
    };

    connection.on("TelemetryUpdate", handler);

    return () => {
      connection.invoke("UnsubscribeFromDevice", deviceId);
      connection.off("TelemetryUpdate", handler);
    };
  }, [connection, deviceId]);

  const formatTime = (ts: number) => {
    const date = new Date(ts);
    return `${date.getHours()}:${String(date.getMinutes()).padStart(
      2,
      "0"
    )}:${String(date.getSeconds()).padStart(2, "0")}`;
  };

  return (
    <div style={{ width: "100%", height: 300, marginTop: 20 }}>
      <ResponsiveContainer>
        <LineChart data={points}>
          <XAxis
            dataKey="timestamp"
            tickFormatter={formatTime}
            domain={["dataMin", "dataMax"]}
            type="number"
          />
          <YAxis />
          <Tooltip labelFormatter={(label) => formatTime(label as number)} />

          <Line
            type="monotone"
            dataKey="cpu"
            stroke="#1976d2"
            strokeWidth={2}
            dot={false}
            isAnimationActive={false}
          />
          <Line
            type="monotone"
            dataKey="mem"
            stroke="#2e7d32"
            strokeWidth={2}
            dot={false}
            isAnimationActive={false}
          />
          <Line
            type="monotone"
            dataKey="temp"
            stroke="#d32f2f"
            strokeWidth={2}
            dot={false}
            isAnimationActive={false}
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
};
