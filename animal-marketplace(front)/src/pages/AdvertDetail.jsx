import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { advertService } from '../api/advertService';
import { ANIMAL_KINDS, GENDER } from '../utils/constants';

export default function AdvertDetail() {
  const { id } = useParams();
  const [advert, setAdvert] = useState(null);
  const [loading, setLoading] = useState(true);
  
  // Resim Slider State'i
  const [currentImageIndex, setCurrentImageIndex] = useState(0);

  useEffect(() => {
    const fetchDetail = async () => {
      try {
        const data = await advertService.getById(id);
        setAdvert(data);
      } catch (error) {
        console.error("İlan detayı çekilemedi:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id]);

  if (loading) return <div className="text-center mt-20 text-gray-500">Yükleniyor...</div>;
  if (!advert) return <div className="text-center mt-20 text-red-500">İlan bulunamadı!</div>;

  // --- RESİM SLIDER FONKSİYONLARI ---
  const images = advert.images || [];
  
  const nextImage = () => {
    setCurrentImageIndex((prev) => (prev + 1) % images.length);
  };

  const prevImage = () => {
    setCurrentImageIndex((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  };
  // ----------------------------------

  return (
    <div className="container mx-auto p-4 md:p-8">
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 bg-white rounded-lg shadow-lg overflow-hidden">
        
        {/* --- SOL TARAFT: RESİM GALERİSİ --- */}
        <div className="relative bg-gray-100 flex items-center justify-center min-h-[400px] max-h-[600px]">
          {images.length > 0 ? (
            <>
              <img 
                src={images[currentImageIndex].url} 
                alt={advert.title} 
                className="w-full h-full object-contain max-h-[600px]"
              />
              
              {/* Birden fazla resim varsa OK tuşlarını göster */}
              {images.length > 1 && (
                <>
                  <button 
                    onClick={prevImage}
                    className="absolute left-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white p-3 rounded-full hover:bg-black/70 transition"
                  >
                    ❮
                  </button>
                  <button 
                    onClick={nextImage}
                    className="absolute right-4 top-1/2 transform -translate-y-1/2 bg-black/50 text-white p-3 rounded-full hover:bg-black/70 transition"
                  >
                    ❯
                  </button>
                  
                  {/* Alt nokta göstergeleri */}
                  <div className="absolute bottom-4 flex gap-2">
                    {images.map((_, idx) => (
                      <div 
                        key={idx} 
                        className={`w-3 h-3 rounded-full ${idx === currentImageIndex ? 'bg-white' : 'bg-white/50'}`}
                      />
                    ))}
                  </div>
                </>
              )}
            </>
          ) : (
            <div className="text-gray-400">Görsel Yok</div>
          )}
        </div>

        {/* --- SAĞ TARAF: İLAN DETAYLARI --- */}
        <div className="p-6 space-y-6">
            
            {/* Başlık ve Fiyat */}
            <div className="border-b pb-4">
                <h1 className="text-3xl font-bold text-gray-900 mb-2">{advert.title}</h1>
                <div className="flex justify-between items-end">
                    <span className="text-3xl font-extrabold text-green-600">{advert.price.toLocaleString()} ₺</span>
                    <span className="text-sm text-gray-500">
                        {new Date(advert.date).toLocaleDateString('tr-TR')}
                    </span>
                </div>
            </div>

            {/* Satıcı Bilgi Kartı */}
            <div className="bg-orange-50 border border-orange-200 p-4 rounded-lg">
                <h3 className="text-sm font-bold text-orange-800 uppercase mb-2 border-b border-orange-200 pb-1">
                    Satıcı Bilgileri
                </h3>
                <div className="flex flex-col gap-1">
                    <span className="font-bold text-lg text-gray-800 uppercase">
                        🏪 {advert.sellerBusinessName}
                    </span>
                    <span className="text-gray-700">👤 {advert.sellerName} {advert.sellerSurname}</span>
                    <span className="text-gray-700 font-mono">📞 {advert.sellerPhone}</span>
                </div>
            </div>

            {/* Hayvan Özellikleri Grid */}
            <div className="grid grid-cols-2 gap-4 bg-gray-50 p-4 rounded-lg border">
                <div>
                    <span className="block text-xs text-gray-500 uppercase">Tür</span>
                    <span className="font-semibold text-gray-800">
                        {ANIMAL_KINDS[advert.animalKind] || "Belirtilmemiş"}
                    </span>
                </div>
                <div>
                    <span className="block text-xs text-gray-500 uppercase">Yaş</span>
                    <span className="font-semibold text-gray-800">
                        {advert.animalAge} Yaşında
                    </span>
                </div>
                <div>
                    <span className="block text-xs text-gray-500 uppercase">Cinsiyet</span>
                    <span className="font-semibold text-gray-800">
                        {GENDER[advert.animalGender] || "Belirtilmemiş"}
                    </span>
                </div>
                <div>
                    <span className="block text-xs text-gray-500 uppercase">Konum</span>
                    <span className="font-semibold text-gray-800">
                        {advert.address?.city} / {advert.address?.district}
                    </span>
                </div>
            </div>

            {/* Açıklama */}
            <div>
                <h3 className="font-bold text-gray-900 mb-2">Açıklama</h3>
                <p className="text-gray-700 leading-relaxed whitespace-pre-line bg-white border p-4 rounded min-h-[100px]">
                    {advert.description}
                </p>
            </div>

            {/* İletişim Butonu */}
            <a 
                href={`tel:${advert.sellerPhone}`}
                className="block w-full text-center bg-blue-600 text-white font-bold py-3 rounded-lg hover:bg-blue-700 transition shadow-md"
            >
                Satıcıyı Ara ({advert.sellerPhone})
            </a>

        </div>
      </div>
    </div>
  );
}