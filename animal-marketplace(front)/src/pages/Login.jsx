import { useForm } from 'react-hook-form';
import { authService } from '../api/authService';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function Login() {
  const { register, handleSubmit } = useForm();
  const { login } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      // Backend'den direkt token string mi yoksa { token: "..." } objesi mi dönüyor?
      // Genelde obje döner. Varsayım: response.token
      const response = await authService.login(data);
      
      // Eğer backend direkt string döndürüyorsa 'response' kullanın.
      // Eğer json ise 'response.token' (backend yapısına göre ayarlanmalı)
      const token = response.token || response; 
      
      login(token);
      toast.success("Giriş başarılı!");
      navigate('/');
    } catch (error) {
      toast.error("Giriş başarısız. Bilgileri kontrol edin.");
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white rounded shadow">
      <h2 className="text-2xl font-bold mb-4">Giriş Yap</h2>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <input {...register("email")} type="email" placeholder="Email" required className="w-full border p-2 rounded"/>
        <input {...register("password")} type="password" placeholder="Şifre" required className="w-full border p-2 rounded"/>
        <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded">Giriş</button>
      </form>
    </div>
  );
}