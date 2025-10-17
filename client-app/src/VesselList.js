import React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Typography,
  Box
} from '@mui/material';

function VesselList({ vessels }) {
  if (!vessels || vessels.length === 0) {
    return (
      <Box sx={{ my: 2 }}>
        <Typography color="text.secondary">No vessels found or still loading...</Typography>
      </Box>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table sx={{ minWidth: 650 }} aria-label="simple table">
        <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
          <TableRow>
            <TableCell sx={{ fontWeight: 'bold' }}>Vessel ID</TableCell>
            <TableCell>Name</TableCell>
            <TableCell>IMO Number</TableCell>
            <TableCell>Type</TableCell>
            <TableCell align="right">Capacity (DWT)</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {vessels.map((vessel) => (
            <TableRow
              key={vessel.vesselID}
              sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
            >
              <TableCell component="th" scope="row">
                {vessel.vesselID}
              </TableCell>
              <TableCell>
                {vessel.vesselName} 
              </TableCell>
              <TableCell>{vessel.imoNumber}</TableCell>
              <TableCell>{vessel.type}</TableCell>
              <TableCell align="right">{vessel.capacity}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default VesselList;