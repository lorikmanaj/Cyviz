import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect, useState } from "react";

export const SignalRProvider = ({ children }: { children: React.ReactNode }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl("http://localhost:7266/hubs/control", {
        headers: { "X-Api-Key": "operator-secret-key" },
      })
      .withAutomaticReconnect()
      .build();

    conn.start().catch(console.error);
    setConnection(conn);

    return () => {
      conn.stop();
    };
  }, []);

  return (
    <SignalRContext.Provider value={connection}>
      {children}
    </SignalRContext.Provider>
  );
};
