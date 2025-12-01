import { AppBar, Toolbar, Typography, IconButton, Box, Button } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useLocation, useNavigate } from "react-router-dom";

export const Layout = ({ children }: { children: React.ReactNode }) => {
  const location = useLocation();
  const navigate = useNavigate();

  const showBack = location.pathname.startsWith("/device/");

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static" elevation={1} sx={{ mb: 3 }}>
        <Toolbar>
          {showBack && (
            <IconButton edge="start" color="inherit" onClick={() => navigate("/")}>
              <ArrowBackIcon />
            </IconButton>
          )}

          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Cyviz Dashboard
          </Typography>

          <Button color="inherit" onClick={() => navigate("/")}>
            Devices
          </Button>
        </Toolbar>
      </AppBar>

      <Box sx={{ px: 3 }}>{children}</Box>
    </Box>
  );
};
