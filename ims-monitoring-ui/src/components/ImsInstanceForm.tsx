import React, { useState } from 'react';
import { Box, Button, TextField, Paper, Typography, Alert } from '@mui/material';
import { api } from '../utils/api';
import { jwtDecode } from 'jwt-decode';

interface FormData {
  name: string;
  baseUrl: string;
  programCode: string;
  email: string;
  password: string;
  notes: string;
}

export default function ImsInstanceForm() {
  const [formData, setFormData] = useState<FormData>({
    name: '',
    baseUrl: '',
    programCode: '',
    email: '',
    password: '',
    notes: ''
  });
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        setError('Not authenticated');
        return;
      }

      const decoded: any = jwtDecode(token);

      const baseUrl = formData.baseUrl.endsWith('/') 
        ? formData.baseUrl 
        : `${formData.baseUrl}/`;

      const data = {
        ...formData,
        baseUrl
      };

      console.log('Sending data:', JSON.stringify(data, null, 2));
      const response = await api.post('/imsinstances', data);
      console.log('Response:', response.data);

      setSuccess('IMS instance added successfully');
      
      setFormData({
        name: '',
        baseUrl: '',
        programCode: '',
        email: '',
        password: '',
        notes: ''
      });
    } catch (err: any) {
      console.error('Full error:', err);
      console.error('Response data:', err.response?.data);
      
      const errorMessage = err.response?.data?.title || 
                          err.response?.data?.message || 
                          err.response?.data || 
                          'Failed to add IMS instance';
      
      setError(errorMessage);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  return (
    <Box sx={{ p: 3 }}>
      <Paper elevation={3} sx={{ p: 4, maxWidth: 600, mx: 'auto' }}>
        <Typography variant="h5" gutterBottom align="center">
          Add IMS Instance
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Instance Name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            margin="normal"
            required
          />

          <TextField
            fullWidth
            label="Base URL"
            name="baseUrl"
            value={formData.baseUrl}
            onChange={handleChange}
            margin="normal"
            required
            error={Boolean(formData.baseUrl && !formData.baseUrl.toLowerCase().match(/^https?:\/\//))}
            helperText={
              formData.baseUrl && !formData.baseUrl.toLowerCase().match(/^https?:\/\//)
                ? 'URL must start with http:// or https://'
                : ''
            }
          />

          <TextField
            fullWidth
            label="Program Code"
            name="programCode"
            value={formData.programCode}
            onChange={handleChange}
            margin="normal"
            required
          />

          <TextField
            fullWidth
            label="Email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            margin="normal"
            required
          />

          <TextField
            fullWidth
            label="Password"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
            margin="normal"
            required
          />

          <TextField
            fullWidth
            label="Notes"
            name="notes"
            value={formData.notes}
            onChange={handleChange}
            margin="normal"
            multiline
            rows={3}
          />

          <Button 
            type="submit"
            variant="contained"
            fullWidth
            sx={{ mt: 3 }}
          >
            Add Instance
          </Button>
        </form>
      </Paper>
    </Box>
  );
}