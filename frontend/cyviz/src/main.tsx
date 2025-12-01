import React from "react";
import ReactDOM from "react-dom/client";
import { ThemeProvider, CssBaseline } from "@mui/material";
import { theme } from "./theme";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { SignalRProvider } from "./providers/SignalRProvider";
import App from "./App";
import { Toaster } from "react-hot-toast";

const qc = new QueryClient();

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <QueryClientProvider client={qc}>
        <SignalRProvider>
            <Toaster position="bottom-right" />
          <App />
        </SignalRProvider>
      </QueryClientProvider>
    </ThemeProvider>
  </React.StrictMode>
);
