import React, { useState } from 'react';
import { uploadDocument } from '../services/api';
import '../styles/style.css';

const UploadPage = () => {
    const [file, setFile] = useState(null);
    const [name, setName] = useState('');
    const [fileType, setFileType] = useState('');
    const [alertMessage, setAlertMessage] = useState(null);
    const [alertType, setAlertType] = useState(null);

    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    const handleNameChange = (e) => {
        setName(e.target.value);
    };

    const handleFileTypeChange = (e) => {
        setFileType(e.target.value);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!file || !name || !fileType) {
            setAlertMessage('Please fill all fields');
            setAlertType('error');
            return;
        }

        try {
            const document = { file, name, fileType };
            await uploadDocument(document);
            setAlertMessage('File uploaded successfully');
            setAlertType('success');
        } catch (error) {
            console.error('Upload error:', error.response || error.message || error);
            setAlertMessage('Error uploading file');
            setAlertType('error');
        }
    };

    return (
        <div className="upload-container">
            <h2>Upload Document</h2>
            {alertMessage && (
                <div className={`alert ${alertType}`}>
                    {alertMessage}
                </div>
            )}
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="name">Document Name:</label>
                    <input
                        type="text"
                        id="name"
                        value={name}
                        onChange={handleNameChange}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="fileType">File Type:</label>
                    <select
                        id="fileType"
                        value={fileType}
                        onChange={handleFileTypeChange}
                    >
                        <option value="">Select a file type</option>
                        <option value="pdf">PDF</option>
                        <option value="doc">DOC</option>
                        <option value="docx">DOCX</option>
                        <option value="xls">XLS</option>
                        <option value="xlsx">XLSX</option>
                        <option value="txt">TXT</option>
                        <option value="jpg">JPG</option>
                        <option value="png">PNG</option>
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="file">Select File:</label>
                    <input
                        type="file"
                        id="file"
                        onChange={handleFileChange}
                    />
                </div>
                <button type="submit">Upload</button>
            </form>
        </div>
    );
};

export default UploadPage;
