import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Grid,
  Card,
  CardContent,
  IconButton,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { api } from '../api/client';

interface ExternalSystem {
  id: string;
  name: string;
  version: string;
  description: string;
}

interface Connection {
  id: string;
  imsInstanceId: string;
  externalSystemId: string;
  connectionString: string;
  username?: string;
  password?: string;
  apiKey?: string;
  externalSystem: ExternalSystem;
}

interface Props {
  imsInstanceId: string;
}

export default function ExternalSystemConnections({ imsInstanceId }: Props) {
  const [systems, setSystems] = useState<ExternalSystem[]>([]);
  const [connections, setConnections] = useState<Connection[]>([]);
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [formData, setFormData] = useState({
    externalSystemId: '',
    connectionString: '',
    username: '',
    password: '',
    apiKey: '',
  });

  useEffect(() => {
    fetchData();
  }, [imsInstanceId]);

  const fetchData = async () => {
    try {
      const [systemsRes, connectionsRes] = await Promise.all([
        api.get<ExternalSystem[]>('/externalsystem'),
        api.get<Connection[]>(`/externalsystem/instance/${imsInstanceId}`)
      ]);
      setSystems(systemsRes.data);
      setConnections(connectionsRes.data);
    } catch (error) {
      console.error('Failed to fetch data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    try {
      await api.post('/externalsystem/connect', {
        ...formData,
        imsInstanceId
      });
      setOpen(false);
      fetchData();
    } catch (error) {
      console.error('Failed to create connection:', error);
    }
  };

  const handleDelete = async (connectionId: string) => {
    try {
      await api.delete(`/externalsystem/connect/${connectionId}`);
      fetchData();
    } catch (error) {
      console.error('Failed to delete connection:', error);
    }
  };

  return (
    <Box sx={{ mt: 2 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h6">External System Connections</Typography>
        <Button variant="contained" onClick={() => setOpen(true)}>
          Add Connection
        </Button>
      </Box>

      <Grid container spacing={2}>
        {connections.map((connection) => (
          <Grid item xs={12} md={6} key={connection.id}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="h6">
                    {connection.externalSystem.name} v{connection.externalSystem.version}
                  </Typography>
                  <IconButton onClick={() => handleDelete(connection.id)}>
                    <DeleteIcon />
                  </IconButton>
                </Box>
                <Typography color="textSecondary" gutterBottom>
                  {connection.externalSystem.description}
                </Typography>
                {connection.username && (
                  <Typography>Username: {connection.username}</Typography>
                )}
                {connection.connectionString && (
                  <Typography>Connection String: {connection.connectionString}</Typography>
                )}
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={open} onClose={() => setOpen(false)}>
        <DialogTitle>Add External System Connection</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>External System</InputLabel>
            <Select
              value={formData.externalSystemId}
              onChange={(e) => setFormData({ ...formData, externalSystemId: e.target.value })}
            >
              {systems.map((system) => (
                <MenuItem key={system.id} value={system.id}>
                  {system.name} v{system.version}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            fullWidth
            label="Connection String"
            value={formData.connectionString}
            onChange={(e) => setFormData({ ...formData, connectionString: e.target.value })}
            margin="normal"
          />
          <TextField
            fullWidth
            label="Username (optional)"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            margin="normal"
          />
          <TextField
            fullWidth
            label="Password (optional)"
            type="password"
            value={formData.password}
            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
            margin="normal"
          />
          <TextField
            fullWidth
            label="API Key (optional)"
            value={formData.apiKey}
            onChange={(e) => setFormData({ ...formData, apiKey: e.target.value })}
            margin="normal"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            Add Connection
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
} 