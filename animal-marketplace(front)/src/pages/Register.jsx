import { useForm } from "react-hook-form";
import { authService } from "../api/authService";
import { useNavigate, Link } from "react-router-dom"; // Link eklendi
import { toast } from "react-toastify";

export default function Register() {
  // setError fonksiyonunu da hook'tan çekiyoruz
  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm();
  const navigate = useNavigate();

  const onSubmit = async (data) => {
    try {
      // Backend Gender'ı int (enum) bekliyor
      const payload = {
        ...data,
        gender: parseInt(data.gender),
      };

      await authService.register(payload);
      toast.success("Kayıt başarılı! Giriş yapabilirsiniz.");
      navigate("/login");
    } catch (error) {
      // --- HATA YÖNETİMİ ---
      if (error.response && error.response.status === 400) {
        // Backend'den gelen validation hataları genelde "errors" objesi içindedir
        // Örn: { "Email": ["Email boş olamaz"], "Password": ["Şifre yetersiz"] }
        const backendErrors = error.response.data.errors;

        if (backendErrors) {
          Object.keys(backendErrors).forEach((key) => {
            // Backend "Password" (Büyük harf) dönebilir, biz formda "password" (küçük) kullanıyoruz.
            // Bu yüzden key'in ilk harfini küçültüyoruz.
            const fieldName = key.charAt(0).toLowerCase() + key.slice(1);

            setError(fieldName, {
              type: "server",
              message: backendErrors[key][0], // İlk hata mesajını al
            });
          });
          toast.warn("Lütfen formdaki hataları düzeltin.");
        } else {
          // Validation dışında genel bir 400 hatası ise
          toast.error(error.response.data.title || "Kayıt işlemi başarısız.");
        }
      } else {
        console.error(error);
        toast.error("Sunucu ile iletişim hatası.");
      }
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="max-w-md w-full bg-white rounded-xl shadow-lg p-8">
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-800">Aramıza Katıl</h2>
          <p className="text-gray-500 mt-2">
            Hayvan dostlarımız için hemen üye ol.
          </p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Ad & Soyad */}
          <div className="grid grid-cols-2 gap-4">
            <div>
              <input
                {...register("name")}
                placeholder="Ad"
                className={`w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 transition ${errors.name ? "border-red-500" : "border-gray-300"}`}
              />
              {errors.name && (
                <p className="text-red-500 text-xs mt-1 ml-1">
                  {errors.name.message}
                </p>
              )}
            </div>
            <div>
              <input
                {...register("surname")}
                placeholder="Soyad"
                className={`w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 transition ${errors.surname ? "border-red-500" : "border-gray-300"}`}
              />
              {errors.surname && (
                <p className="text-red-500 text-xs mt-1 ml-1">
                  {errors.surname.message}
                </p>
              )}
            </div>
          </div>

          {/* Email */}
          <div>
            <input
              {...register("email")}
              type="email"
              placeholder="E-posta Adresi"
              className={`w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 transition ${errors.email ? "border-red-500" : "border-gray-300"}`}
            />
            {errors.email && (
              <p className="text-red-500 text-xs mt-1 ml-1">
                {errors.email.message}
              </p>
            )}
          </div>

          {/* Telefon */}
          <div>
            <input
              {...register("phone")}
              placeholder="Telefon (5XX...)"
              className={`w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 transition ${errors.phone ? "border-red-500" : "border-gray-300"}`}
            />
            {errors.phone && (
              <p className="text-red-500 text-xs mt-1 ml-1">
                {errors.phone.message}
              </p>
            )}
          </div>

          {/* Şifre */}
          <div>
            <input
              {...register("password")}
              type="password"
              placeholder="Şifre"
              className={`w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 transition ${errors.password ? "border-red-500" : "border-gray-300"}`}
            />
            {errors.password && (
              <p className="text-red-500 text-xs mt-1 ml-1">
                {errors.password.message}
              </p>
            )}
          </div>

          {/* Cinsiyet */}
          <div>
            <select
              {...register("gender")}
              className="w-full border p-3 rounded-lg outline-none focus:ring-2 focus:ring-blue-500 bg-white border-gray-300 text-gray-700"
            >
              <option value="0">Erkek</option>
              <option value="1">Kadın</option>
            </select>
          </div>

          <button
            type="submit"
            className="w-full bg-blue-600 text-white py-3 rounded-lg font-bold hover:bg-blue-700 transition shadow-md hover:shadow-lg transform active:scale-95"
          >
            Kayıt Ol
          </button>
        </form>

        <p className="text-center text-gray-600 mt-6 text-sm">
          Zaten hesabın var mı?{" "}
          <Link to="/login" className="text-blue-600 font-bold hover:underline">
            Giriş Yap
          </Link>
        </p>
      </div>
    </div>
  );
}
