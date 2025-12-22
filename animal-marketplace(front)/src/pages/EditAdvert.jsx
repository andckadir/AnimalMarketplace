import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useParams, useNavigate } from 'react-router-dom';
import { advertService } from '../api/advertService';
import { ANIMAL_KINDS } from '../utils/constants';
import { toast } from 'react-toastify';

export default function EditAdvert() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { register, handleSubmit, setValue } = useForm();
  
  const [loading, setLoading] = useState(true);
  const [images, setImages] = useState([]);
  const [uploading, setUploading] = useState(false);

  // --- VERİLERİ YÜKLE ---
  const loadAdvertData = async () => {
    try {
      const data = await advertService.getById(id);
      
      // 1. Formu Doldur
      setValue("Title", data.title);
      setValue("Description", data.description);
      setValue("Price", data.price);
      setValue("Age", data.animalAge || data.age);
      setValue("City", data.address?.city || data.city);
      setValue("District", data.address?.district || data.district);
      setValue("Kind", data.animalKind);
      setValue("Gender", data.animalGender);

      // 2. Resimleri State'e At (Primary olan en başa)
      const sortedImages = (data.images || []).sort((a, b) => (b.isPrimary === true) - (a.isPrimary === true));
      setImages(sortedImages);

    } catch (error) {
      toast.error("İlan verileri alınamadı.");
      navigate('/profile');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAdvertData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  // --- 1. METİN GÜNCELLEME İŞLEMİ ---
  const onSubmit = async (data) => {
    try {
      const payload = {
        title: data.Title,
        description: data.Description,
        price: Number(data.Price),
        age: Number(data.Age),
        city: data.City,
        district: data.District,
        kind: Number(data.Kind),
        gender: Number(data.Gender)
      };

      await advertService.updateAdvert(id, payload);
      toast.success("İlan bilgileri başarıyla güncellendi!");
      // İsteğe bağlı: navigate('/profile'); 
    } catch (error) {
      console.error(error);
      toast.error("Güncelleme başarısız.");
    }
  };

  // --- 2. RESİM YÜKLEME İŞLEMİ ---
  const handleImageUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    try {
      setUploading(true);
      await advertService.addImage(id, file); // Backend'e anında yükle
      toast.success("Resim eklendi.");
      await loadAdvertData(); // Sayfayı yenilemeden verileri güncelle
    } catch (error) {
      console.error(error);
      toast.error("Resim yüklenirken hata oluştu.");
    } finally {
      setUploading(false);
      e.target.value = null; // Input'u temizle
    }
  };

  // --- 3. RESİM SİLME İŞLEMİ ---
  const handleImageDelete = async (image) => {
    if (images.length <= 1) {
      toast.warn("İlanda en az bir resim kalmalıdır.");
      return;
    }
    if (image.isPrimary) {
      toast.warn("Ana resim silinemez! Önce başka bir resmi ana resim yapın.");
      return;
    }
    if (!window.confirm("Bu resmi silmek istediğinize emin misiniz?")) return;

    try {
      // AdvertImageDto içinde ID alanı 'id' mi 'imageId' mi kontrol ediyoruz
      const imgId = image.id || image.imageId;
      await advertService.removeImage(imgId);
      
      toast.success("Resim silindi.");
      setImages(prev => prev.filter(img => (img.id || img.imageId) !== imgId));
    } catch (error) {
      console.error(error);
      toast.error("Silme işlemi başarısız.");
    }
  };

  // --- 4. ANA RESİM YAPMA İŞLEMİ ---
  const handleSetPrimary = async (image) => {
    try {
      const imgId = image.id || image.imageId;
      await advertService.setPrimaryImage(imgId);
      toast.success("Ana resim güncellendi.");
      await loadAdvertData(); // Sıralamayı güncellemek için veriyi tekrar çek
    } catch (error) {
      console.error(error);
      toast.error("İşlem başarısız.");
    }
  };

  if (loading) return <div className="text-center mt-10">Yükleniyor...</div>;

  return (
    <div className="max-w-4xl mx-auto mt-10 p-6 bg-white rounded shadow mb-20">
      
      <div className="flex justify-between items-center mb-6 border-b pb-4">
        <h2 className="text-2xl font-bold text-gray-800">İlanı Düzenle</h2>
        <button onClick={() => navigate('/profile')} className="text-gray-500 hover:text-gray-700 font-medium">
            ← Profile Dön
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* --- SOL TARAFTAKİ FORM (Metin Bilgileri) --- */}
        <div className="lg:col-span-2 space-y-6">
            <h3 className="text-lg font-bold text-blue-600 border-b pb-2">1. İlan Bilgileri</h3>
            
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                <div>
                    <label className="block text-sm font-bold text-gray-700">Başlık</label>
                    <input {...register("Title")} required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/>
                </div>
                
                <div>
                    <label className="block text-sm font-bold text-gray-700">Açıklama</label>
                    <textarea {...register("Description")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none" rows="5"/>
                </div>
                
                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-bold text-gray-700">Fiyat</label>
                        <input {...register("Price")} type="number" step="0.01" className="border p-2 rounded w-full outline-none"/>
                    </div>
                    <div>
                        <label className="block text-sm font-bold text-gray-700">Yaş</label>
                        <input {...register("Age")} type="number" className="border p-2 rounded w-full outline-none"/>
                    </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-bold text-gray-700">Şehir</label>
                        <input {...register("City")} className="border p-2 rounded w-full outline-none"/>
                    </div>
                    <div>
                        <label className="block text-sm font-bold text-gray-700">İlçe</label>
                        <input {...register("District")} className="border p-2 rounded w-full outline-none"/>
                    </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-bold text-gray-700">Tür</label>
                        <select {...register("Kind")} className="border p-2 rounded w-full bg-white">
                            {Object.entries(ANIMAL_KINDS).map(([k,v]) => <option key={k} value={k}>{v}</option>)}
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-bold text-gray-700">Cinsiyet</label>
                        <select {...register("Gender")} className="border p-2 rounded w-full bg-white">
                            <option value="0">Dişi</option>
                            <option value="1">Erkek</option>
                        </select>
                    </div>
                </div>

                <button type="submit" className="w-full bg-blue-600 text-white py-3 rounded-lg font-bold hover:bg-blue-700 transition shadow-md mt-4">
                    Bilgileri Kaydet
                </button>
            </form>
        </div>

        {/* --- SAĞ TARAFTAKİ PANEL (Resim Yönetimi) --- */}
        <div className="lg:col-span-1 space-y-6">
            <h3 className="text-lg font-bold text-orange-600 border-b pb-2">2. Resim Yönetimi</h3>
            
            {/* Yükleme Alanı */}
            <div className="border-2 border-dashed border-orange-200 bg-orange-50 rounded-lg p-4 text-center hover:bg-orange-100 transition cursor-pointer relative">
                <input 
                    type="file" 
                    accept="image/*" 
                    onChange={handleImageUpload} 
                    disabled={uploading} 
                    className="absolute inset-0 w-full h-full opacity-0 cursor-pointer" 
                />
                <div className="space-y-1 pointer-events-none">
                    <span className="text-3xl block">📸</span>
                    <span className="text-orange-700 font-bold block">{uploading ? "Yükleniyor..." : "Resim Ekle"}</span>
                </div>
            </div>

            {/* Resim Listesi */}
            <div className="space-y-4 max-h-[600px] overflow-y-auto pr-2 custom-scrollbar">
                {images.map((img) => (
                    <div key={img.id || img.imageId} className={`flex gap-3 p-2 border rounded-lg bg-white shadow-sm ${img.isPrimary ? 'ring-2 ring-green-500 border-transparent' : ''}`}>
                        
                        {/* Küçük Resim */}
                        <div className="w-20 h-20 flex-shrink-0 bg-gray-100 rounded overflow-hidden flex items-center justify-center">
                            <img src={img.url} alt="advert" className="w-full h-full object-contain" />
                        </div>

                        {/* İşlemler */}
                        <div className="flex flex-col justify-between w-full">
                            {img.isPrimary ? (
                                <span className="text-xs font-bold text-green-600 bg-green-50 px-2 py-1 rounded w-fit">
                                    🌟 ANA RESİM
                                </span>
                            ) : (
                                <button 
                                    onClick={() => handleSetPrimary(img)}
                                    className="text-xs font-medium text-blue-600 bg-blue-50 px-2 py-1 rounded hover:bg-blue-100 w-fit transition"
                                >
                                    Ana Resim Yap
                                </button>
                            )}
                            
                            <button 
                                onClick={() => handleImageDelete(img)}
                                disabled={img.isPrimary}
                                className={`text-xs font-bold px-2 py-1 rounded w-fit self-end
                                    ${img.isPrimary 
                                        ? 'text-gray-300 cursor-not-allowed' 
                                        : 'text-red-600 bg-red-50 hover:bg-red-100 hover:text-red-700'}`}
                            >
                                🗑️ Sil
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </div>

      </div>
    </div>
  );
}