import axiosClient from './axiosClient';

export const advertService = {
  // --- MEVCUT METODLAR ---
  getAdverts: async (filterDto, page = 1, pageSize = 20) => {
    const response = await axiosClient.post(`/Advert/filter`, filterDto, {
      params: { page, pageSize }
    });
    return response.data;
  },

  getById: async (id) => {
    const response = await axiosClient.get(`/Advert/${id}`);
    return response.data;
  },

  create: async (formData) => {
    const response = await axiosClient.post('/Advert/create', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  deleteAdvert: async (id) => {
    await axiosClient.delete(`/Advert/${id}`);
  },

  updateAdvert: async (id, data) => {
    const response = await axiosClient.patch(`/Advert/${id}`, data);
    return response.data;
  },

  // --- GÜNCELLENEN RESİM İŞLEMLERİ (Ekran görüntüne göre) ---

  /**
   * Resim Ekleme
   * URL: POST /api/Advert/{advertId}/images
   */
  addImage: async (advertId, imageFile) => {
    const formData = new FormData();
    // Backend "images" adında bir array bekliyor
    formData.append('images', imageFile);

    const response = await axiosClient.post(`/Advert/${advertId}/images`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
  },

  /**
   * Resmi Silme
   * URL: DELETE /api/Advert/images/{imageId}
   */
  removeImage: async (imageId) => {
    await axiosClient.delete(`/Advert/images/${imageId}`);
  },

  /**
   * Resmi Ana Resim Yapma
   * URL: PATCH /api/Advert/images/{imageId}/set-primary
   */
  setPrimaryImage: async (imageId) => {
    await axiosClient.patch(`/Advert/images/${imageId}/set-primary`);
  }
};