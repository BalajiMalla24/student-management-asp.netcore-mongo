import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function SchoolList() {
  const [schools, setSchools] = useState([]);
  const [loading, setLoading] = useState(true);
  const { role } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    fetchSchools();
  }, []);

  const fetchSchools = async () => {
    try {
      const { data } = await api.get('/schools');
      setSchools(data);
    } catch (err) {
      console.error('Error fetching schools:', err);
    } finally {
      setLoading(false);
    }
  };

  const deleteSchool = async (id) => {
    if (!window.confirm('Delete this school?')) return;

    try {
      await api.delete(`/schools/${id}`);
      setSchools(prev => prev.filter(s => s.id !== id));
    } catch (err) {
      console.error('Error deleting school:', err);
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2>Schools</h2>
        {role === 'Admin' && (
          <Link to="/schools/add" className="btn btn-primary">Add School</Link>
        )}
      </div>

      {schools.length === 0 ? (
        <p>No schools found.</p>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped">
            <thead>
              <tr>
                <th>Name</th>
                <th>Address</th>
                <th>Phone</th>
                {role === 'Admin' && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {schools.map((s) => (
                <tr key={s.id}>
                  <td>{s.name}</td>
                  <td>{s.address}</td>
                  <td>{s.phoneNumber}</td>
                  {role === 'Admin' && (
                    <td>
                      <Link to={`/schools/edit/${s.id}`} className="btn btn-sm btn-info me-2">Edit</Link>
                      <button onClick={() => deleteSchool(s.id)} className="btn btn-sm btn-danger">Delete</button>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default SchoolList;
