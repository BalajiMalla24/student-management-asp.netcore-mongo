import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function SchoolForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);
  const { role } = useAuth();  // <-- use role directly

  const [formData, setFormData] = useState({
    name: '',
    address: '',
    phoneNumber: ''
  });
  const [loading, setLoading] = useState(isEditMode);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (role !== 'Admin') {
      navigate('/schools');
      return;
    }
    if (isEditMode) fetchSchool();
  }, [id, role, isEditMode, navigate]);

  const fetchSchool = async () => {
    try {
      const { data } = await api.get(`/schools/${id}`);
      setFormData({
        name: data.name,
        address: data.address,
        phoneNumber: data.phoneNumber
      });
    } catch (err) {
      console.error('Error fetching school:', err);
      setError('Failed to load school data.');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = e => {
    const { name, value } = e.target;
    setFormData(f => ({ ...f, [name]: value }));
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setError(null);
    try {
      if (isEditMode) {
        await api.put(`/schools/${id}`, formData);
      } else {
        await api.post('/schools', formData);
      }
      navigate('/schools');
    } catch (err) {
      console.error('Error saving school:', err);
      setError('Failed to save school. Please check the input and try again.');
    }
  };

  if (loading) return <div className="text-center my-5">Loading...</div>;

  return (
    <div className="container mt-4" style={{ maxWidth: 600 }}>
      <h2 className="mb-4 text-center">{isEditMode ? 'Edit School' : 'Add School'}</h2>

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} noValidate>
        {['name', 'address', 'phoneNumber'].map(field => (
          <div className="mb-3" key={field}>
            <label htmlFor={field} className="form-label fw-semibold">
              {field === 'phoneNumber' ? 'Phone Number' : field.charAt(0).toUpperCase() + field.slice(1)}
            </label>
            <input
              type="text"
              className="form-control"
              id={field}
              name={field}
              value={formData[field]}
              onChange={handleChange}
              placeholder={`Enter ${field === 'phoneNumber' ? 'phone number' : field}`}
              required
            />
          </div>
        ))}

        <div className="d-flex justify-content-center gap-3">
          <button type="submit" className="btn btn-primary px-4">
            Save
          </button>
          <button
            type="button"
            className="btn btn-outline-secondary px-4"
            onClick={() => navigate('/schools')}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

export default SchoolForm;
