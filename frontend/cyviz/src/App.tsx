import { Routes, Route } from "react-router-dom";
import { Dashboard } from "./pages/Dashboard";
import { DevicePage } from "./pages/DevicePage";

export const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/device/:id" element={<DevicePage />} />
    </Routes>
  );
};
