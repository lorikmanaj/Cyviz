import { BrowserRouter, Routes, Route } from "react-router-dom";
import { DevicePage } from "./pages/DevicePage";
import { Dashboard } from "./pages/Dashboard";
import { Layout } from "./layout/Layout";

export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/device/:id" element={<DevicePage />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}
