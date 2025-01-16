import { useEffect, useState } from 'react';
import { Box, Container, Typography, Paper, Grid, CircularProgress } from '@mui/material';
import { api } from '../api/client';

interface UserProfile {
  email: string;
  // Add more profile fields as needed
}

export default function Dashboard() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [profile, setProfile] = useState<UserProfile | null>(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      setError('Not authenticated');
      setLoading(false);
      return;
    }

    // Set the token in the API client headers
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;

    // Fetch user profile
    const fetchProfile = async () => {
      try {
        const response = await api.get('/auth/profile');
        setProfile(response.data);
      } catch (err) {
        console.error('Failed to fetch profile:', err);
        setError('Failed to load profile');
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, []);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 3 }}>
            <Typography variant="h4" gutterBottom>
              Welcome to IMS Monitoring
            </Typography>
            {profile && (
              <Typography variant="subtitle1">
                Logged in as: {profile.email}
              </Typography>
            )}
          </Paper>
        </Grid>

        {/* Stats Overview */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Typography variant="h6" gutterBottom>
              Total Submissions
            </Typography>
            <Typography component="p" variant="h4">
              0
            </Typography>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Typography variant="h6" gutterBottom>
              Active Monitors
            </Typography>
            <Typography component="p" variant="h4">
              0
            </Typography>
          </Paper>
        </Grid>

        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column', height: 140 }}>
            <Typography variant="h6" gutterBottom>
              System Status
            </Typography>
            <Typography component="p" variant="h4" color="success.main">
              Online
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
