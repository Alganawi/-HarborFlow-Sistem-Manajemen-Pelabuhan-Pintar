import React, { useState } from 'react';
import {
  Button,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Link,
  Alert
} from '@mui/material';

function RegisterModal({ handleRegister, handleClose, handleShowLogin }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const onSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (password.length < 8) {
      setError('Password must be at least 8 characters long.');
      return;
    }
    if (username.length < 3) {
        setError('Username must be at least 3 characters long.');
        return;
    }

    try {
      await handleRegister(username, password);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <Dialog open onClose={handleClose} PaperProps={{ component: 'form', onSubmit: onSubmit }}>
      <DialogTitle>Register</DialogTitle>
      <DialogContent>
        <DialogContentText mb={2}>
          Create a new account to access HarborFlow.
        </DialogContentText>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <TextField
          autoFocus
          required
          margin="dense"
          id="username"
          name="username"
          label="Username"
          type="text"
          fullWidth
          variant="outlined"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          helperText="Must be at least 3 characters."
        />
        <TextField
          required
          margin="dense"
          id="password"
          name="password"
          label="Password"
          type="password"
          fullWidth
          variant="outlined"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          helperText="Must be at least 8 characters."
        />
        <DialogContentText mt={1}>
          Already have an account?{' '}
          <Link component="button" type="button" variant="body2" onClick={handleShowLogin}>
            Login here
          </Link>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button type="submit" variant="contained">Register</Button>
      </DialogActions>
    </Dialog>
  );
}

export default RegisterModal;