import { useState, useEffect, useCallback, useMemo } from 'react';
import { getWorkRequests, createWorkRequest, updateWorkRequestStatus, addNoteToWorkRequest } from '../services/workRequestsService';

function WorkRequestList({ fetchWorkRequests, workRequests, loading }) {
    // const [workRequests, setWorkRequests] = useState([]);
    const [filters, setFilters] = useState({ status: '', search: '', page: 1, pageSize: 10 });

    const filteredRequests = useMemo(() => {
        return workRequests.filter((wr) => {
            const matchesStatus = filters.status ? wr.status == filters.status : true;
            const matchesSearch = filters.search
                ? wr.title.toLowerCase().includes(filters.search.toLowerCase()) ||
                wr.clientName.toLowerCase().includes(filters.search.toLowerCase())
                : true;
            return matchesStatus && matchesSearch;
        });
    }, [filters, workRequests]);
    //const [loading, setLoading] = useState(false);

    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        fetchWorkRequests();
    }, []);


    const handleStatusUpdate = async (id, status) => {
        try {
            await updateWorkRequestStatus(id, status);
            fetchWorkRequests();
        } catch (error) {
            console.error('Failed to update status:', error);
        }
    };

    const handleAddNote = async (id, note) => {
        if (!note.trim()) return;
        try {
            await addNoteToWorkRequest(id, note);
            fetchWorkRequests();
        } catch (error) {
            console.error('Failed to add note:', error);
        }
    };

    return (
        <div className="wr-list">
            {/* Filters and Search */}
            <div className="filters">
                <input
                    type="text"
                    placeholder="Search by title or client name"
                    value={filters.search}
                    onChange={(e) => setFilters({ ...filters, search: e.target.value })}
                />
                <select
                    value={filters.status}
                    onChange={(e) => setFilters({ ...filters, status: e.target.value })}
                >
                    <option value="">All Status</option>
                    <option value="0">Open</option>
                    <option value="1">In Progress</option>
                    <option value="2">Closed</option>
                </select>
            </div>


            {/* Work Requests List */}
            <div className="work-requests">
                <h2>Work Requests</h2>
                {loading ? (
                    <p>Loading...</p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Client Name</th>
                                <th>Status</th>
                                <th>Notes</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredRequests.map((wr) => (
                                <tr key={wr.id}>
                                    <td>{wr.title}</td>
                                    <td>{wr.clientName}</td>
                                    <td>
                                        <select
                                            value={wr.status}
                                            onChange={(e) => handleStatusUpdate(wr.id, e.target.value)}
                                        >
                                            <option value="0">Open</option>
                                            <option value="1">In Progress</option>
                                            <option value="2">Closed</option>
                                        </select>
                                    </td>
                                    <td>

                                        <p>{wr.notes}</p>
                                    </td>
                                    <td>

                                        <button onClick={() => handleAddNote(wr.id, prompt('Enter note:'))}>Add Note</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default WorkRequestList;
