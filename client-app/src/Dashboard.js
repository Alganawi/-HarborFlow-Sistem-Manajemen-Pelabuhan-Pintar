import React, { useState, useEffect } from 'react';
import axios from 'axios';
import {
  Box,
  Button,
  Typography,
  Paper,
  Grid,
  CircularProgress,
  Alert,
  Collapse,
  Divider,
  Card
} from '@mui/material';
import VesselList from './VesselList';
import CreateServiceRequest from './CreateServiceRequest';
import ServiceRequestList from './ServiceRequestList';
import ServiceRequestDetail from './ServiceRequestDetail';
import DirectionsBoatIcon from '@mui/icons-material/DirectionsBoat';
import ListAltIcon from '@mui/icons-material/ListAlt';
import HourglassEmptyIcon from '@mui/icons-material/HourglassEmpty';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';

// KPI Card Component
const KpiCard = ({ title, value, icon }) => (
  <Card elevation={2} sx={{ display: 'flex', alignItems: 'center', p: 2 }}>
    <Box sx={{ mr: 2, color: 'primary.main' }}>{icon}</Box>
    <Box>
      <Typography variant="h6" component="div">{value}</Typography>
      <Typography color="text.secondary">{title}</Typography>
    </Box>
  </Card>
);

function Dashboard({ user, handleLogout }) {
  const [vessels, setVessels] = useState([]);
  const [serviceRequests, setServiceRequests] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);
  const [showCreateRequest, setShowCreateRequest] = useState(false);
  const [selectedRequest, setSelectedRequest] = useState(null);

  useEffect(() => {
    const fetchVessels = async () => {
      try {
        const response = await axios.get('/api/vessels');
        setVessels(response.data);
      } catch (err) {
        setError('Failed to fetch vessels.');
        console.error(err);
      }
    };

    const fetchServiceRequests = async () => {
      if (!user) {
        setServiceRequests([]);
        return;
      }
      try {
        const response = await axios.get('/api/servicerequests');
        setServiceRequests(response.data);
      } catch (err) {
        console.log('Could not fetch service requests.');
        setServiceRequests([]);
      }
    };

    setLoading(true);
    Promise.all([fetchVessels(), fetchServiceRequests()]).finally(() => setLoading(false));
  }, [user]);

  const handleServiceRequestCreated = (newRequest) => {
    setServiceRequests([newRequest, ...serviceRequests]);
    setShowCreateRequest(false);
  };

  const handleViewDetails = (request) => {
    setSelectedRequest(request);
  };

  const handleBackToList = () => {
    setSelectedRequest(null);
  };

  const isShippingAgent = user && user.role === 0;
  const userRole = user ? ['ShippingAgent', 'PortManager', 'PortStaff', 'FinanceAdmin'][user.role] : 'Guest';

  // KPI Calculations
  const pendingRequests = serviceRequests.filter(r => r.status === 0).length;
  const completedRequests = serviceRequests.filter(r => r.status === 2).length;

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  }

  if (selectedRequest) {
    return <ServiceRequestDetail user={user} request={selectedRequest} onBackToList={handleBackToList} />;
  }

  return (
    <Grid container spacing={3}>
      {/* User Welcome Card */}
      <Grid item xs={12}>
        <Paper elevation={3} sx={{ p: 3 }}>
          {user ? (
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Box>
                <Typography variant="h4" component="h1">Welcome, {user.username}!</Typography>
                <Typography variant="subtitle1" color="text.secondary">Your role is: {userRole}</Typography>
              </Box>
              <Button variant="outlined" color="primary" onClick={handleLogout}>Logout</Button>
            </Box>
          ) : (
            <Box textAlign="center">
              <Typography variant="h4" component="h1">Welcome to HarborFlow</Typography>
              <Typography variant="subtitle1" color="text.secondary">Please log in or register to manage harbor operations.</Typography>
            </Box>
          )}
        </Paper>
      </Grid>

      {/* KPI Cards */}
      {user && (
        <Grid item xs={12} container spacing={3}>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard title="Total Vessels" value={vessels.length} icon={<DirectionsBoatIcon fontSize="large" />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard title="Total Service Requests" value={serviceRequests.length} icon={<ListAltIcon fontSize="large" />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard title="Pending Requests" value={pendingRequests} icon={<HourglassEmptyIcon fontSize="large" />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <KpiCard title="Completed Services" value={completedRequests} icon={<CheckCircleIcon fontSize="large" />} />
          </Grid>
        </Grid>
      )}

      {/* Actions for Shipping Agent */}
      {user && isShippingAgent && (
        <Grid item xs={12}>
          <Paper elevation={1} sx={{ p: 2 }}>
            <Button variant="contained" onClick={() => setShowCreateRequest(!showCreateRequest)}>
              {showCreateRequest ? 'Cancel Request' : 'Create New Service Request'}
            </Button>
            <Collapse in={showCreateRequest} sx={{ mt: 2 }}>
              <CreateServiceRequest onServiceRequestCreated={handleServiceRequestCreated} />
            </Collapse>
          </Paper>
        </Grid>
      )}

      {/* Service Requests List */}
      {user && (
        <Grid item xs={12}>
          <Paper elevation={1} sx={{ p: 2 }}>
            <Typography variant="h5" component="h2" gutterBottom>Service Requests</Typography>
            <Divider sx={{ mb: 2 }} />
            <ServiceRequestList requests={serviceRequests} onViewDetails={handleViewDetails} />
          </Paper>
        </Grid>
      )}

      {/* Vessels List */}
      <Grid item xs={12}>
        <Paper elevation={1} sx={{ p: 2 }}>
          <Typography variant="h5" component="h2" gutterBottom>Available Vessels</Typography>
          <Divider sx={{ mb: 2 }} />
          {error && <Alert severity="error">{error}</Alert>}
          <VesselList vessels={vessels} />
        </Paper>
      </Grid>
    </Grid>
  );
}

export default Dashboard;
