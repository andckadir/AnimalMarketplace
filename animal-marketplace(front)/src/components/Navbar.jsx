import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Navbar() {
  const { user, logout } = useAuth();

  return (
    <nav className="bg-blue-600 text-white p-4 shadow-md sticky top-0 z-50">
      <div className="container mx-auto flex justify-between items-center">
        {/* LOGO */}
        <Link
          to="/"
          className="text-2xl font-bold flex items-center gap-2 hover:opacity-90 transition"
        >
          🐾 Hayvan Pazarı
        </Link>

        {/* SAĞ MENÜ LİNKLERİ */}
        <div className="flex items-center gap-4 md:gap-6">
          {/* Herkesin görebileceği link */}
          <Link to="/" className="hover:text-blue-200 font-medium transition">
            İlanlar
          </Link>

          {user ? (
            // --- GİRİŞ YAPMIŞ KULLANICI ---
            <>
              {/* Favorilerim */}
              <Link
                to="/favorites"
                className="hover:text-pink-200 font-medium flex items-center gap-1 transition"
              >
                ❤️ <span className="hidden md:inline">Favorilerim</span>
              </Link>

              {/* İlan Ver Butonu */}
              <Link
                to="/create-advert"
                className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 font-bold shadow-sm transition flex items-center gap-1"
              >
                <span>+</span>{" "}
                <span className="hidden md:inline">İlan Ver</span>
              </Link>

              {/* Profil Linki */}
              <Link
                to="/profile"
                className="flex items-center gap-2 hover:text-blue-200 transition font-medium"
              >
                <div className="w-8 h-8 bg-blue-800 rounded-full flex items-center justify-center text-sm border border-blue-400">
                  👤
                </div>
                <span className="hidden md:inline">Hesabım</span>
              </Link>

              {/* Çıkış Yap */}
              <button
                onClick={logout}
                className="text-sm bg-blue-700 hover:bg-red-500 px-4 py-2 rounded transition"
              >
                Çıkış
              </button>
            </>
          ) : (
            // --- MİSAFİR KULLANICI ---
            <>
              <Link
                to="/login"
                className="hover:text-blue-200 font-medium transition"
              >
                Giriş Yap
              </Link>
              <Link
                to="/register"
                className="bg-white text-blue-600 px-5 py-2 rounded font-bold hover:bg-gray-100 shadow-sm transition"
              >
                Kayıt Ol
              </Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
