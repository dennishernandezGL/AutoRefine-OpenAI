import { Box, Container, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import { type FunctionComponent } from 'react';
import PropTypes from 'prop-types';
import type { Recommendation } from '../../models/log.model';

const Recommendations: FunctionComponent<RecommendationsProps> = ({
    recommendations = [],
}) => {
  return (
    <Container>
      {/* Title */}
      <Box sx={{ marginBottom: '30px' }}>
        <Typography component={'h1'} sx={{ fontFamily: 'Agdasima', fontSize: '30px'}}>AI Recommendations:</Typography>
      </Box>
      
      {/* Table */}
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="Recommendations Table">
          {/* Header */}
          <TableHead>
            <TableRow>
              <TableCell>Field</TableCell>
              <TableCell align="right">Type</TableCell>
              <TableCell align="right">Justification</TableCell>
            </TableRow>
          </TableHead>
          {/* Body */}
          <TableBody>
            {recommendations.map((recommendation: Recommendation) => (
              <TableRow
                key={recommendation.field ? recommendation.field : Math.random()} // Add a fallback for key
                sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
              >
                <TableCell component="th" scope="row">{recommendation.field}</TableCell>
                <TableCell align="right">{recommendation.type}</TableCell>
                <TableCell align="right">{recommendation.justification}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>    
  );
}

interface RecommendationsProps {
  recommendations?: Recommendation[];
}

Recommendations.propTypes = {
  recommendations: PropTypes.arrayOf(PropTypes.shape({
    field: PropTypes.string.isRequired,
    type: PropTypes.string.isRequired,
    justification: PropTypes.string.isRequired
  }))
};

export default Recommendations;
