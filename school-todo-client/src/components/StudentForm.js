import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function StudentForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = Boolean(id);
  const { role } = useAuth();

  const [formData, setFormData] = useState({
    name: '',
    age: '',
    grade: '',
    schoolId: ''
  });
  const [loading, setLoading] = useState(isEditMode);
  const [schools, setSchools] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (role !== 'Admin') {
      navigate('/students');
      return;
    }
    fetchSchools();
    if (isEditMode) fetchStudent();
  }, [id, isEditMode, role, navigate]);

  const fetchSchools = async () => {
    try {
      const { data } = await api.get('/schools');
      setSchools(data);
    } catch (err) {
      console.error('Error fetching schools:', err);
      setError('Failed to load schools.');
    }
  };

  const fetchStudent = async () => {
    try {
      const { data } = await api.get(`/students/${id}`);
      setFormData({
        name: data.name,
        age: data.age,
        grade: data.grade,
        schoolId: data.schoolId || ''
      });
    } catch (err) {
      console.error('Error fetching student:', err);
      setError('Failed to load student data.');
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
        await api.put(`/students/${id}`, formData);
      } else {
        await api.post('/students', formData);
      }
      navigate('/students');
    } catch (err) {
      console.error('Error saving student:', err);
      setError('Failed to save student. Please check the input and try again.');
    }
  };

  if (loading) return <div className="text-center my-5">Loading...</div>;

  return (
    <div className="container mt-4" style={{ maxWidth: 600 }}>
      <h2 className="mb-4 text-center">{isEditMode ? 'Edit Student' : 'Add Student'}</h2>

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} noValidate>
        {['name', 'age', 'grade'].map(field => (
          <div className="mb-3" key={field}>
            <label htmlFor={field} className="form-label fw-semibold">
              {field.charAt(0).toUpperCase() + field.slice(1)}
            </label>
            <input
              type={field === 'age' ? 'number' : 'text'}
              className="form-control"
              id={field}
              name={field}
              value={formData[field]}
              onChange={handleChange}
              placeholder={`Enter ${field}`}
              required
              min={field === 'age' ? 1 : undefined}
            />
          </div>
        ))}

        <div className="mb-4">
          <label htmlFor="schoolId" className="form-label fw-semibold">
            School
          </label>
          <select
            className="form-select"
            id="schoolId"
            name="schoolId"
            value={formData.schoolId}
            onChange={handleChange}
            required
          >
            <option value="" disabled>
              Select a School
            </option>
            {schools.map(s => (
              <option key={s.id} value={s.id}>
                {s.name}
              </option>
            ))}
          </select>
        </div>

        <div className="d-flex justify-content-center gap-3">
          <button type="submit" className="btn btn-primary px-4">
            Save
          </button>
          <button
            type="button"
            className="btn btn-outline-secondary px-4"
            onClick={() => navigate('/students')}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

export default StudentForm;
