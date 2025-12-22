import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { TR_CITIES } from '../utils/locations'; 
import { advertService } from '../api/advertService';
import { ANIMAL_KINDS } from '../utils/constants';
import { useAuth } from '../context/AuthContext';

export default function CreateAdvert() {
  // setValue fonksiyonunu useForm'dan çıkardık (İlçe sıfırlamak için lazım)
  const { register, handleSubmit, setValue } = useForm();
  
  const navigate = useNavigate();
  const { isSeller, user } = useAuth();
  
  // Şehre göre ilçeleri tutacak state
  const [districts, setDistricts] = useState([]);

  // --- KORUMA MANTIĞI ---
  useEffect(() => {
    if (!user) {
        navigate('/login');
        return;
    }
    if (!isSeller) {
        toast.info("İlan vermek için önce satıcı profili oluşturmalısınız.");
        navigate('/create-seller');
    }
  }, [user, isSeller, navigate]);

  // Şehir değişince çalışacak fonksiyon
  const handleCityChange = (e) => {
    const selectedCityName = e.target.value;
    
    // 1. Seçilen şehri bul
    const cityData = TR_CITIES.find(city => city.name === selectedCityName);
    
    // 2. İlçeleri state'e yükle
    setDistricts(cityData ? cityData.districts : []);
    
    // 3. Formdaki İlçe seçimini sıfırla
    setValue("District", ""); 
  };

  // Eğer satıcı değilse formu gösterme
  if (!isSeller) return null; 

  const onSubmit = async (data) => {
    const formData = new FormData();
    formData.append('Title', data.Title);
    formData.append('Description', data.Description);
    formData.append('Price', data.Price);
    formData.append('Age', data.Age);
    formData.append('City', data.City);
    formData.append('District', data.District);
    formData.append('Kind', data.Kind);
    formData.append('Gender', data.Gender);

    if (data.images && data.images.length > 0) {
      for (let i = 0; i < data.images.length; i++) {
        formData.append('images', data.images[i]);
      }
    }

    try {
      await advertService.create(formData);
      toast.success("İlan başarıyla yayınlandı!");
      navigate('/');
    } catch (error) {
      console.error(error);
      toast.error("Hata oluştu. Lütfen bilgileri kontrol edin.");
    }
  };

  return (
    <div className="max-w-2xl mx-auto mt-10 p-6 bg-white rounded shadow mb-20">
        <h2 className="text-2xl font-bold mb-6 text-gray-800">Yeni İlan Oluştur</h2>
        
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {/* BAŞLIK */}
            <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">İlan Başlığı</label>
                <input {...register("Title")} placeholder="Örn: Sahibinden Satılık Kedi" required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/>
            </div>

            {/* AÇIKLAMA */}
            <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Açıklama</label>
                <textarea {...register("Description")} placeholder="Detaylı bilgi giriniz..." className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none" rows="4"/>
            </div>
            
            {/* FİYAT VE YAŞ */}
            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">Fiyat (TL)</label>
                    <input {...register("Price")} type="number" step="0.01" placeholder="0.00" required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/>
                </div>
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">Yaş</label>
                    <input {...register("Age")} type="number" placeholder="0" className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/>
                </div>
            </div>

            {/* ŞEHİR VE İLÇE (DİNAMİK SELECT) */}
            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">Şehir</label>
                    <select 
                        {...register("City", { 
                            required: true,
                            onChange: handleCityChange // Şehir değişince ilçeleri güncelle
                        })} 
                        className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none bg-white"
                    >
                        <option value="">Şehir Seçiniz</option>
                        {TR_CITIES.map((city) => (
                            <option key={city.name} value={city.name}>{city.name}</option>
                        ))}
                    </select>
                </div>
                
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">İlçe</label>
                    <select 
                        {...register("District", { required: true })} 
                        disabled={districts.length === 0}
                        className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none bg-white disabled:bg-gray-100"
                    >
                        <option value="">İlçe Seçiniz</option>
                        {districts.map((d) => (
                            <option key={d} value={d}>{d}</option>
                        ))}
                    </select>
                </div>
            </div>

            {/* TÜR VE CİNSİYET */}
            <div className="grid grid-cols-2 gap-4">
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">Hayvan Türü</label>
                    <select {...register("Kind")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none bg-white">
                        {Object.entries(ANIMAL_KINDS).map(([k,v]) => <option key={k} value={k}>{v}</option>)}
                    </select>
                </div>
                <div>
                    <label className="block text-sm font-bold text-gray-700 mb-1">Cinsiyet</label>
                    <select {...register("Gender")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none bg-white">
                        <option value="0">Erkek</option> 
                        <option value="1">Dişi</option>
                    </select>
                </div>
            </div>

            {/* RESİM YÜKLEME */}
            <div>
                <label className="block text-sm font-bold text-gray-700 mb-1">Resimler</label>
                <div className="border-2 border-dashed border-gray-300 rounded-lg p-4 text-center hover:bg-gray-50 transition">
                    <input {...register("images")} type="file" multiple accept="image/*" className="w-full cursor-pointer"/>
                    <p className="text-xs text-gray-500 mt-2">Birden fazla resim seçebilirsiniz.</p>
                </div>
            </div>

            <button type="submit" className="w-full bg-blue-600 text-white py-3 rounded-lg font-bold hover:bg-blue-700 transition shadow-md">
                İlanı Yayınla
            </button>
        </form>
    </div>
  );
}