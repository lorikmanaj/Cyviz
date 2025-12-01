import { useParams } from "react-router-dom";
import { CircularProgress, Typography, Box } from "@mui/material";
import { DeviceDetailsPanel } from "../components/DeviceDetail/DeviceDetailsPanel";
import { CommandSender } from "../components/DeviceDetail/CommandSender";
import { TelemetryChart } from "../components/TelemetryChart/TelemetryChart";
import { useDeviceDetails } from "../hooks/useDeviceDetail";

export const DevicePage = () => {
    const { id } = useParams<{ id: string }>();

    const { data, isLoading, error } = useDeviceDetails(id);

    if (!id) return <Typography>No device ID provided.</Typography>;
    if (isLoading) return <CircularProgress />;
    if (error) return <Typography>Error loading device.</Typography>;
    if (!data) return <Typography>No device found.</Typography>;

  return (
    <Box p={3}>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Device: {data.name}
      </Typography>

      {/* Metadata panel */}
      <DeviceDetailsPanel device={data} />

      {/* Telemetry live chart */}
      <TelemetryChart deviceId={data.id} />

      {/* Command sender */}
      <CommandSender deviceId={data.id} />
    </Box>
  );
};
