import type { HubConnection } from "@microsoft/signalr";
import { useEffect, useState, useContext } from "react";
import { LineChart, Line, XAxis, YAxis, Tooltip } from "recharts";
import type { TelemetryPoint } from "../../types/telemetry";

export const TelemetryChart = ({ deviceId }: { deviceId: string }) => {
  const connection = useContext<HubConnection | null>(SignalRContext);
  const [points, setPoints] = useState<TelemetryPoint[]>([]);

  useEffect(() => {
    if (!connection) return;

    connection.invoke("Subscribe", deviceId);

    connection.on("TelemetryUpdated", (payload: TelemetryPoint) => {
      setPoints(prev => [...prev.slice(-20), payload]); // Keep last 20
    });

    return () => {
      connection.invoke("Unsubscribe", deviceId);
    };
  }, [connection, deviceId]);

  return (
    <LineChart width={500} height={300} data={points}>
      <XAxis dataKey="timestamp" />
      <YAxis />
      <Tooltip />
      <Line type="monotone" dataKey="cpu" stroke="#1976d2" />
    </LineChart>
  );
};