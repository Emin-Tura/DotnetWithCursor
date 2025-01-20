import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  List,
  ListItem,
  ListItemText,
  Divider,
} from '@mui/material';
import { authApi, User } from '../services/api';

const Profile: React.FC = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const response = await authApi.getCurrentUser();
        setUser(response.data);
      } catch (err) {
        setError('Failed to load user profile');
        navigate('/login');
      }
    };

    fetchUser();
  }, [navigate]);

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/login');
  };

  if (!user) {
    return null;
  }

  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 8 }}>
        <Paper elevation={3} sx={{ p: 4 }}>
          <Typography component="h1" variant="h4" align="center" gutterBottom>
            Profile
          </Typography>
          <List>
            <ListItem>
              <ListItemText
                primary="Username"
                secondary={user.username}
              />
            </ListItem>
            <Divider />
            <ListItem>
              <ListItemText
                primary="Email"
                secondary={user.email}
              />
            </ListItem>
            <Divider />
            <ListItem>
              <ListItemText
                primary="Full Name"
                secondary={`${user.firstName} ${user.lastName}`}
              />
            </ListItem>
            <Divider />
            <ListItem>
              <ListItemText
                primary="Member Since"
                secondary={new Date(user.createdAt).toLocaleDateString()}
              />
            </ListItem>
          </List>
          <Box sx={{ mt: 3, textAlign: 'center' }}>
            <Button
              variant="contained"
              color="error"
              onClick={handleLogout}
            >
              Logout
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default Profile; 