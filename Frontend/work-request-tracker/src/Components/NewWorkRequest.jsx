import { useState, useEffect, useCallback } from 'react';
import { getWorkRequests, createWorkRequest, updateWorkRequestStatus, addNoteToWorkRequest } from '../services/workRequestsService';


function NewWorkRequest() {
  const [newRequest, setNewRequest] = useState({
    title: '',
    clientName: '',
    description: '',
    status: 'new',
    priority: "low",
    dueDate: ''
  });
  const [loading, setLoading] = useState(false);



  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      await createWorkRequest(newRequest);
      setNewRequest({ title: '', clientName: '', description: '' });
      fetchWorkRequests();
    } catch (error) {
      console.error('Failed to create work request:', error);
    }
  };

  return (
    <>
      {/* Create New Work Request */}
      <div className="create-form">
        <h2>Create New Work Request</h2>
        <form onSubmit={handleCreate}>
          <input
            type="text"
            placeholder="Title"
            value={newRequest.title}
            onChange={(e) => setNewRequest({ ...newRequest, title: e.target.value })}
            required
          />
          <input
            type="text"
            placeholder="Client Name"
            value={newRequest.clientName}
            onChange={(e) => setNewRequest({ ...newRequest, clientName: e.target.value })}
            required
          />

          <select name="priority" id="priority" onChange={(e) => setNewRequest({ ...newRequest, priority: e.target.value })}>
            <option value="low">Low</option>
            <option value="medium">Medium</option>
            <option value="high">High</option>
          </select>
          <textarea
            placeholder="Description"
            value={newRequest.description}
            onChange={(e) => setNewRequest({ ...newRequest, description: e.target.value })}
          />
          <input
            type="date"
            value={newRequest.dueDate ? new Date(newRequest.dueDate).toISOString().split('T')[0] : ''}
            onChange={(e) => setNewRequest({ ...newRequest, dueDate: e.target.value })}
          />
          <button type="submit">Create Work Request</button>
        </form>
      </div >
    </>
  );
}

export default NewWorkRequest;
