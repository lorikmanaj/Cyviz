import { useDeviceDetail } from "../../hooks/useDeviceDetail";
import { useParams } from "react-router-dom";
import { CircularProgress, Paper, Typography } from "@mui/material";
import { TelemetryChart } from "../TelemetryChart/TelemetryChart";

export const DeviceDetail = () => {
  const { id } = useParams();
  const { data, isLoading } = useDeviceDetail(id!);

  if (isLoading) return <CircularProgress />;
  if (!data) return <div>Device not found</div>;

  return (
    <Paper sx={{ padding: 2 }}>
      <Typography variant="h5">{data.name}</Typography>
      <Typography>Status: {data.status}</Typography>
      <Typography>Last Seen: {data.lastSeenUtc}</Typography>

      <TelemetryChart deviceId={data.id} />
    </Paper>
  );
};
