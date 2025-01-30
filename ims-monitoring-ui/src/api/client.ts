import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5071/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to requests if it exists
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
    // Log the request details
    console.log('Request:', {
      url: config.url,
      method: config.method,
      headers: config.headers,
      data: config.data
    });
  } else {
    console.warn('No token found in localStorage');
  }
  return config;
}, (error) => {
  console.error('Request interceptor error:', error);
  return Promise.reject(error);
});

// Handle errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Enhanced error logging
    const errorDetails = {
      status: error.response?.status,
      statusText: error.response?.statusText,
      data: error.response?.data,
      errors: error.response?.data?.errors,
      title: error.response?.data?.title,
      type: error.response?.data?.type,
      traceId: error.response?.data?.traceId,
      request: {
        method: error.config?.method,
        url: error.config?.url,
        data: typeof error.config?.data === 'string' 
          ? JSON.parse(error.config.data) 
          : error.config?.data,
        headers: error.config?.headers
      }
    };
    
    console.error('API Error Details:', JSON.stringify(errorDetails, null, 2));
    return Promise.reject(error);
  }
);
