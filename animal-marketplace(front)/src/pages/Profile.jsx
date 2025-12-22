import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { userService } from '../api/userService';
import { sellerService } from '../api/sellerService';
import { advertService } from '../api/advertService';
import { useAuth } from '../context/AuthContext';
import { toast } from 'react-toastify';
import { Link, useNavigate } from 'react-router-dom';
import { ANIMAL_KINDS } from '../utils/constants';

export default function Profile() {
  const { user, isSeller, logout } = useAuth();
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState('info');
  const [myAdverts, setMyAdverts] = useState([]);
  const [loadingAdverts, setLoadingAdverts] = useState(false);

  // --- STATE'LER (HESAP SİLME İŞLEMLERİ İÇİN) ---
  
  // 1. Tam Hesap Silme State'leri
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [deletePassword, setDeletePassword] = useState("");

  // 2. Satıcı Hesabı Silme State'leri (YENİ)
  const [showSellerDeleteConfirm, setShowSellerDeleteConfirm] = useState(false);
  const [sellerDeletePassword, setSellerDeletePassword] = useState("");
  
  // ---------------------------------------------

  const { register: registerInfo, handleSubmit: submitInfo, setValue: setInfoValue } = useForm();
  const { register: registerPass, handleSubmit: submitPass, reset: resetPass } = useForm();

  // VERİLERİ YÜKLE
  useEffect(() => {
    const loadData = async () => {
      try {
        const userData = await userService.getUser();
        setInfoValue("name", userData.name);
        setInfoValue("surname", userData.surname);
        setInfoValue("email", userData.email);
        setInfoValue("phone", userData.phone);
        setInfoValue("gender", userData.gender);

        if (isSeller) {
            const sellerRes = await sellerService.getSeller();
            if (sellerRes) {
                setInfoValue("businessName", sellerRes.businessName); 
                fetchMyAdverts(sellerRes.businessName);
            }
        }
      } catch (error) {
        console.error("Profil yüklenemedi", error);
      }
    };
    loadData();
  }, [isSeller, setInfoValue]);

  const fetchMyAdverts = async (businessName) => {
    setLoadingAdverts(true);
    try {
        const result = await advertService.getAdverts({ businessName }, 1, 100);
        setMyAdverts(result.data || []);
    } catch (error) { console.error(error); } finally { setLoadingAdverts(false); }
  };

  const onUpdateInfo = async (data) => {
    try {
      const userPayload = {
        name: data.name,
        surname: data.surname,
        email: data.email,
        phone: data.phone,
        gender: Number(data.gender)
      };
      await userService.updateUser(userPayload);

      if (isSeller && data.businessName) {
         await sellerService.updateSeller({ businessName: data.businessName });
      }
      toast.success("Profil bilgileri güncellendi.");
    } catch (error) {
      toast.error("Güncelleme sırasında hata oluştu.");
    }
  };

  const onChangePassword = async (data) => {
    if (data.newPassword !== data.confirmNewPassword) {
      toast.error("Yeni şifreler uyuşmuyor!");
      return;
    }
    try {
      await userService.changePassword(data);
      toast.success("Şifre başarıyla değiştirildi.");
      resetPass();
    } catch (error) {
      toast.error("Şifre değiştirilemedi.");
    }
  };

  const onDeleteAdvert = async (id) => {
    if (!window.confirm("Bu ilanı silmek istediğinize emin misiniz?")) return;
    try {
      await advertService.deleteAdvert(id);
      setMyAdverts(prev => prev.filter(ad => ad.advertId !== id));
      toast.success("İlan silindi.");
    } catch (error) {
      toast.error("Silme işlemi başarısız.");
    }
  };

  // --- HESAP SİLME FONKSİYONLARI ---

  // 1. Satıcı Hesabı Kapatma (ŞİFRELİ)
  // 1. Satıcı Hesabı Kapatma (ŞİFRELİ - ÇIKIŞ YAPMAZ)
  // Profile.jsx içindeki handleDeleteSellerAccount fonksiyonunu bul ve güncelle:

  const handleDeleteSellerAccount = async () => {
    if (!sellerDeletePassword) {
        toast.warn("Lütfen şifrenizi girin.");
        return;
    }

    try {
        await sellerService.deleteSeller(sellerDeletePassword);
        toast.success("Satıcı hesabı kapatıldı.");
        
        // --- DEĞİŞİKLİK BURADA ---
        // Sayfayı yenilemek yerine (reload), manuel olarak satıcı modundan çıkalım.
        // Bu, token yenilenene kadar geçici bir çözümdür.
        
        // 1. Satıcı form alanını temizle
        setInfoValue("businessName", ""); 
        
        // 2. İlanları temizle
        setMyAdverts([]);

        // 3. Kullanıcıya bilgi verip sayfayı yenile (Token düzelmesi için çıkış-giriş gerekir ama şimdilik yenileme yeter)
        setTimeout(() => {
           window.location.reload(); 
        }, 1000);
        // -------------------------
        
    } catch (error) {
        toast.error("İşlem başarısız. Şifre yanlış olabilir.");
    }
  };

  // 2. Tam Hesap Silme (ŞİFRELİ - ÇIKIŞ YAPAR)
  const handleDeleteFullAccount = async () => {
    if (!deletePassword) {
        toast.warn("Lütfen şifrenizi girin.");
        return;
    }

    try {
        await userService.deleteUser(deletePassword);
        toast.success("Hesabınız silindi. Güle güle!");
        
        // Hesap yok olduğu için çıkış yapmak zorunlu
        logout();
        navigate('/');
    } catch (error) {
        toast.error("Hesap silinemedi. Şifre yanlış olabilir.");
    }
  };

  return (
    <div className="container mx-auto p-4 md:p-10 min-h-screen bg-gray-50">
      <div className="flex flex-col md:flex-row gap-6">
        
        {/* SOL MENÜ */}
        <div className="w-full md:w-1/4 bg-white p-6 rounded-lg shadow h-fit">
          <div className="text-center mb-6">
            <div className="w-20 h-20 bg-blue-100 rounded-full mx-auto flex items-center justify-center text-3xl">👤</div>
            <h2 className="mt-2 font-bold text-lg">Hesabım</h2>
            {isSeller && <span className="text-xs bg-orange-100 text-orange-600 px-2 py-1 rounded font-bold">SATICI HESABI</span>}
          </div>
          
          <nav className="space-y-2">
            <button onClick={() => setActiveTab('info')} className={`w-full text-left p-3 rounded transition ${activeTab === 'info' ? 'bg-blue-50 text-blue-600 font-bold' : 'hover:bg-gray-50'}`}>📝 Profil Bilgileri</button>
            <button onClick={() => setActiveTab('password')} className={`w-full text-left p-3 rounded transition ${activeTab === 'password' ? 'bg-blue-50 text-blue-600 font-bold' : 'hover:bg-gray-50'}`}>🔒 Şifre Değiştir</button>
            {isSeller && <button onClick={() => setActiveTab('adverts')} className={`w-full text-left p-3 rounded transition ${activeTab === 'adverts' ? 'bg-orange-50 text-orange-600 font-bold' : 'hover:bg-gray-50'}`}>📋 İlanlarım</button>}
            <button onClick={logout} className="w-full text-left p-3 rounded text-red-600 hover:bg-red-50 mt-4 border-t">🚪 Çıkış Yap</button>
          </nav>
        </div>

        {/* SAĞ İÇERİK */}
        <div className="w-full md:w-3/4 bg-white p-6 rounded-lg shadow">
            
            {/* --- TAB 1: PROFİL BİLGİLERİ --- */}
            {activeTab === 'info' && (
                <div className="space-y-8">
                    <form onSubmit={submitInfo(onUpdateInfo)} className="space-y-4 max-w-lg">
                        <h3 className="text-xl font-bold mb-4 border-b pb-2">Profil Bilgilerini Güncelle</h3>
                        
                        {isSeller && (
                            <div className="bg-orange-50 p-4 rounded border border-orange-200 mb-6">
                                <label className="block text-sm font-bold text-orange-800 mb-1">🏪 İşletme / Mağaza Adı</label>
                                <input {...registerInfo("businessName")} className="w-full border p-2 rounded focus:ring-2 focus:ring-orange-500 outline-none uppercase font-bold text-gray-700" placeholder="ŞİRKET ADI"/>
                            </div>
                        )}

                        <div className="grid grid-cols-2 gap-4">
                            <div><label className="block text-sm text-gray-600">Ad</label><input {...registerInfo("name")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                            <div><label className="block text-sm text-gray-600">Soyad</label><input {...registerInfo("surname")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                        </div>
                        
                        <div><label className="block text-sm text-gray-600">Email</label><input {...registerInfo("email")} type="email" className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                        <div><label className="block text-sm text-gray-600">Telefon</label><input {...registerInfo("phone")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                        <div>
                            <label className="block text-sm text-gray-600">Cinsiyet</label>
                            <select {...registerInfo("gender")} className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none bg-white">
                                <option value="0">Kadın</option>
                                <option value="1">Erkek</option>
                            </select>
                        </div>
                        <button type="submit" className="bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700 font-bold transition w-full md:w-auto">Bilgileri Güncelle</button>
                    </form>

                    {/* --- HESAP SİLME BUTONLARI --- */}
                    <div className="pt-8 mt-8 border-t border-gray-200">
                        <h4 className="text-red-600 font-bold mb-4">⚠️ Hesap İşlemleri</h4>
                        <div className="flex flex-col gap-4">
                            
                            {/* 1. SATICI HESABINI KAPAT (Şifreli) */}
                            {isSeller && (
                                <div className="bg-orange-50 p-4 rounded border border-orange-100">
                                    {!showSellerDeleteConfirm ? (
                                        <div className="flex flex-col sm:flex-row justify-between items-center gap-4">
                                            <div>
                                                <span className="text-sm font-bold text-orange-800 block">Satıcı Hesabını Kapat</span>
                                                <span className="text-xs text-orange-700">İlanlarınız silinir, normal üye olarak kalırsınız.</span>
                                            </div>
                                            <button onClick={() => setShowSellerDeleteConfirm(true)} className="text-xs bg-orange-100 text-orange-700 px-4 py-2 rounded hover:bg-orange-200 font-bold transition whitespace-nowrap">
                                                Satıcılığı Bırak
                                            </button>
                                        </div>
                                    ) : (
                                        <div className="animate-fade-in">
                                            <p className="text-sm font-bold text-orange-800 mb-2">İşlemi onaylamak için şifrenizi girin:</p>
                                            <div className="flex flex-col sm:flex-row gap-2">
                                                <input 
                                                    type="password" 
                                                    placeholder="Şifreniz" 
                                                    className="border p-2 rounded text-sm flex-grow outline-none focus:ring-2 focus:ring-orange-500"
                                                    value={sellerDeletePassword}
                                                    onChange={(e) => setSellerDeletePassword(e.target.value)}
                                                />
                                                <button onClick={handleDeleteSellerAccount} className="bg-orange-600 text-white px-4 py-2 rounded font-bold text-sm hover:bg-orange-700 transition">
                                                    Onayla
                                                </button>
                                                <button 
                                                    onClick={() => {
                                                        setShowSellerDeleteConfirm(false);
                                                        setSellerDeletePassword("");
                                                    }} 
                                                    className="bg-gray-200 text-gray-700 px-4 py-2 rounded font-bold text-sm hover:bg-gray-300 transition"
                                                >
                                                    Vazgeç
                                                </button>
                                            </div>
                                        </div>
                                    )}
                                </div>
                            )}

                            {/* 2. HESABI TAMAMEN SİL (Şifreli) */}
                            <div className="bg-red-50 p-4 rounded border border-red-100">
                                {!showDeleteConfirm ? (
                                    <div className="flex flex-col sm:flex-row justify-between items-center gap-4">
                                        <div>
                                            <span className="text-sm font-bold text-red-800 block">Hesabı Tamamen Sil</span>
                                            <span className="text-xs text-red-700">Tüm verileriniz kalıcı olarak silinir.</span>
                                        </div>
                                        <button onClick={() => setShowDeleteConfirm(true)} className="text-xs bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700 font-bold transition whitespace-nowrap">
                                            Hesabımı Sil
                                        </button>
                                    </div>
                                ) : (
                                    <div className="animate-fade-in">
                                        <p className="text-sm font-bold text-red-800 mb-2">Güvenliğiniz için lütfen şifrenizi girin:</p>
                                        <div className="flex flex-col sm:flex-row gap-2">
                                            <input 
                                                type="password" 
                                                placeholder="Şifreniz" 
                                                className="border p-2 rounded text-sm flex-grow outline-none focus:ring-2 focus:ring-red-500"
                                                value={deletePassword}
                                                onChange={(e) => setDeletePassword(e.target.value)}
                                            />
                                            <button onClick={handleDeleteFullAccount} className="bg-red-600 text-white px-4 py-2 rounded font-bold text-sm hover:bg-red-700 transition">
                                                Onayla ve Sil
                                            </button>
                                            <button 
                                                onClick={() => {
                                                    setShowDeleteConfirm(false);
                                                    setDeletePassword("");
                                                }} 
                                                className="bg-gray-200 text-gray-700 px-4 py-2 rounded font-bold text-sm hover:bg-gray-300 transition"
                                            >
                                                Vazgeç
                                            </button>
                                        </div>
                                    </div>
                                )}
                            </div>

                        </div>
                    </div>
                </div>
            )}

            {activeTab === 'password' && (
                <form onSubmit={submitPass(onChangePassword)} className="space-y-4 max-w-lg">
                    <h3 className="text-xl font-bold mb-4 border-b pb-2">Şifre Değiştir</h3>
                    <div><label className="block text-sm text-gray-600">Eski Şifre</label><input type="password" {...registerPass("oldPassword")} required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                    <div><label className="block text-sm text-gray-600">Yeni Şifre</label><input type="password" {...registerPass("newPassword")} required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                    <div><label className="block text-sm text-gray-600">Yeni Şifre (Tekrar)</label><input type="password" {...registerPass("confirmNewPassword")} required className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none"/></div>
                    <button type="submit" className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700 font-bold transition w-full md:w-auto">Şifreyi Güncelle</button>
                </form>
            )}

            {activeTab === 'adverts' && isSeller && (
                <div>
                     <div className="flex justify-between items-center mb-4 border-b pb-2">
                        <h3 className="text-xl font-bold">Yayındaki İlanlarım</h3>
                        <Link to="/create-advert" className="bg-orange-500 text-white text-sm px-3 py-1 rounded hover:bg-orange-600 font-bold">+ Yeni İlan</Link>
                    </div>
                    {loadingAdverts ? <p>Yükleniyor...</p> : (
                        <div className="space-y-4">
                            {myAdverts.length === 0 && <p className="text-gray-500">Henüz ilanınız yok.</p>}
                            {myAdverts.map(ad => (
                                <div key={ad.advertId} className="flex items-center justify-between border p-4 rounded hover:shadow-md transition bg-gray-50">
                                    <div className="flex items-center gap-4">
                                        <div className="w-16 h-16 bg-gray-200 rounded overflow-hidden flex items-center justify-center">
                                            {ad.image?.url ? <img src={ad.image.url} className="w-full h-full object-contain" alt="ilan"/> : <span className="text-xs text-gray-500">Resim Yok</span>}
                                        </div>
                                        <div>
                                            <h4 className="font-bold text-gray-800">{ad.title}</h4>
                                            <p className="text-sm text-green-600 font-bold">{ad.price} ₺</p>
                                        </div>
                                    </div>
                                    <div className="flex gap-2">
                                        <Link to={`/edit-advert/${ad.advertId}`} className="bg-blue-100 text-blue-600 px-3 py-1 rounded text-sm hover:bg-blue-200 font-medium">Düzenle</Link>
                                        <button onClick={() => onDeleteAdvert(ad.advertId)} className="bg-red-100 text-red-600 px-3 py-1 rounded text-sm hover:bg-red-200 font-medium">Sil</button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            )}
        </div>
      </div>
    </div>
  );
}