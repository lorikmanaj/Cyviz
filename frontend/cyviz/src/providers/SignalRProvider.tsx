import { useEffect, useState, type ReactNode } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { SignalRContext } from "./SignalRContext";

export const SignalRProvider = ({ children }: { children: ReactNode }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const url = import.meta.env.VITE_SIGNALR_URL;

    if (!url) {
      console.error("❌ VITE_SIGNALR_URL is missing. Define it in .env");
      return;
    }

    const conn = new HubConnectionBuilder()
      .withUrl(url, {
        withCredentials: true,
        headers: {
          "X-Api-Key": "operator-secret-key",
        },
      })
      .withAutomaticReconnect()
      .build();

    conn
      .start()
      .then(() => {
        console.log("SignalR connected:", url);
        setConnection(conn);
      })
      .catch((err) => console.error("SignalR connection error:", err));

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