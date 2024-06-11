import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL;

const api = axios.create({
    baseURL: API_URL,
});

export const uploadDocument = async (document) => {
    const formData = new FormData();
    formData.append('content', document.file);
    formData.append('name', document.name);
    formData.append('fileType', document.fileType);

    const response = await api.post('/documents/upload', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
    return response.data;
};

export const getDocuments = async () => {
    const response = await api.get('/documents', {
        params: { timestamp: new Date().getTime() },
    });
    return response.data;
};


export const downloadDocument = async (id) => {
    const response = await api.get(`/documents/download/${id}`, {
        responseType: 'blob',
    });
    return response.data;
};

export const generateShareLink = async ({ documentId, duration, unit }) => {
    const formData = new FormData();
    formData.append('DocumentId', documentId);
    formData.append('Duration', duration);
    formData.append('Unit', unit);

    const response = await api.post('/documents/share', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
    return response.data;
};

export const downloadMultipleDocuments = async (ids) => {
    const response = await api.post('/documents/download/multiple', { documentIds: ids }, { responseType: 'blob' });
    return response.data;
};

