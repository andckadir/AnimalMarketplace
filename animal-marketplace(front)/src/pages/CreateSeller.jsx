import { useForm } from 'react-hook-form';
import { sellerService } from '../api/sellerService';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function CreateSeller() {
  const { register, handleSubmit } = useForm();
  const { refreshSellerStatus } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      // --- DEĞİŞİKLİK BURADA ---
      // Formdan gelen datayı kopyala ve businessName'i BÜYÜK HARFE çevir
      const payload = {
        ...data,
        businessName: data.businessName.toUpperCase()
      };
      
      // Backend'e manipüle edilmiş payload'ı gönder
      const response = await sellerService.createSeller(payload);
      
      // Token kontrolü ve güncelleme (Önceki adımda yaptığımız yapı)
      if (response && response.token) {
        updateToken(response.token);
      } else {
        await refreshSellerStatus();
      }
      
      toast.success("Tebrikler! Artık satıcı hesabınız var.");
      navigate('/create-advert');
      
    } catch (error) {
      console.error(error);
      toast.error("İşlem başarısız oldu.");
    }
  };

  return (
    <div className="max-w-md mx-auto mt-20 p-8 bg-white rounded shadow-lg border border-orange-200">
      <div className="text-center mb-6">
        <h2 className="text-2xl font-bold text-gray-800">Satıcı Hesabı Oluştur</h2>
        <p className="text-gray-600 mt-2">İlan verebilmek için önce işletme/satıcı adı belirlemelisiniz.</p>
      </div>
      
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">İşletme / Satıcı Adı</label>
            <input 
             {...register("businessName")} 
            required 
            placeholder="Örn: PATİ PET SHOP" 
             className="w-full border border-gray-300 p-3 rounded focus:ring-2 focus:ring-orange-500 outline-none uppercase" // Eklendi
            />
        </div>

        <button type="submit" className="w-full bg-orange-600 text-white py-3 rounded font-bold hover:bg-orange-700 transition">
          Hesabı Oluştur ve Devam Et
        </button>
      </form>
    </div>
  );
}