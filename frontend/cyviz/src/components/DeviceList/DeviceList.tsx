import {
  CircularProgress,
  List,
  ListItemButton,
  ListItemText,
  Typography,
  Button,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useDevices } from "../../hooks/useDevices";
import dayjs from "dayjs";
import type { DeviceListDto } from "../../types/device";

export const DeviceList = () => {
  const navigate = useNavigate();
  const { data, fetchNextPage, hasNextPage, isLoading, isFetchingNextPage } =
    useDevices();

  if (isLoading) return <CircularProgress />;

  return (
    <div style={{ padding: 16 }}>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Devices
      </Typography>

      <List>
        {data?.pages.map((page) =>
          page.items.map((d: DeviceListDto) => (
            <ListItemButton
              key={d.id}
              onClick={() => navigate(`/device/${d.id}`)}
            >
              <ListItemText
                primary={`${d.name} (${d.type})`}
                secondary={`Status: ${d.status} — Last seen: ${dayjs(
                  d.lastSeenUtc
                ).format("YYYY-MM-DD HH:mm:ss")}`}
              />
            </ListItemButton>
          ))
        )}
      </List>

      {hasNextPage && (
        <Button
          variant="outlined"
          onClick={() => fetchNextPage()}
          disabled={isFetchingNextPage}
        >
          {isFetchingNextPage ? "Loading..." : "Load More"}
        </Button>
      )}
    </div>
  );
};
