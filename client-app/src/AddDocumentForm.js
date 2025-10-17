import React, { useState } from 'react';
import axios from 'axios';
import {
    Box,
    TextField,
    Button,
    Alert,
    Typography
} from '@mui/material';

function AddDocumentForm({ serviceRequestId, onDocumentAdded }) {
    const [documentName, setDocumentName] = useState('');
    const [filePath, setFilePath] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        if (!documentName || !filePath) {
            setError('Document Name and File Path are required.');
            return;
        }

        try {
            const response = await axios.post(`/api/servicerequests/${serviceRequestId}/documents`, { documentName, filePath });
            setSuccess('Document added successfully!');
            setDocumentName('');
            setFilePath('');
            if (onDocumentAdded) {
                onDocumentAdded(response.data);
            }
        } catch (err) {
            setError('Failed to add document.');
            console.error(err);
        }
    };

    return (
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2, p: 2, border: '1px dashed grey', borderRadius: 1 }}>
            <Typography variant="subtitle1" gutterBottom>Add New Document</Typography>
            <TextField
                margin="dense"
                required
                fullWidth
                id="documentName"
                label="Document Name"
                name="documentName"
                value={documentName}
                onChange={(e) => setDocumentName(e.target.value)}
            />
            <TextField
                margin="dense"
                required
                fullWidth
                id="filePath"
                label="File Path"
                name="filePath"
                placeholder="e.g., /docs/manifest.pdf"
                value={filePath}
                onChange={(e) => setFilePath(e.target.value)}
            />
            {error && <Alert severity="error" sx={{ mt: 2 }}>{error}</Alert>}
            {success && <Alert severity="success" sx={{ mt: 2 }}>{success}</Alert>}
            <Button
                type="submit"
                variant="contained"
                size="small"
                sx={{ mt: 2 }}
            >
                Add Document
            </Button>
        </Box>
    );
}

export default AddDocumentForm;