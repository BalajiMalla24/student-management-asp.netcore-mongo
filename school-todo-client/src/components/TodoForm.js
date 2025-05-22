import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

function TodoForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { role, token, userId } = useAuth();
  const isEditMode = Boolean(id);
  console.log(role)
  console.log(token)
  console.log(userId)
  useEffect(() => {
    if (role !== 'Admin' && role !== 'Student') {
      navigate('/todos');
    }
  }, [role, navigate]);

  const [formData, setFormData] = useState({
    Title: '',
    Description: '',
    IsCompleted: false,
    DueDate: '',
    RelatedEntityId: '',
    RelatedEntityType: 'Student',
    createdById: userId,
  });
  // console.log(formData.createdById)
  const [documentFile, setDocumentFile] = useState(null);
  const [loading, setLoading] = useState(isEditMode);
  const [students, setStudents] = useState([]);
  const [schools, setSchools] = useState([]);

  useEffect(() => {
    if (role === 'Student') {
      setFormData(f => ({
        ...f,
        RelatedEntityType: 'Student',
        createdById: userId,
      }));
    }
  }, [role, userId]);

  useEffect(() => {
    (async () => {
      try {
        const [sRes, schRes] = await Promise.all([
          api.get('/students', { headers: { Authorization: `Bearer ${token}` } }),
          api.get('/schools', { headers: { Authorization: `Bearer ${token}` } }),
        ]);
        setStudents(sRes.data);
        setSchools(schRes.data);

        if (isEditMode) {
          const { data } = await api.get(`/todoitems/${id}`, { headers: { Authorization: `Bearer ${token}` } });
          setFormData({
            Title: data.title,
            Description: data.description,
            IsCompleted: data.isCompleted,
            DueDate: new Date(data.dueDate).toISOString().split('T')[0],
            RelatedEntityId: data.relatedEntityId || '',
            RelatedEntityType: data.relatedEntityType || 'Student',
          });
        }
      } catch (err) {
        console.error('Error fetching data:', err);
      } finally {
        setLoading(false);
      }
    })();
  }, [id, isEditMode, token]);

  const handleChange = e => {
    const { name, value, type, checked } = e.target;
    setFormData(f => ({
      ...f,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleFileChange = e => {
    setDocumentFile(e.target.files.length > 0 ? e.target.files[0] : null);
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      const payload = new FormData();
      Object.entries(formData).forEach(([key, value]) => {
        payload.append(key, value);
      });
      if (documentFile) {
        payload.append('document', documentFile);
      }
      if (isEditMode) {
        await api.put(`/todoitems/${id}`, payload, {
          headers: { 'Content-Type': 'multipart/form-data', Authorization: `Bearer ${token}` },
        });
      } else {
        await api.post('/todoitems', payload, {
          headers: { 'Content-Type': 'multipart/form-data', Authorization: `Bearer ${token}` },
        });
      }
      navigate('/todos');
    } catch (err) {
      console.error('Error saving todo:', err);
      alert('Failed to save todo item.');
    }
  };

  if (loading)
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status" aria-label="Loading spinner">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-light p-3">
      <div className="card shadow-sm p-4 rounded" style={{ maxWidth: 600, width: '100%' }}>
        <h2 className="mb-4 text-center fw-bold text-primary">
          {isEditMode ? 'Edit Todo Item' : 'Add Todo Item'}
        </h2>
        <form onSubmit={handleSubmit} encType="multipart/form-data" noValidate>
          <div className="mb-3">
            <label htmlFor="Title" className="form-label fw-semibold">
              Title <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              id="Title"
              name="Title"
              className="form-control"
              value={formData.Title}
              onChange={handleChange}
              placeholder="Enter title"
              required
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label htmlFor="Description" className="form-label fw-semibold">
              Description
            </label>
            <textarea
              id="Description"
              name="Description"
              className="form-control"
              rows="4"
              value={formData.Description}
              onChange={handleChange}
              placeholder="Enter description (optional)"
              disabled={loading}
            />
          </div>

          <div className="form-check mb-3">
            <input
              type="checkbox"
              id="IsCompleted"
              name="IsCompleted"
              className="form-check-input"
              checked={formData.IsCompleted}
              onChange={handleChange}
              disabled={loading}
            />
            <label htmlFor="IsCompleted" className="form-check-label fw-semibold">
              Mark as Completed
            </label>
          </div>

          <div className="mb-3">
            <label htmlFor="DueDate" className="form-label fw-semibold">
              Due Date <span className="text-danger">*</span>
            </label>
            <input
              type="date"
              id="DueDate"
              name="DueDate"
              className="form-control"
              value={formData.DueDate}
              onChange={handleChange}
              required
              disabled={loading}
            />
          </div>

          <div className="mb-3">
            <label htmlFor="RelatedEntityType" className="form-label fw-semibold">
              Related To
            </label>
            <select
              id="RelatedEntityType"
              name="RelatedEntityType"
              className="form-select"
              value={formData.RelatedEntityType}
              onChange={handleChange}
              disabled={loading || role === 'Student'}
            >
              <option value="Student">Student</option>
              <option value="School">School</option>
            </select>
          </div>

          <div className="mb-3">
            <label htmlFor="RelatedEntityId" className="form-label fw-semibold">
              {formData.RelatedEntityType} <span className="text-danger">*</span>
            </label>
            <select
              id="RelatedEntityId"
              name="RelatedEntityId"
              className="form-select"
              value={formData.RelatedEntityId}
              onChange={handleChange}
              required
              disabled={loading}
            >
              <option value="" disabled>
                Select {formData.RelatedEntityType}
              </option>
              {formData.RelatedEntityType === 'Student'
                ? students.map(s => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))
                : schools.map(s => (
                    <option key={s.id} value={s.id}>
                      {s.name}
                    </option>
                  ))}
            </select>
          </div>

          <div className="mb-4">
            <label htmlFor="document" className="form-label fw-semibold">
              Upload Document (optional)
            </label>
            <input
              type="file"
              id="document"
              name="document"
              className="form-control"
              accept=".pdf,.doc,.docx,.png,.jpg,.jpeg"
              onChange={handleFileChange}
              disabled={loading || isEditMode}
            />
            {documentFile && (
              <div className="form-text text-truncate" title={documentFile.name}>
                Selected file: {documentFile.name}
              </div>
            )}
            {isEditMode && (
              <small className="text-muted fst-italic">
                Document upload disabled in edit mode.
              </small>
            )}
          </div>

          <div className="d-flex justify-content-between">
            <button type="submit" className="btn btn-primary px-4" disabled={loading}>
              {loading ? (
                <>
                  <span
                    className="spinner-border spinner-border-sm me-2"
                    role="status"
                    aria-hidden="true"
                  ></span>
                  Saving...
                </>
              ) : (
                'Save'
              )}
            </button>
            <button
              type="button"
              className="btn btn-outline-secondary px-4"
              onClick={() => navigate('/todos')}
              disabled={loading}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default TodoForm;
