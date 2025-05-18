import { Box, Container, Typography } from "@mui/material";
import AutoRefinePortal from "../../features/auto-refine-portal/auto-refine-portal";

const Homepage = () => {
    const renderHeaderLabel = (label: string = '') => (
        <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '50px'}}>{ label }</Typography>
    );

    return (
        <Container sx={{ color: '#000', padding: '50px 0' }}>
            <Box 
                display="flex"
                justifyContent="center"
                alignItems="center"
            >
                <Box 
                    component="img"
                    src="src/assets/aiNoExperts-logo.png"
                    alt="AiNoExperts Logo"
                    height="200px"
                    width="200px"
                    sx={{
                        backgroundSize: '100% 100%',
                        marginRight: '30px',
                    }}
                />
                <Box>
                    { renderHeaderLabel('GorillaLogic - AI Hackaton') }
                    { renderHeaderLabel('AiNoExperts') }
                </Box>
            </Box>
            <Box sx={{ marginTop: '50px' }}>
                <AutoRefinePortal />
            </Box>
        </Container>
    );
};

export default Homepage;
