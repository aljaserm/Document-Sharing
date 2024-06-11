import React, { useState } from 'react';
import { generateShareLink } from '../services/api';
import '../styles/modal.css';

const ShareModal = ({ document, onClose }) => {
    const [duration, setDuration] = useState('');
    const [unit, setUnit] = useState('Minutes');
    const [shareLink, setShareLink] = useState('');
    const [error, setError] = useState(null);
    const [copySuccess, setCopySuccess] = useState('');

    const handleShare = async () => {
        try {
            const link = await generateShareLink({ documentId: document.id, duration, unit });
            setShareLink(link);
            setCopySuccess('URL copied successfully!');
        } catch (err) {
            console.error('Error generating share link:', err);
            setError(err.response?.data?.message || 'Error generating share link');
        }
    };

    const handleDurationChange = (e) => {
        const value = e.target.value;
        if (!isNaN(value) && value >= 0) {
            setDuration(value);
        }
    };

    const handleCopy = () => {
        navigator.clipboard.writeText(shareLink.link).then(() => {
            setCopySuccess('URL copied successfully!');
        });
    };

    return (
        <div className="modal-overlay">
            <div className="modal">
                <button className="modal-close" onClick={onClose}>Close X</button>
                <h2>Share Document</h2>
                {error && <div className="alert error">{error}</div>}
                <div className="form-group">
                    <label htmlFor="duration">Duration:</label>
                    <input
                        type="number"
                        id="duration"
                        value={duration}
                        onChange={handleDurationChange}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="unit">Unit:</label>
                    <select id="unit" value={unit} onChange={(e) => setUnit(e.target.value)}>
                        <option value="Minutes">Minutes</option>
                        <option value="Hours">Hours</option>
                        <option value="Days">Days</option>
                        <option value="Weeks">Weeks</option>
                    </select>
                </div>
                <button onClick={handleShare}>Generate Link</button>
                {shareLink && (
                    <div>
                        <input type="text" value={shareLink.link} readOnly />
                        <button onClick={handleCopy}>Copy</button>
                        {copySuccess && <p>{copySuccess}</p>}
                    </div>
                )}
            </div>
        </div>
    );
};

export default ShareModal;
