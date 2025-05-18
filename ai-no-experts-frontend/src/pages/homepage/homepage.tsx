import { Box, Container, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import AutoRefinePortal from "../../features/auto-refine-portal/auto-refine-portal";

const Homepage = () => {
    const [logoURL, setLogoURL] = useState("");

    useEffect(() => {
        import(`${process.env.PUBLIC_URL}/assets/aiNoExperts-logo.png`).then((module) => {
            setLogoURL(module.default);
        }).catch((error) => {
            console.error('Error loading logo:', error);
        });
    }, []);

    const renderHeaderLabel = (label: string = '') => 
        <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '50px'}}>{ label }</Typography>;

    return (
        <Container sx={{ color: '#000', padding: '50px 0' }}>
            {/* Header */}
            <Box sx={{ 
                alignItems: 'center',
                display: 'flex',
                justifyContent: 'center',
            }}> 
                {/* Team Logo */}
                <Box sx={{
                    backgroundImage: `url(${logoURL})`,
                    backgroundSize: '100% 100%',
                    height: '200px',
                    marginRight: '30px',
                    width: '200px',
                }} />
                {/* Team Information */}
                <Box>
                    { renderHeaderLabel('GorillaLogic - AI Hackaton') }                
                    { renderHeaderLabel('AiNoExperts') }
                </Box>                                
            </Box>

            {/* Auto Refine Portal */}
            <Box sx={{ marginTop: '50px' }}>
                <AutoRefinePortal />
            </Box>            
        </Container>
    );
}

export default Homepage;

import AutoRefinePortal from "../../features/auto-refine-portal/auto-refine-portal";

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
            }}> 
                {/* Team Logo */}
                <Box sx={{
                    backgroundImage: `url(src/assets/aiNoExperts-logo.png)`,
                    backgroundSize: '100% 100%',
                    height: '200px',
                    marginRight: '30px',
                    width: '200px',
                }} />
                {/* Team Information */}
                <Box>
                    { renderHeaderLabel('GorillaLogic - AI Hackaton') }                
                    { renderHeaderLabel('AiNoExperts') }
                </Box>                                
            </Box>

            {/* Auto Refine Portal */}
            <Box sx={{ marginTop: '50px' }}>
                <AutoRefinePortal />
            </Box>            
        </Container>
    );
}

export default Homepage;