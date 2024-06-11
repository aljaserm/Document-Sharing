import React from 'react';
import { Route, Routes } from 'react-router-dom';
import HomePage from './pages/HomePage';
import DocumentPage from './pages/DocumentPage';
import UploadPage from './pages/UploadPage';
import NavBar from './components/NavBar';

const App = () => {
    return (
        <div>
            <NavBar />
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/documents" element={<DocumentPage />} />
                <Route path="/upload" element={<UploadPage />} />
            </Routes>
        </div>
    );
};

export default App;
