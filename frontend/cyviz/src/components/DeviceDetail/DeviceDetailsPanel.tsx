import { Paper, Typography, Box, Chip, Stack } from "@mui/material";
import dayjs from "dayjs";
import type { DeviceDetailDto } from "../../types/device";

export const DeviceDetailsPanel = ({ device }: { device: DeviceDetailDto }) => {
  const parsedTelemetry = (() => {
    try {
      if (!device.latestTelemetry?.dataJson) return null;
      return JSON.parse(device.latestTelemetry.dataJson);
    } catch {
      return { error: "Invalid telemetry JSON" };
    }
  })();

  return (
    <Paper sx={{ p: 2, mb: 3 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Device Information
      </Typography>

      <Box sx={{ mb: 1 }}>
        <strong>Name:</strong> {device.name}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Type:</strong> {device.type}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Status:</strong> {device.status}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Protocol:</strong> {device.protocol}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Firmware:</strong> {device.firmware}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Location:</strong> {device.location}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Last Seen:</strong>{" "}
        {dayjs(device.lastSeenUtc).format("YYYY-MM-DD HH:mm:ss")}
      </Box>

      <Box sx={{ mb: 1 }}>
        <strong>Capabilities:</strong>
        <Stack direction="row" spacing={1} sx={{ mt: 1, flexWrap: "wrap" }}>
          {device.capabilities.map((cap) => (
            <Chip key={cap} label={cap} size="small" sx={{ mr: 1, mb: 1 }} />
          ))}
        </Stack>
      </Box>

      {parsedTelemetry && (
        <Box sx={{ mt: 2 }}>
          <strong>Latest Telemetry:</strong>
          <pre
            style={{
              background: "#f5f5f5",
              padding: 8,
              borderRadius: 4,
              marginTop: 8,
            }}
          >
            {JSON.stringify(parsedTelemetry, null, 2)}
          </pre>
        </Box>
      )}
    </Paper>
  );
};
