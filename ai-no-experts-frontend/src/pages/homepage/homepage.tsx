import { Box, Container, Typography } from "@mui/material";
import { styled } from '@mui/system';

import AutoRefinePortal from "../../features/auto-refine-portal/auto-refine-portal";

const LogoBox = styled(Box)({
    backgroundImage: `url(src/assets/aiNoExperts-logo.png)`,
    backgroundSize: '100% 100%',
    height: '200px',
    marginRight: '30px',
    width: '200px',
    ariaLabel: 'Team logo'
});

const Homepage = () => {
    const renderHeaderLabel = (label: string = '') => 
        <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '50px'}}>{ label }</Typography>;

    return (
        <Container sx={{ color: '#000', padding: '50px 0' }}>
            {/* Header */}
            <Box sx={{ 
                alignItems: 'center',
                display: 'flex',
                justifyContent: 'center',
                ariaLabel: 'Header Section'
            }}> 
                {/* Team Logo */}
                <LogoBox />
                {/* Team Information */}
                <Box sx={{ ariaLabel: 'Team Information' }}>
                    { renderHeaderLabel('GorillaLogic - AI Hackaton') }                
                    { renderHeaderLabel('AiNoExperts') }
                </Box>                                
            </Box>

            {/* Auto Refine Portal */}
            <Box sx={{ marginTop: '50px', ariaLabel: 'Auto Refine Portal' }}>
                <AutoRefinePortal />
            </Box>            
        </Container>
    );
}

export default Homepage;
