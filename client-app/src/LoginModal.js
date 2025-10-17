import React, { useState, useEffect } from 'react';
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

function LoginModal({ handleLogin, handleClose, handleShowRegister, error, setError }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  useEffect(() => {
    // Clear error when modal opens
    setError('');
  }, [setError]);

  const onSubmit = (e) => {
    e.preventDefault();
    if (!username || !password) {
      setError("Username and password are required.");
      return;
    }
    handleLogin(username, password);
  };

  return (
    <Dialog open onClose={handleClose} PaperProps={{ component: 'form', onSubmit: onSubmit }}>
      <DialogTitle>Login</DialogTitle>
      <DialogContent>
        <DialogContentText mb={2}>
          Please enter your credentials to log in.
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
        />
        <DialogContentText mt={1}>
          Don't have an account?{' '}
          <Link component="button" type="button" variant="body2" onClick={handleShowRegister}>
            Register here
          </Link>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>Cancel</Button>
        <Button type="submit" variant="contained">Login</Button>
      </DialogActions>
    </Dialog>
  );
}

export default LoginModal;