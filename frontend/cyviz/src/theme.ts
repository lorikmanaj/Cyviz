import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
    palette: {
        mode: "light",
        primary: {
            main: "#6366f1", // indigo-500
        },
        secondary: {
            main: "#ec4899", // pink-500
        },
        error: {
            main: "#ef4444", // red-500
        },
        warning: {
            main: "#f59e0b", // amber-500
        },
        success: {
            main: "#22c55e", // green-500
        },
    },
    components: {
        MuiButton: {
            styleOverrides: {
                root: {
                    textTransform: "none",
                    fontWeight: 600,
                    borderRadius: 8,
                },
            },
        },
        MuiPaper: {
            styleOverrides: {
                root: {
                    borderRadius: 10,
                },
            },
        },
    },
});
