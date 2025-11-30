import { CircularProgress, List, ListItemButton, ListItemText } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useDevices } from "../../hooks/useDevices";
import type { DeviceListDto } from "../../dtos/DeviceListDto";

export const DeviceList = () => {
  const navigate = useNavigate();
  const { data, isLoading } = useDevices();

  if (isLoading) return <CircularProgress />;

  return (
    <List>
      {data?.items.map((device: DeviceListDto) => (
        <ListItemButton
          key={device.id}
          onClick={() => navigate(`/device/${device.id}`)}
        >
          <ListItemText
            primary={device.name}
            secondary={device.status}
          />
        </ListItemButton>
      ))}
    </List>
  );
};
