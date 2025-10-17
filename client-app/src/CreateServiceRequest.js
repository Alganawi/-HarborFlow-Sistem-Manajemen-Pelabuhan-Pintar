import React, { useState } from 'react';
import axios from 'axios';
import {
  TextField,
  Button,
  Box,
  Typography,
  Alert,
  Paper
} from '@mui/material';

function CreateServiceRequest({ onServiceRequestCreated }) {
  const [vesselId, setVesselId] = useState('');
  const [serviceType, setServiceType] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!vesselId || !serviceType) {
      setError('Both Vessel ID and Service Type are required.');
      return;
    }

    try {
      const response = await axios.post('/api/servicerequests', { 
        vesselId: parseInt(vesselId), 
        serviceType: serviceType 
      });
      setSuccess('Service request created successfully!');
      setVesselId('');
      setServiceType('');
      if (onServiceRequestCreated) {
        onServiceRequestCreated(response.data);
      }
    } catch (err) {
        if (err.response && err.response.status === 404) {
            setError('Failed to create service request. The specified Vessel ID was not found.');
        } else {
            setError('Failed to create service request. Please try again.');
        }
      console.error(err);
    }
  };

  return (
    <Paper elevation={4} sx={{ p: 3, mt: 2 }}>
      <Typography variant="h6" gutterBottom>Create New Service Request</Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        <TextField
          margin="normal"
          required
          fullWidth
          id="vesselId"
          label="Vessel ID"
          name="vesselId"
          type="number"
          value={vesselId}
          onChange={(e) => setVesselId(e.target.value)}
        />
        <TextField
          margin="normal"
          required
          fullWidth
          id="serviceType"
          label="Service Type (e.g., Cargo Ship, Refueling)"
          name="serviceType"
          value={serviceType}
          onChange={(e) => setServiceType(e.target.value)}
        />
        {error && <Alert severity="error" sx={{ mt: 2 }}>{error}</Alert>}
        {success && <Alert severity="success" sx={{ mt: 2 }}>{success}</Alert>}
        <Button
          type="submit"
          fullWidth
          variant="contained"
          sx={{ mt: 3, mb: 2 }}
        >
          Submit Request
        </Button>
      </Box>
    </Paper>
  );
}

export default CreateServiceRequest;