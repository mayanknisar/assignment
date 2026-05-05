const BASE_URL = 'http://localhost:3000/api/work-requests';

// Helper function to build query string
const buildQueryString = (params) => {
    const query = new URLSearchParams();
    Object.keys(params).forEach(key => {
        if (params[key] !== undefined && params[key] !== null && params[key] !== '') {
            query.append(key, params[key]);
        }
    });
    return query.toString();
};

// GET /api/work-requests?status=&search=&page=&pageSize=
export const getWorkRequests = async (params = {}) => {
    const queryString = buildQueryString(params);
    const url = `${BASE_URL}${queryString ? `?${queryString}` : ''}`;
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Failed to fetch work requests: ${response.statusText}`);
    }
    return response.json();
};

// GET /api/work-requests/{id}
export const getWorkRequestById = async (id) => {
    const response = await fetch(`${BASE_URL}/${id}`);
    if (!response.ok) {
        throw new Error(`Failed to fetch work request: ${response.statusText}`);
    }
    return response.json();
};

// POST /api/work-requests
export const createWorkRequest = async (data) => {
    const response = await fetch(BASE_URL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    if (!response.ok) {
        throw new Error(`Failed to create work request: ${response.statusText}`);
    }
    return response.json();
};

// PATCH /api/work-requests/{id}/status
export const updateWorkRequestStatus = async (id, status) => {
    const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ status }),
    });
    if (!response.ok) {
        throw new Error(`Failed to update work request status: ${response.statusText}`);
    }
    return response.json();
};

// POST /api/work-requests/{id}/notes
export const addNoteToWorkRequest = async (id, notes) => {
    const response = await fetch(`${BASE_URL}/${id}`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ notes }),
    });
    if (!response.ok) {
        throw new Error(`Failed to add note to work request: ${response.statusText}`);
    }
    return response.json();
};