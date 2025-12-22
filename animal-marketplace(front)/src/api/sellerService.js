import axiosClient from './axiosClient';

export const sellerService = {
  // Satıcı bilgisini getir
  getSeller: async () => {
    try {
      const response = await axiosClient.get('/Seller/get');
      return response.data; 
    } catch (error) {
      // Eğer 400 veya 404 alırsak (Satıcı silinmişse), 
      // hatayı konsola basma, sadece null dön.
      // Bu sayede sistem onu normal kullanıcı sanar.
      return null; 
    }
  },

  createSeller: async (data) => {
    const response = await axiosClient.post('/Seller/create', data);
    return response.data;
  },

  updateSeller: async (data) => {
    const response = await axiosClient.patch('/Seller/update', data);
    return response.data;
  },

  deleteSeller: async (password) => {
    await axiosClient.delete('/Seller/delete', {
      data: { password: password }
    });
  }
};