import { Box, Container, Typography } from "@mui/material";
import { css } from '@emotion/react';
import AutoRefinePortal from "../../features/auto-refine-portal/auto-refine-portal";
import logo from '../../assets/aiNoExperts-logo.png';

const headerStyles = css`
  color: #000;
  padding: 50px 0;
`;

const logoStyles = css`
  background-image: url(${logo});
  background-size: cover;
  height: 200px;
  margin-right: 30px;
  width: 200px;
`;

const renderHeaderLabel = (label: string = '') =>
  <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '50px'}}>{ label }</Typography>;

const Homepage = () => {
  return (
    <Container sx={headerStyles}>
      {/* Header */}
      <Box sx={{ 
        alignItems: 'center',
        display: 'flex',
        justifyContent: 'center',
      }}> 
        {/* Team Logo */}
        <Box css={logoStyles} />
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
