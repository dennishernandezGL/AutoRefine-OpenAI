import { Snackbar, type SnackbarOrigin, Alert } from "@mui/material";
import { type FunctionComponent, useState, useEffect } from "react";

const SnackbarComponent: FunctionComponent<SnackbarComponentProps> = ({
    autoHideDuration = 6000,
    horizontalPosition = 'center',
    message = '', 
    open = false, 
    severity = 'error',
    verticalPosition = 'bottom',
    onClose = () => {} // Add default function to prevent errors if not provided
}) => {
  const [isOpen, setIsOpen] = useState(open);

  useEffect(() => {
    setIsOpen(open);
  }, [open]);

  const handleClose = (_event?: React.SyntheticEvent | Event, reason?: string) => {
    if (reason === 'clickaway') return;
    setIsOpen(false);
    // Safely call onClose if it's a function
    if (typeof onClose === 'function') onClose();
  };

  return (
    <Snackbar
      open={isOpen}
      autoHideDuration={autoHideDuration}
      onClose={handleClose}
      anchorOrigin={{ vertical: verticalPosition, horizontal: horizontalPosition }}
    >
      <Alert onClose={handleClose} severity={severity} sx={{ width: "100%" }}>
        {message}
      </Alert>
    </Snackbar>
  );
};

interface SnackbarComponentProps {
    autoHideDuration?: number;
    horizontalPosition?: SnackbarOrigin['horizontal'];
    message?: string;
    open: boolean;
    severity?: 'error' | 'success' | 'info' | 'warning';
    verticalPosition?: SnackbarOrigin['vertical'];
    onClose?: () => void; // Mark onClose as optional
}

export default SnackbarComponent;