import { Box, Container, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import { type FunctionComponent, memo } from 'react';

import type { Recommendation } from '../../models/log.model';

const Recommendations: FunctionComponent<RecommendationsProps> = memo(({
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
                key={recommendation.field}
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
);

interface RecommendationsProps {
  recommendations?: Recommendation[];
}

export default Recommendations;