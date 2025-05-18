import { Snackbar, type SnackbarOrigin, Alert } from "@mui/material";
import { type FunctionComponent, useState, useEffect, type SyntheticEvent } from "react";

const SnackbarComponent: FunctionComponent<SnackbarComponentProps> = ({
    autoHideDuration = 6000,
    horizontalPosition = 'center',
    message = '', 
    open = false, 
    severity = 'error',
    verticalPosition = 'bottom',
    onClose 
}) => {
  const [isOpen, setIsOpen] = useState(open);

  useEffect(() => {
    setIsOpen(open);
  }, [open]);

  const handleClose = (event?: SyntheticEvent | Event, reason?: string) => {
    if (reason === 'clickaway') return;
    setIsOpen(false);
    onClose?.();
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
    onClose?: () => void;
}

export default SnackbarComponent;
