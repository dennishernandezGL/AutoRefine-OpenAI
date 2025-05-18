import { FC } from 'react';
import { Typography } from '@mui/material';
import Backdrop from '@mui/material/Backdrop';
import CircularProgress from '@mui/material/CircularProgress';

interface LoadingProps {
    isOpen?: boolean;
    label?: string;
}

const Loading: FC<LoadingProps> = ({
    isOpen = false,
    label = '',
}) => {
    return (
        <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={isOpen}>
            <CircularProgress color="inherit" />
            {label && <Typography variant='body1'>{label}</Typography>}
        </Backdrop>
    );
};

export default Loading;