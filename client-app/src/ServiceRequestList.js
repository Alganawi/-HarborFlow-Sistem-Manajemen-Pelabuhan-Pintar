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
  Box,
  Button
} from '@mui/material';

function ServiceRequestList({ requests, onViewDetails }) {
  if (!requests || requests.length === 0) {
    return (
      <Box sx={{ my: 2 }}>
        <Typography color="text.secondary">No service requests found.</Typography>
      </Box>
    );
  }

  const getStatusText = (status) => {
    switch (status) {
      case 0:
        return 'Pending';
      case 1:
        return 'Approved';
      case 2:
        return 'Completed';
      case 3:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  };

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table sx={{ minWidth: 650 }} aria-label="service requests table">
        <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
          <TableRow>
            <TableCell>Request ID</TableCell>
            <TableCell>Vessel ID</TableCell>
            <TableCell>Service Type</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Request Date</TableCell>
            <TableCell align="right">Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {requests.map((request) => (
            <TableRow key={request.serviceRequestID}>
              <TableCell component="th" scope="row">
                {request.serviceRequestID}
              </TableCell>
              <TableCell>{request.vesselID}</TableCell>
              <TableCell>{request.serviceType}</TableCell>
              <TableCell>{getStatusText(request.status)}</TableCell>
              <TableCell>{new Date(request.requestDate).toLocaleDateString()}</TableCell>
              <TableCell align="right">
                <Button variant="text" size="small" onClick={() => onViewDetails(request)}>
                  View Details
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default ServiceRequestList;