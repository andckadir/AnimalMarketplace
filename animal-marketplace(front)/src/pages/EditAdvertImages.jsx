import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { advertService } from '../api/advertService';
import { toast } from 'react-toastify';

export default function EditAdvertImages() {
  const { id } = useParams(); // URL'den gelen advertId
  const navigate = useNavigate();
  const [images, setImages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);

  // İlan verilerini ve resimleri çek
  const loadImages = async () => {
    try {
      const data = await advertService.getById(id);
      
      // Swagger'a göre resim objesi "AdvertImageDto" yapısında ve "isPrimary" alanı var.
      // Primary olanı en başa alarak sıralıyoruz.
      const sortedImages = (data.images || []).sort((a, b) => (b.isPrimary === true) - (a.isPrimary === true));
      setImages(sortedImages);
    } catch (error) {
      toast.error("Resimler yüklenemedi.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadImages();
  }, [id]);

  // --- YENİ RESİM YÜKLEME ---
  const handleUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    try {
      setUploading(true);
      // Servise advertId (id) ve dosyayı gönderiyoruz
      await advertService.addImage(id, file);
      
      toast.success("Resim eklendi.");
      await loadImages(); // Listeyi yenile
    } catch (error) {
      console.error(error);
      toast.error("Resim yüklenirken hata oluştu.");
    } finally {
      setUploading(false);
      e.target.value = null; // Input'u temizle
    }
  };

  // --- ANA RESİM YAPMA ---
  const handleSetPrimary = async (imageId) => {
    try {
      // Servise imageId gönderiyoruz
      await advertService.setPrimaryImage(imageId);
      toast.success("Ana resim güncellendi.");
      await loadImages();
    } catch (error) {
      console.error(error);
      toast.error("İşlem başarısız.");
    }
  };

  // --- RESİM SİLME ---
  const handleDelete = async (image) => {
    // Kural: Tek resim kaldıysa veya ana resimse uyarı ver (Opsiyonel UX kuralı)
    if (images.length <= 1) {
      toast.warn("İlanda en az bir resim bulunmalıdır.");
      return;
    }

    if (image.isPrimary) {
      toast.warn("Ana resim silinemez! Lütfen önce başka bir resmi ana resim yapın.");
      return;
    }

    if (!window.confirm("Bu resmi silmek istediğinize emin misiniz?")) return;

    try {
      // Servise imageId gönderiyoruz
      // Swagger'da AdvertImageDto içinde id alanı 'id' olarak tanımlı
      await advertService.removeImage(image.id);
      
      toast.success("Resim silindi.");
      // Listeden anlık silerek tekrar yükleme yapmadan hız kazanalım
      setImages(prev => prev.filter(img => img.id !== image.id));
    } catch (error) {
      console.error(error);
      toast.error("Silme işlemi başarısız.");
    }
  };

  if (loading) return <div className="text-center mt-10">Yükleniyor...</div>;

  return (
    <div className="max-w-4xl mx-auto mt-10 p-6 bg-white rounded shadow">
      <div className="flex justify-between items-center mb-6 border-b pb-4">
        <h2 className="text-2xl font-bold text-gray-800">Resim Yönetimi</h2>
        <button onClick={() => navigate(`/edit-advert/${id}`)} className="text-blue-600 hover:underline">
          ← İlan Bilgilerine Dön
        </button>
      </div>

      {/* --- YENİ YÜKLEME ALANI --- */}
      <div className="mb-8 p-4 bg-blue-50 border border-blue-200 rounded-lg text-center">
        <label className="cursor-pointer block">
            <div className="flex flex-col items-center gap-2">
                <span className="text-4xl">📸</span>
                <span className="font-bold text-blue-700">Yeni Resim Ekle</span>
                <span className="text-sm text-gray-500">Yüklemek için tıklayın</span>
            </div>
            <input 
                type="file" 
                accept="image/*" 
                onChange={handleUpload} 
                disabled={uploading} 
                className="hidden" 
            />
        </label>
        {uploading && <p className="mt-2 text-blue-600 font-bold animate-pulse">Yükleniyor...</p>}
      </div>

      {/* --- RESİM LİSTESİ --- */}
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
        {images.map((img) => (
          <div key={img.id} className={`relative border rounded-lg overflow-hidden group bg-gray-50 ${img.isPrimary ? 'ring-4 ring-green-500' : 'hover:shadow-lg'}`}>
            
            {/* Resim */}
            <div className="h-48 flex items-center justify-center p-2 bg-white">
                <img src={img.url} alt="Advert" className="w-full h-full object-contain" />
            </div>

            {/* Kontrol Butonları */}
            <div className="p-3 bg-white border-t">
                {img.isPrimary ? (
                    <div className="w-full bg-green-100 text-green-700 text-center py-1 rounded font-bold text-sm mb-2 border border-green-200">
                        🌟 ANA RESİM
                    </div>
                ) : (
                    <button 
                        onClick={() => handleSetPrimary(img.id)}
                        className="w-full bg-gray-100 text-gray-700 hover:bg-blue-100 hover:text-blue-700 text-center py-1 rounded font-medium text-sm mb-2 transition border"
                    >
                        Ana Resim Yap
                    </button>
                )}

                <button 
                    onClick={() => handleDelete(img)}
                    disabled={img.isPrimary} 
                    className={`w-full py-1 rounded font-medium text-sm transition flex items-center justify-center gap-1 border
                        ${img.isPrimary 
                            ? 'bg-gray-50 text-gray-300 cursor-not-allowed border-transparent' 
                            : 'bg-white text-red-600 border-red-200 hover:bg-red-50'}`}
                >
                    🗑️ Sil
                </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}