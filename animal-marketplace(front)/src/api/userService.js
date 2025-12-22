import axiosClient from './axiosClient';

export const userService = {
  addFavorite: async (advertId) => {
    // POST /api/User/favorite?advertId=...
    const response = await axiosClient.post(`/User/favorite`, null, {
      params: { advertId }
    });
    return response.data;
  },

  removeFavorite: async (advertId) => {
    // DELETE /api/User/favorite?advertId=...
    const response = await axiosClient.delete(`/User/favorite`, {
      params: { advertId }
    });
    return response.data;
  },

  getAllFavorites: async () => {
    // GET /api/User/allfavorites -> Örn dönüş: [1, 4, 12]
    const response = await axiosClient.get('/User/allfavorites');
    return response.data; 
  },

  // --- YENİ EKLENENLER ---
  
  // Kullanıcı Bilgilerini Getir
  getUser: async () => {
    const response = await axiosClient.get('/User/get');
    return response.data;
  },

  // Bilgileri Güncelle
  updateUser: async (userData) => {
    // Swagger: PATCH /api/User/update
    const response = await axiosClient.patch('/User/update', userData);
    return response.data;
  },

  // Şifre Değiştir
  changePassword: async (passData) => {
    // Swagger: PATCH /api/User/change-password
    const response = await axiosClient.patch('/User/change-password', passData);
    return response.data;
  },

  // Hesabı Sil (Şifre ile)
  deleteUser: async (password) => {
    // Swagger: DELETE /api/User (Body içinde password bekleniyor)
    // Axios DELETE isteğinde body göndermek için { data: ... } yapısı kullanılır.
    await axiosClient.delete('/User/delete', {
      data: { password: password }
    });
  }
};