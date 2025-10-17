import React, { useState, useEffect } from 'react';
import axios from 'axios';
import {
  CssBaseline,
  ThemeProvider,
  createTheme,
  Container,
  Box,
  AppBar,
  Toolbar,
  Typography,
  Button
} from '@mui/material';
import SailingIcon from '@mui/icons-material/Sailing';
import './App.css';
import Dashboard from './Dashboard';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';

import backgroundImage from './assets/background.jpg';

// Create a theme instance.
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2', // A professional blue
    },
    secondary: {
      main: '#f50057',
    },
    background: {
      default: '#f4f6f8',
    },
  },
  typography: {
    fontFamily: 'Roboto, Arial, sans-serif',
    h4: {
      fontWeight: 600,
    },
  },
});

function App() {
  const [user, setUser] = useState(null);
  const [error, setError] = useState('');
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);

  useEffect(() => {
    const loggedInUser = localStorage.getItem("user");
    if (loggedInUser) {
      const foundUser = JSON.parse(loggedInUser);
      setUser(foundUser);
      axios.defaults.headers.common['Authorization'] = `Bearer ${foundUser.token}`;
    }
  }, []);

  const handleLogin = async (username, password) => {
    setError('');
    try {
      const response = await axios.post('/api/auth/login', { username, password });
      const userWithToken = { ...response.data };
      localStorage.setItem('user', JSON.stringify(userWithToken));
      axios.defaults.headers.common['Authorization'] = `Bearer ${userWithToken.token}`;
      setUser(userWithToken);
      setShowLoginModal(false);
    } catch (err) {
      if (err.response) {
        if (err.response.status === 401) {
          setError('Invalid credentials.');
        } else if (err.response.data && typeof err.response.data === 'string') {
          setError(err.response.data);
        } else {
          setError('Login failed. Please try again.');
        }
      } else {
        setError('Network error. Please check your connection.');
      }
      setUser(null);
      localStorage.removeItem('user');
      delete axios.defaults.headers.common['Authorization'];
    }
  };
  
  const handleRegister = async (username, password) => {
    try {
        await axios.post('/api/auth/register', { username, password });
        // Automatically close the modal and show the login modal on success
        setShowRegisterModal(false);
        setShowLoginModal(true);
        setError('Registration successful! Please log in.'); // Use error state to show success message in login
    } catch (err) {
        if (err.response) {
            if (err.response.status === 409) {
                throw new Error('Username already exists.');
            } else if (err.response.status === 400) {
                // Assuming the backend sends a more specific message
                throw new Error('Invalid data. Username must be at least 3 characters and password at least 8 characters.');
            }
        }
        throw new Error('Registration failed. Please try again.');
    }
};


  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem('user');
    delete axios.defaults.headers.common['Authorization'];
    setShowLoginModal(true);
  };

  const handleShowRegister = () => {
    setShowLoginModal(false);
    setShowRegisterModal(true);
  };
  
  const handleShowLogin = () => {
    setShowRegisterModal(false);
    setShowLoginModal(true);
  }

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{
        minHeight: '100vh',
        width: '100%',
        backgroundImage: `linear-gradient(rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url(${backgroundImage})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        backgroundRepeat: 'no-repeat',
        backgroundAttachment: 'fixed',
      }}>
        <AppBar position="static">
          <Toolbar>
            <SailingIcon sx={{ mr: 2 }} />
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              HarborFlow
            </Typography>
            {!user && <Button color="inherit" onClick={() => setShowLoginModal(true)}>Login</Button>}
            {!user && <Button color="inherit" onClick={() => setShowRegisterModal(true)}>Register</Button>}
          </Toolbar>
        </AppBar>
        <Container component="main" sx={{ mt: 4, mb: 4 }}>
          <Dashboard user={user} handleLogout={handleLogout} />
        </Container>
        {showLoginModal && <LoginModal handleLogin={handleLogin} handleClose={() => setShowLoginModal(false)} handleShowRegister={handleShowRegister} error={error} setError={setError} />}
        {showRegisterModal && <RegisterModal handleRegister={handleRegister} handleClose={() => setShowRegisterModal(false)} handleShowLogin={handleShowLogin} />}
      </Box>
    </ThemeProvider>
  );
}

export default App;
