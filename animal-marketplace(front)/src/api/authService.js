import axiosClient from './axiosClient';

export const authService = {
  login: async (credentials) => {
    // credentials: { email, password }
    const response = await axiosClient.post('/User/login', credentials);
    return response.data; // Token döneceğini varsayıyoruz
  },
  register: async (userData) => {
    const response = await axiosClient.post('/User/register', userData);
    return response.data;
  }
};