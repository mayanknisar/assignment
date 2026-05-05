import { useState, useEffect, useCallback } from 'react';
import WorkRequestList from './Components/WorkRequestList';
import NewWorkRequest from './Components/NewWorkRequest';
import { getWorkRequests, createWorkRequest, updateWorkRequestStatus, addNoteToWorkRequest } from './services/workRequestsService';
import './App.css';

function App() {
  const [createNew, setCreateNew] = useState(false);

  return (
    <div className="app">
      <h1>Work Request Tracker</h1>
      <button onClick={() => setCreateNew(!createNew)}> {createNew ? 'Close' : 'Create New Work Request'}</button>
      {createNew ? <NewWorkRequest /> : <WorkRequestList />}

    </div>
  );
}

export default App;
