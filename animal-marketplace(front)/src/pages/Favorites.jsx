import { useState, useEffect } from 'react';
import { userService } from '../api/userService';
import { advertService } from '../api/advertService'; // Detay çekmek için lazım
import { ANIMAL_KINDS } from '../utils/constants';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function Favorites() {
  const [favoriteAdverts, setFavoriteAdverts] = useState([]);
  const [loading, setLoading] = useState(true);

  // Favorileri Yükle
  const loadFavorites = async () => {
    try {
      setLoading(true);
      
      // 1. Adım: Favori ID listesini çek (Örn: [3, 5])
      const ids = await userService.getAllFavorites();
      
      if (ids && ids.length > 0) {
        // 2. Adım: Her ID için ilan detayını çek (Paralel İstek)
        // Promise.all ile tüm istekleri aynı anda atıp bitmesini bekliyoruz
        const advertPromises = ids.map(id => advertService.getById(id));
        const advertsData = await Promise.all(advertPromises);
        
        setFavoriteAdverts(advertsData);
      } else {
        setFavoriteAdverts([]);
      }

    } catch (error) {
      console.error("Favoriler yüklenirken hata:", error);
      toast.error("Favoriler getirilemedi.");
    } finally {
      setLoading(false);
    }
  };

  // Favoriden Kaldır
  const handleRemove = async (e, advertId) => {
    e.preventDefault();
    e.stopPropagation();

    try {
      await userService.removeFavorite(advertId);
      // Listeden anında sil
      setFavoriteAdverts(prev => prev.filter(ad => (ad.advertId || ad.id) !== advertId));
      toast.success("Favorilerden kaldırıldı.");
    } catch (error) {
      console.error(error);
      toast.error("Silinemedi.");
    }
  };

  useEffect(() => {
    loadFavorites();
  }, []);

  if (loading) return <div className="text-center mt-20 text-gray-500 font-medium">Favoriler Yükleniyor...</div>;

  return (
    <div className="container mx-auto p-4 min-h-screen">
      <div className="flex items-center gap-2 mb-6 border-b pb-4">
        <h1 className="text-2xl font-bold text-gray-800">❤️ Favori İlanlarım</h1>
        <span className="bg-red-100 text-red-600 px-2 py-1 rounded-full text-xs font-bold">
            {favoriteAdverts.length} İlan
        </span>
      </div>
      
      {favoriteAdverts.length === 0 ? (
        <div className="text-center py-20 bg-gray-50 rounded-lg border-2 border-dashed border-gray-200">
          <p className="text-gray-500 text-lg mb-4">Henüz favorilere eklenmiş bir ilan yok.</p>
          <Link to="/" className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition shadow">
            İlanlara Göz At
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {favoriteAdverts.map(ad => {
            // Veri Uyumluluğu:
            // Home sayfası 'AdvertSimpleDto' kullanırken, burası 'AdvertDetailDto' kullanıyor olabilir.
            // Bu yüzden alan isimlerini "veya" (||) ile kontrol ediyoruz.
            const id = ad.advertId || ad.id; 
            const imgUrl = ad.image?.url || ad.images?.[0]?.url; // Simple'da image, Detail'de images[] olabilir
            const businessName = ad.businessName || ad.sellerBusinessName;
            const city = ad.address?.city || ad.city; // Detail'de city root'ta da olabilir, address objesinde de.
            const district = ad.address?.district || ad.district;

            return (
              <Link to={`/advert/${id}`} key={id} className="group bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-xl transition duration-300 flex flex-col h-full relative">
                
                {/* SİLME BUTONU (ÇÖP KUTUSU) */}
                <button
                  onClick={(e) => handleRemove(e, id)}
                  className="absolute top-2 right-2 z-10 p-2 bg-white/90 backdrop-blur-sm rounded-full text-gray-400 hover:text-red-600 hover:bg-red-50 shadow-md transition transform hover:scale-105"
                  title="Favorilerden Kaldır"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5">
                    <path strokeLinecap="round" strokeLinejoin="round" d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0" />
                  </svg>
                </button>

                {/* RESİM */}
                <div className="h-48 bg-gray-50 relative overflow-hidden flex items-center justify-center p-2">
                  {imgUrl ? (
                    <img 
                        src={imgUrl} 
                        alt={ad.title} 
                        className="w-full h-full object-contain group-hover:scale-105 transition duration-500"
                    />
                  ) : (
                    <div className="text-gray-400 text-sm">Resim Yok</div>
                  )}
                  
                  {city && (
                      <span className="absolute bottom-2 left-2 bg-white/90 px-2 py-1 text-xs font-bold rounded text-gray-700 shadow-sm border">
                        {city}
                      </span>
                  )}
                </div>

                {/* KART İÇERİĞİ */}
                <div className="p-4 flex flex-col flex-grow">
                  
                  {/* Satıcı Adı */}
                  {businessName && (
                      <div className="mb-2">
                          <span className="text-[10px] font-bold text-orange-600 uppercase tracking-wide bg-orange-50 px-2 py-1 rounded border border-orange-100">
                              🏪 {businessName}
                          </span>
                      </div>
                  )}

                  <h3 className="font-bold text-gray-800 truncate mb-1 text-lg" title={ad.title}>
                    {ad.title}
                  </h3>
                  
                  <div className="mb-2">
                     <span className="text-xs font-medium bg-blue-100 text-blue-800 px-2 py-0.5 rounded-full">
                        {ANIMAL_KINDS[ad.animalKind] || "Diğer"}
                     </span>
                  </div>

                  <div className="mt-auto pt-3 border-t border-gray-100 flex justify-between items-center">
                      <span className="text-green-600 font-extrabold text-xl">{ad.price} ₺</span>
                      <span className="text-xs text-blue-600 font-semibold hover:underline flex items-center gap-1">
                        İncele <span className="text-lg leading-3">›</span>
                      </span>
                  </div>
                </div>
              </Link>
            );
          })}
        </div>
      )}
    </div>
  );
}