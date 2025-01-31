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

interface ValidationErrors {
  name?: string;
  baseUrl?: string;
  programCode?: string;
  email?: string;
  password?: string;
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
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
  const [testing, setTesting] = useState(false);

  const validateForm = (): boolean => {
    const errors: ValidationErrors = {};
    
    if (!formData.name.trim()) {
      errors.name = 'Name is required';
    }

    if (!formData.baseUrl.trim()) {
      errors.baseUrl = 'Base URL is required';
    } else if (!/^https?:\/\/.+/.test(formData.baseUrl)) {
      errors.baseUrl = 'Base URL must start with http:// or https://';
    }

    if (!formData.programCode.trim()) {
      errors.programCode = 'Program code is required';
    } else if (formData.programCode.length !== 5) {
      errors.programCode = 'Program code must be exactly 5 characters';
    }

    if (!formData.email.trim()) {
      errors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      errors.email = 'Invalid email format';
    }

    if (!formData.password.trim()) {
      errors.password = 'Password is required';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const testConnection = async () => {
    setTesting(true);
    setError(null);
    try {
      // First test the login to get a token
      const response = await api.post('/imsinstances/test-connection', {
        baseUrl: formData.baseUrl,
        programCode: formData.programCode,
        email: formData.email,
        password: formData.password
      });
      
      setSuccess('Connection test successful! Token received: ' + response.data.token);
    } catch (err: any) {
      console.error('Connection test failed:', err);
      setError(err.response?.data?.message || 'Failed to test connection to IMS');
    } finally {
      setTesting(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!validateForm()) {
      return;
    }

    try {
      // Ensure baseUrl ends with a forward slash
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
      
      // Clear form
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
      setError(err.response?.data?.message || 'Failed to add IMS instance');
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