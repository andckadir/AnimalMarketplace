import { createContext, useState, useEffect, useContext } from 'react';
import { sellerService } from '../api/sellerService';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isSeller, setIsSeller] = useState(false); // Yeni state
  const [loading, setLoading] = useState(true); // Kontrol bitene kadar bekle

  const checkUserStatus = async (token) => {
    if (token) {
      localStorage.setItem('token', token);
      // 1. Kullanıcı var sayalım
      setUser({ token }); 
      
      // 2. Satıcı mı diye kontrol et (Backend isteği)
      const sellerData = await sellerService.getSeller();
      if (sellerData) {
        setIsSeller(true);
      } else {
        setIsSeller(false);
      }
    } else {
      localStorage.removeItem('token');
      setUser(null);
      setIsSeller(false);
    }
    setLoading(false);
  };

  useEffect(() => {
    const token = localStorage.getItem('token');
    checkUserStatus(token);
  }, []);

  const login = async (newToken) => {
    await checkUserStatus(newToken);
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
    setIsSeller(false);
    window.location.href = "/";
  };
  
  // Satıcı olduktan sonra state'i manuel güncellemek için
  const refreshSellerStatus = async () => {
    const sellerData = await sellerService.getSeller();
    setIsSeller(!!sellerData);
  };

  return (
    <AuthContext.Provider value={{ user, isSeller, login, logout, refreshSellerStatus, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);