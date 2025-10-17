import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import {
    Box,
    Button,
    Typography,
    Paper,
    Grid,
    CircularProgress,
    Alert,
    Divider,
    List,
    ListItem,
    ListItemIcon,
    ListItemText
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import DescriptionIcon from '@mui/icons-material/Description';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import AddDocumentForm from './AddDocumentForm';

function ServiceRequestDetail({ user, request, onBackToList }) {
    const [documents, setDocuments] = useState([]);
    const [cargo, setCargo] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(true);

    const getStatusText = (status) => {
        return ['Pending', 'Approved', 'Rejected', 'Cancelled', 'Completed'][status] || 'Unknown';
    };

    const fetchDetails = useCallback(async () => {
        if (!request) return;
        setLoading(true);
        try {
            const docsResponse = await axios.get(`/api/servicerequests/${request.serviceRequestID}/documents`);
            setDocuments(docsResponse.data);

            const cargoResponse = await axios.get(`/api/servicerequests/${request.serviceRequestID}/cargo`);
            setCargo(cargoResponse.data);
        } catch (err) {
            setError('Failed to fetch service request details.');
            console.error(err);
        }
        setLoading(false);
    }, [request]);

    useEffect(() => {
        fetchDetails();
    }, [fetchDetails]);

    if (!request) {
        return null;
    }

    const handleDocumentAdded = (newDocument) => {
        setDocuments([...documents, newDocument]);
    };

    const isOwner = user && user.id === request.createdByUserID;

    return (
        <Paper elevation={3} sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h4" component="h1">
                    Service Request Details (ID: {request.serviceRequestID})
                </Typography>
                <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={onBackToList}>
                    Back to List
                </Button>
            </Box>
            <Divider sx={{ mb: 3 }} />

            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center' }}><CircularProgress /></Box>
            ) : error ? (
                <Alert severity="error">{error}</Alert>
            ) : (
                <Grid container spacing={3}>
                    <Grid item xs={12} md={6}>
                        <Typography variant="h6" gutterBottom>Details</Typography>
                        <Typography><strong>Vessel ID:</strong> {request.vesselID}</Typography>
                        <Typography><strong>Service Type:</strong> {request.serviceType}</Typography>
                        <Typography><strong>Status:</strong> {getStatusText(request.status)}</Typography>
                        <Typography><strong>Request Date:</strong> {new Date(request.requestDate).toLocaleString()}</Typography>
                    </Grid>

                    <Grid item xs={12} md={6}>
                        <Typography variant="h6" gutterBottom>Documents</Typography>
                        <List dense>
                            {documents.length > 0 ? documents.map(doc => (
                                <ListItem key={doc.documentID}>
                                    <ListItemIcon>
                                        {doc.isVerified ? <CheckCircleIcon color="success" /> : <DescriptionIcon />}
                                    </ListItemIcon>
                                    <ListItemText primary={doc.documentName} secondary={`Path: ${doc.filePath}`} />
                                </ListItem>
                            )) : <ListItem><ListItemText primary="No documents found." /></ListItem>}
                        </List>
                        {isOwner && <AddDocumentForm serviceRequestId={request.serviceRequestID} onDocumentAdded={handleDocumentAdded} />}
                    </Grid>

                    <Grid item xs={12}>
                       <Divider sx={{ my: 2 }} />
                        <Typography variant="h6" gutterBottom>Cargo</Typography>
                        <List dense>
                            {cargo.length > 0 ? cargo.map(c => (
                                <ListItem key={c.cargoID}>
                                    <ListItemText primary={c.description} secondary={`Weight: ${c.weight} tons - Hazardous: ${c.isHazardous ? 'Yes' : 'No'}`} />
                                </ListItem>
                            )) : <ListItem><ListItemText primary="No cargo found." /></ListItem>}
                        </List>
                    </Grid>
                </Grid>
            )}
        </Paper>
    );
}

export default ServiceRequestDetail;