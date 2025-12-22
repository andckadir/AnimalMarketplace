import { useState, useEffect } from 'react';
import { advertService } from '../api/advertService';
import { userService } from '../api/userService';
import { ANIMAL_KINDS } from '../utils/constants';
import { TR_CITIES } from '../utils/locations';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { toast } from 'react-toastify';

export default function Home() {
  const { user } = useAuth();
  
  const [adverts, setAdverts] = useState([]);
  const [favorites, setFavorites] = useState([]); // Favori ID listesi
  const [loading, setLoading] = useState(true);
  const [districts, setDistricts] = useState([]);

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize] = useState(12);

  const [filters, setFilters] = useState({
    city: null,
    district: null,
    minPrice: null,
    maxPrice: null,
    animalKind: null,
    gender: null,
    minAge: null,
    maxAge: null,
    businessName: null
  });

  // --- FAVORİLERİ ÇEK (DÜZELTİLDİ) ---
  const fetchFavorites = async () => {
    if (!user) {
      setFavorites([]);
      return;
    }
    try {
      const data = await userService.getAllFavorites();
      
      // Backend bazen [1, 2] bazen [{advertId:1}, {advertId:2}] dönebilir.
      // Her iki durumu da kapsayan güvenli çözüm:
      if (Array.isArray(data)) {
        const ids = data.map(item => {
          // Eğer item bir sayı ise (örn: 5) direkt onu al
          if (typeof item === 'number') return item;
          // Eğer string ise sayıya çevir
          if (typeof item === 'string') return Number(item);
          // Eğer obje ise (örn: {advertId: 5}) içindeki ID'yi al
          return item.advertId || item.id;
        });
        
        // Null veya undefined olanları temizle ve kaydet
        setFavorites(ids.filter(id => id !== null && id !== undefined));
      }
    } catch (error) {
      console.error("Favoriler çekilemedi", error);
    }
  };

  // --- İLANLARI GETİR ---
  const fetchAdverts = async (pageNo = 1) => {
    setLoading(true);
    try {
      const result = await advertService.getAdverts(filters, pageNo, pageSize);
      setAdverts(result.data || []);
      setTotalPages(result.totalPages || 1);
      setCurrentPage(result.pageNumber || 1);
    } catch (error) {
      console.error("Hata:", error);
    } finally {
      setLoading(false);
    }
  };

  // --- FAVORİ TOGGLE (EKLE/ÇIKAR) ---
  const handleToggleFavorite = async (e, advertId) => {
    e.preventDefault(); 
    e.stopPropagation();

    if (!user) {
      toast.info("Favorilere eklemek için giriş yapmalısınız.");
      return;
    }

    try {
      if (favorites.includes(advertId)) {
        // Çıkarma
        await userService.removeFavorite(advertId);
        setFavorites(prev => prev.filter(id => id !== advertId));
        toast.success("Favorilerden çıkarıldı.");
      } else {
        // Ekleme
        await userService.addFavorite(advertId);
        setFavorites(prev => [...prev, advertId]);
        toast.success("Favorilere eklendi.");
      }
    } catch (error) {
      console.error(error);
      toast.error("İşlem başarısız.");
    }
  };

  useEffect(() => {
    fetchAdverts(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Sadece ilk açılışta ilanları çek

  // Kullanıcı değiştiğinde (Giriş yapınca) favorileri çek
  useEffect(() => {
    fetchFavorites();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user]);

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setCurrentPage(newPage);
      fetchAdverts(newPage);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    if (name === "city") {
      const selectedCity = TR_CITIES.find(c => c.name === value);
      setDistricts(selectedCity ? selectedCity.districts : []);
      setFilters(prev => ({ ...prev, city: value === "" ? null : value, district: null }));
    } else if (name === "businessName") {
      setFilters(prev => ({ ...prev, businessName: value === "" ? null : value.toUpperCase() }));
    } else {
      setFilters(prev => ({
        ...prev,
        [name]: value === "" ? null : (['animalKind', 'gender', 'minAge', 'maxAge', 'minPrice', 'maxPrice'].includes(name) ? Number(value) : value)
      }));
    }
  };

  return (
    <div className="container mx-auto p-4 flex flex-col md:flex-row gap-6">
      
      {/* SOL FİLTRE */}
      <div className="w-full md:w-1/4 bg-white p-5 rounded-lg shadow-lg h-fit border border-gray-100">
        <h2 className="text-xl font-bold mb-5 text-gray-800 flex items-center gap-2">🔍 Detaylı Filtrele</h2>
        
        <div className="space-y-4">
          <div>
            <label className="text-sm text-gray-600 font-semibold">Hayvan Türü</label>
            <select name="animalKind" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none focus:ring-2 focus:ring-blue-100">
              <option value="">Tümü</option>
              {Object.entries(ANIMAL_KINDS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
            </select>
          </div>

          <div>
            <label className="text-sm text-gray-600 font-semibold">Cinsiyet</label>
            <select name="gender" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none focus:ring-2 focus:ring-blue-100">
              <option value="">Tümü</option>
              <option value="0">Dişi</option>
              <option value="1">Erkek</option>
            </select>
          </div>

          <div>
            <label className="text-sm text-gray-600 font-semibold">Şehir</label>
            <select name="city" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none focus:ring-2 focus:ring-blue-100">
              <option value="">Tüm Türkiye</option>
              {TR_CITIES.map((city) => <option key={city.name} value={city.name}>{city.name}</option>)}
            </select>
          </div>

          <div>
            <label className="text-sm text-gray-600 font-semibold">İlçe</label>
            <select name="district" onChange={handleFilterChange} disabled={!filters.city} value={filters.district || ""} className="w-full border p-2 rounded outline-none disabled:bg-gray-100 focus:ring-2 focus:ring-blue-100">
              <option value="">Tümü</option>
              {districts.map((d) => <option key={d} value={d}>{d}</option>)}
            </select>
          </div>

          <div className="grid grid-cols-2 gap-2">
            <div><label className="text-xs text-gray-500">Min Fiyat</label><input type="number" name="minPrice" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none"/></div>
            <div><label className="text-xs text-gray-500">Max Fiyat</label><input type="number" name="maxPrice" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none"/></div>
          </div>

          <div className="grid grid-cols-2 gap-2">
            <div><label className="text-xs text-gray-500">Min Yaş</label><input type="number" name="minAge" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none"/></div>
            <div><label className="text-xs text-gray-500">Max Yaş</label><input type="number" name="maxAge" onChange={handleFilterChange} className="w-full border p-2 rounded outline-none"/></div>
          </div>

          <div>
             <label className="text-sm text-gray-600 font-semibold">Satıcı Adı</label>
             <input type="text" name="businessName" onChange={handleFilterChange} placeholder="Örn: PET SHOP" className="w-full border p-2 rounded outline-none uppercase"/>
          </div>

          <button onClick={() => fetchAdverts(1)} className="w-full bg-blue-600 text-white py-3 rounded font-bold hover:bg-blue-700 transition shadow-md">Sonuçları Getir</button>
        </div>
      </div>

      {/* SAĞ İLAN LİSTESİ */}
      <div className="w-full md:w-3/4 flex flex-col justify-between">
        <div>
          {loading ? (
              <div className="flex justify-center items-center h-64 text-gray-500">Yükleniyor...</div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              {adverts.length === 0 && (
                  <div className="col-span-full text-center py-10 bg-white rounded shadow">
                      <p className="text-gray-500 text-lg">Bu kriterlere uygun ilan bulunamadı.</p>
                  </div>
              )}
              {adverts.map(ad => {
                // FAVORİ KONTROLÜ
                const isFav = favorites.includes(ad.advertId);
                
                return (
                <Link to={`/advert/${ad.advertId}`} key={ad.advertId} className="group bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-xl transition duration-300 flex flex-col h-full relative">
                  
                  {/* FAVORİ BUTONU */}
                  <button
                    onClick={(e) => handleToggleFavorite(e, ad.advertId)}
                    className="absolute top-2 right-2 z-10 p-2 bg-white/80 rounded-full hover:bg-white shadow-sm transition transform hover:scale-110"
                    title={isFav ? "Favorilerden Çıkar" : "Favorilere Ekle"}
                  >
                    {isFav ? (
                        // DOLU KALP (KIRMIZI)
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" className="w-6 h-6 text-red-500">
                          <path d="m11.645 20.91-.007-.003-.022-.012a15.247 15.247 0 0 1-.383-.218 25.18 25.18 0 0 1-4.244-3.17C4.688 15.36 2.25 12.174 2.25 8.25 2.25 5.322 4.714 3 7.688 3A5.5 5.5 0 0 1 12 5.052 5.5 5.5 0 0 1 16.313 3c2.973 0 5.437 2.322 5.437 5.25 0 3.925-2.438 7.111-4.739 9.256a25.175 25.175 0 0 1-4.244 3.17 15.247 15.247 0 0 1-.383.219l-.022.012-.007.004-.003.001a.752.752 0 0 1-.704 0l-.003-.001Z" />
                        </svg>
                    ) : (
                        // BOŞ KALP
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6 text-gray-500 hover:text-red-500">
                          <path strokeLinecap="round" strokeLinejoin="round" d="M21 8.25c0-2.485-2.099-4.5-4.688-4.5-1.935 0-3.597 1.126-4.312 2.733-.715-1.607-2.377-2.733-4.313-2.733C5.1 3.75 3 5.765 3 8.25c0 7.22 9 12 9 12s9-4.78 9-12Z" />
                        </svg>
                    )}
                  </button>

                  {/* RESİM */}
                  <div className="h-52 bg-gray-50 relative overflow-hidden flex items-center justify-center p-2">
                    {ad.image?.url ? (
                      <img src={ad.image.url} alt={ad.title} className="w-full h-full object-contain group-hover:scale-105 transition duration-500"/>
                    ) : (
                      <div className="text-gray-400">Resim Yok</div>
                    )}
                    <span className="absolute bottom-2 left-2 bg-white/90 px-2 py-1 text-xs font-bold rounded text-gray-700 shadow-sm border">
                      {ad.address?.city}
                    </span>
                  </div>

                  {/* BİLGİLER */}
                  <div className="p-4 flex flex-col flex-grow">
                    {ad.businessName && (
                        <div className="mb-2">
                            <span className="text-xs font-bold text-orange-600 uppercase tracking-wide bg-orange-50 px-2 py-1 rounded">
                                🏪 {ad.businessName}
                            </span>
                        </div>
                    )}
                    <h3 className="font-bold text-lg text-gray-800 truncate mb-2" title={ad.title}>{ad.title}</h3>
                    <div className="flex items-center gap-2 mb-3">
                        <span className="text-xs font-medium bg-blue-100 text-blue-800 px-2 py-0.5 rounded-full whitespace-nowrap">
                            {ANIMAL_KINDS[ad.animalKind]}
                        </span>
                    </div>
                    <div className="mt-auto">
                        <p className="text-green-600 font-extrabold text-xl mb-3">{ad.price} ₺</p>
                        <div className="flex items-center justify-between text-sm text-gray-500 border-t pt-3">
                            <span className="flex items-center gap-1 truncate max-w-[150px]">📍 {ad.address?.district}</span>
                            <span className="text-xs text-blue-600 font-semibold hover:underline">İncele →</span>
                        </div>
                    </div>
                  </div>
                </Link>
              );})}
            </div>
          )}
        </div>

        {/* PAGINATION */}
        <div className="flex justify-center items-center mt-8 gap-2 py-4 bg-gray-50 rounded border border-gray-100">
            <button 
                onClick={() => handlePageChange(currentPage - 1)}
                disabled={currentPage === 1}
                className={`px-4 py-2 rounded border text-sm font-medium transition ${currentPage === 1 ? 'bg-gray-200 text-gray-400 cursor-not-allowed' : 'bg-white text-gray-700 hover:bg-blue-50 hover:text-blue-600 border-gray-300'}`}
            >
                &lt; Önceki
            </button>

            <div className="flex items-center gap-2 px-4">
               <span className="text-gray-600 font-medium">Sayfa</span>
               <span className="font-bold text-gray-900">{currentPage}</span>
               <span className="text-gray-400">/</span>
               <span className="text-gray-600">{totalPages}</span>
            </div>

            <button 
                onClick={() => handlePageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
                className={`px-4 py-2 rounded border text-sm font-medium transition ${currentPage === totalPages ? 'bg-gray-200 text-gray-400 cursor-not-allowed' : 'bg-white text-gray-700 hover:bg-blue-50 hover:text-blue-600 border-gray-300'}`}
            >
                Sonraki &gt;
            </button>
        </div>

      </div>
    </div>
  );
}