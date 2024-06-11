import React, { useEffect, useState } from 'react';
import { getDocuments, downloadDocument, downloadMultipleDocuments } from '../services/api';
import ShareModal from '../components/ShareModal';
import '../styles/style.css';

const DocumentPage = () => {
    const [documents, setDocuments] = useState([]);
    const [selectedDocuments, setSelectedDocuments] = useState([]);
    const [shareLink, setShareLink] = useState('');
    const [error, setError] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [documentToShare, setDocumentToShare] = useState(null);

    const fileTypeMap = {
        0: 'pdf',
        1: 'doc',
        2: 'docx',
        3: 'xls',
        4: 'xlsx',
        5: 'txt',
        6: 'jpg',
        7: 'png',
        8: 'default_icon'
    };

    useEffect(() => {
        const fetchDocuments = async () => {
            try {
                const documentsData = await getDocuments();
                setDocuments(documentsData);
            } catch (err) {
                setError('Error fetching documents');
            }
        };

        fetchDocuments();
    }, []);

    const handleSelectDocument = (id) => {
        setSelectedDocuments(prev =>
            prev.includes(id) ? prev.filter(docId => docId !== id) : [...prev, id]
        );
    };

    const handleDownload = async (id, name, fileType) => {
        try {
            const response = await downloadDocument(id);
            const url = window.URL.createObjectURL(new Blob([response]));
            const link = document.createElement('a');
            link.href = url;
            const extension = fileTypeMap[fileType] ? fileTypeMap[fileType].toLowerCase() : 'unknown';
            link.setAttribute('download', `${name}.${extension}`);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Download error:', error);
            setError('Error downloading document');
        }
    };

    const handleDownloadMultiple = async () => {
        try {
            const response = await downloadMultipleDocuments(selectedDocuments);
            const url = window.URL.createObjectURL(new Blob([response]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'documents.zip');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Download error:', error);
            setError(`Error downloading documents: ${error.message}`);
        }
    };

    const openShareModal = (document) => {
        setDocumentToShare(document);
        setIsModalOpen(true);
    };

    const closeShareModal = () => {
        setIsModalOpen(false);
        setDocumentToShare(null);
    };

    return (
        <div className="document-page">
            <h2>Documents</h2>
            {error && <div className="alert error">{error}</div>}
            <div className="documents-list">
                {documents.map((document) => (
                    <div key={document.id} className="document-item">
                        <input
                            type="checkbox"
                            checked={selectedDocuments.includes(document.id)}
                            onChange={() => handleSelectDocument(document.id)}
                        />
                        <img src={document.icon} alt={`${document.fileType} icon`} />
                        <div className="document-info">
                            <h3>{document.name}</h3>
                            <p>Uploaded on: {new Date(document.uploadDate).toLocaleDateString()}</p>
                            <p>Download Count: {document.downloadCount}</p>
                            <div className="document-item-buttons">
                                <button onClick={() => handleDownload(document.id, document.name, document.fileType)}>Download</button>
                                <button onClick={() => openShareModal(document)}>Share</button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            {selectedDocuments.length > 0 && (
                <button onClick={handleDownloadMultiple}>Download Selected</button>
            )}
            {isModalOpen && (
                <ShareModal
                    document={documentToShare}
                    onClose={closeShareModal}
                />
            )}
            {shareLink && (
                <div className="share-link">
                    <p>Share this link: <a href={shareLink}>{shareLink}</a></p>
                </div>
            )}
        </div>
    );
};

export default DocumentPage;
