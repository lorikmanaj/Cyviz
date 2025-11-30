import ReactDOM from "react-dom/client";
import { QueryProvider } from "./providers/QueryProvider";
import { SignalRProvider } from "./providers/SignalRProvider";
import { BrowserRouter } from "react-router-dom";
import { App } from "./App";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <QueryProvider>
    <SignalRProvider>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </SignalRProvider>
  </QueryProvider>
);
