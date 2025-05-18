import { Snackbar, type SnackbarOrigin, Alert } from "@mui/material";
import { type FunctionComponent, useState, useEffect } from "react";

const SnackbarComponent: FunctionComponent<SnackbarComponentProps> = ({
    autoHideDuration = 6000,
    horizontalPosition = "center",
    message = "",
    open = false,
    severity = "error",
    verticalPosition = "bottom",
    onClose,
}) => {
    const [isOpen, setIsOpen] = useState(open);

    useEffect(() => {
        setIsOpen(open);
    }, [open]);

    const handleClose = (
        _event?: React.SyntheticEvent | Event,
        reason?: string
    ) => {
        if (reason === "clickaway") return;
        setIsOpen(false);
        if (onClose) onClose();
    };

    return (
        <Snackbar
            open={isOpen}
            autoHideDuration={autoHideDuration}
            onClose={handleClose}
            anchorOrigin={{
                vertical: verticalPosition,
                horizontal: horizontalPosition,
            }}
            message={<Alert severity={severity} sx={{ width: "100%" }}>{message}</Alert>}
        />
    );
};

interface SnackbarComponentProps {
    autoHideDuration?: number;
    horizontalPosition?: SnackbarOrigin["horizontal"];
    message?: string;
    open: boolean;
    severity?: "error" | "success" | "info" | "warning";
    verticalPosition?: SnackbarOrigin["vertical"];
    onClose?: () => void;
}

export default SnackbarComponent;
