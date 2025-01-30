import { useEffect, useState } from 'react';
import { 
  Box, 
  Container, 
  Typography, 
  Paper, 
  Grid, 
  CircularProgress,
  Select,
  MenuItem,
  Button,
  FormControl,
  InputLabel
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { api } from '../api/client';
import ExternalSystemConnections from '../components/ExternalSystemConnections';

interface UserProfile {
  email: string;
  // Add more profile fields as needed
}

interface ImsInstance {
  id: string;
  name: string;
  baseUrl: string;
}

export default function Dashboard() {
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [instances, setInstances] = useState<ImsInstance[]>([]);
  const [selectedInstance, setSelectedInstance] = useState<string>('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchProfile();
    fetchImsInstances();
  }, []);

  const fetchProfile = async () => {
    try {
      const response = await api.get('/auth/profile');
      setProfile(response.data);
    } catch (error) {
      console.error('Failed to fetch profile:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchImsInstances = async () => {
    try {
      const response = await api.get('/imsinstances');
      setInstances(response.data);
      if (response.data.length > 0) {
        setSelectedInstance(response.data[0].id);
      }
    } catch (error) {
      console.error('Failed to fetch IMS instances:', error);
    }
  };

  const handleAddInstance = () => {
    navigate('/add-ims');
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: 'flex', flexDirection: 'column' }}>
            <Typography component="h1" variant="h4" gutterBottom>
              Welcome {profile?.email}
            </Typography>
          </Paper>
        </Grid>

        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: 'flex', alignItems: 'center', gap: 2 }}>
            <FormControl sx={{ minWidth: 200 }}>
              <InputLabel id="ims-instance-label">IMS Instance</InputLabel>
              <Select
                labelId="ims-instance-label"
                value={selectedInstance}
                label="IMS Instance"
                onChange={(e) => setSelectedInstance(e.target.value)}
              >
                {instances.map((instance) => (
                  <MenuItem key={instance.id} value={instance.id}>
                    {instance.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <Button 
              variant="contained" 
              onClick={handleAddInstance}
              sx={{ height: 40 }}
            >
              Add IMS Instance
            </Button>
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

        <Grid item xs={12}>
          {selectedInstance && (
            <Paper sx={{ p: 2 }}>
              <ExternalSystemConnections imsInstanceId={selectedInstance} />
            </Paper>
          )}
        </Grid>
      </Grid>
    </Container>
  );
}
