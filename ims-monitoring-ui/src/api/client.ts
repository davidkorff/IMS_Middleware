import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5071/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add response interceptor for handling errors
api.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);
