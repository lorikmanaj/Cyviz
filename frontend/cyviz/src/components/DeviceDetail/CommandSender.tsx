import {
  Paper,
  Typography,
  Button,
  Stack,
  Box,
  CircularProgress,
} from "@mui/material";

import { useState, useContext, useEffect } from "react";
import { SignalRContext } from "../../providers/SignalRContext";

import { sendCommand, getCommandStatus } from "../../api/commands";
import { newIdempotencyKey } from "../../utils/idempotency";

import type { HubConnection } from "@microsoft/signalr";
import type { CommandStatusDto, CommandResponseDto, CommandSignalREvent } from "../../types/command";

import toast from "react-hot-toast";

export const CommandSender = ({ deviceId }: { deviceId: string }) => {
  const connection = useContext<HubConnection | null>(SignalRContext);

  const [latestCommand, setLatestCommand] = useState<CommandStatusDto | null>(
    null
  );
  const [isSending, setIsSending] = useState(false);

  // Listen for SignalR events
  useEffect(() => {
    if (!connection) return;

    const completedHandler = (msg: CommandSignalREvent) => {
      if (msg.deviceId !== deviceId) return;
      if (!latestCommand || msg.commandId !== latestCommand.commandId) return;

      toast.success("Command completed ⚡");

      setLatestCommand((prev) =>
        prev
          ? {
              ...prev,
              status: "Completed",
              completedUtc: new Date().toISOString(),
            }
          : null
      );
    };

    const failedHandler = (msg: CommandSignalREvent) => {
      if (msg.deviceId !== deviceId) return;
      if (!latestCommand || msg.commandId !== latestCommand.commandId) return;

      toast.error("Command failed ❌");

      setLatestCommand((prev) =>
        prev
          ? {
              ...prev,
              status: "Failed",
              completedUtc: new Date().toISOString(),
            }
          : null
      );
    };

    connection.on("CommandCompleted", completedHandler);
    connection.on("CommandFailed", failedHandler);

    return () => {
      connection.off("CommandCompleted", completedHandler);
      connection.off("CommandFailed", failedHandler);
    };
  }, [connection, latestCommand, deviceId]);

  // Send a command
  const execute = async (commandName: string) => {
    setIsSending(true);

    try {
      const response: CommandResponseDto = await sendCommand(deviceId, {
        command: commandName,
        idempotencyKey: newIdempotencyKey(),
      });

      // Initialize UI as pending
      const status: CommandStatusDto = {
        commandId: response.commandId,
        status: response.status,
        createdUtc: new Date().toISOString(),
        completedUtc: null,
      };

      setLatestCommand(status);

      // Poll fallback (every 1.5s, stop when completed)
      pollStatusUntilDone(response.commandId);
    } catch (err) {
      console.error("Command error:", err);
    } finally {
      setIsSending(false);
    }
  };

  // Poll status fallback
  const pollStatusUntilDone = async (commandId: string) => {
    let done = false;
    while (!done) {
      await new Promise((r) => setTimeout(r, 1500));

      const status = await getCommandStatus(deviceId, commandId);

      setLatestCommand(status);

      if (status.status !== "Pending") {
        done = true;
      }
    }
  };

  return (
    <Paper sx={{ p: 2, mb: 3 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Commands
      </Typography>

      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        <Button
          variant="contained"
          disabled={isSending}
          onClick={() => execute("Ping")}
        >
          Ping
        </Button>

        <Button
          variant="contained"
          color="secondary"
          disabled={isSending}
          onClick={() => execute("GetStatus")}
        >
          Get Status
        </Button>

        <Button
          variant="outlined"
          color="error"
          disabled={isSending}
          onClick={() => execute("Reboot")}
        >
          Reboot
        </Button>
      </Stack>

      {/* Status box */}
      {latestCommand && (
        <Box sx={{ mt: 2 }}>
          <Typography>
            <strong>Command ID:</strong> {latestCommand.commandId}
          </Typography>

          <Typography>
            <strong>Status:</strong>{" "}
            <span
              style={{
                color:
                  latestCommand.status === "Completed"
                    ? "green"
                    : latestCommand.status === "Failed"
                    ? "red"
                    : "orange",
              }}
            >
              {latestCommand.status}
            </span>
          </Typography>

          {latestCommand.status === "Pending" && (
            <CircularProgress size={20} sx={{ mt: 1 }} />
          )}
        </Box>
      )}
    </Paper>
  );
};
