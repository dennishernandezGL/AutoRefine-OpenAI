import { type FunctionComponent } from 'react';
import { Typography } from '@mui/material';
import Backdrop from '@mui/material/Backdrop';
import CircularProgress from '@mui/material/CircularProgress';

// Update to correct the spacing issue and handle props correctly
const Loading: FunctionComponent<LoadingProps> = ({
    isOpen = false,
    label = '',
}) => {
    return (
        <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={isOpen}>
            <CircularProgress color="inherit" />
            {label && <Typography variant='body1'>{label}</Typography>}
        </Backdrop>
    );
}

type LoadingProps = {
    isOpen?: boolean;
    label?: string;
}

export default Loading;
