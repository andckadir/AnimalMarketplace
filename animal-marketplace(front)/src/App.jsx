import { Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import CreateAdvert from './pages/CreateAdvert';
import CreateSeller from './pages/CreateSeller';
import AdvertDetail from './pages/AdvertDetail';
import Favorites from './pages/Favorites'; // Eklendi
import Profile from './pages/Profile'; // Eklendi
import EditAdvert from './pages/EditAdvert'; // Eklendi
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/create-advert" element={<CreateAdvert />} />
        <Route path="/create-seller" element={<CreateSeller />} />
        <Route path="/advert/:id" element={<AdvertDetail />} />
        <Route path="/favorites" element={<Favorites />} /> {/* Eklendi */}
        <Route path="/profile" element={<Profile />} />
        <Route path="/edit-advert/:id" element={<EditAdvert />} />
      </Routes>
      <ToastContainer position="bottom-right" />
    </div>
  );
}

export default App;