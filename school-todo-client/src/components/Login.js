import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const Login = () => {
  const [form, setForm] = useState({ username: '', password: '' });
  const [error, setError] = useState('');
  const [validationErrors, setValidationErrors] = useState({});
  const navigate = useNavigate();
  const { login } = useAuth();

  const validate = () => {
    const errors = {};
    if (!form.username.trim()) errors.username = 'Username is required';
    if (!form.password) errors.password = 'Password is required';
    return errors;
  };

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
    setValidationErrors({ ...validationErrors, [e.target.name]: null });
    setError('');
  };

  const handleSubmit = async (e) => {
  e.preventDefault();
  const errors = validate();
  if (Object.keys(errors).length > 0) {
    setValidationErrors(errors);
    return;
  }

  try {
    const res = await api.post('/auth/login', form);
    login({
      token: res.data.token,
      role: res.data.role,
      id: res.data.id,
    });
    navigate('/');
  } catch (err) {
    setError('Invalid username or password');
  }
};

  return (
    <div
      className="d-flex align-items-center justify-content-center vh-100 bg-light"
      style={{ padding: '1rem' }}
    >
      <div className="card shadow-sm p-4 rounded" style={{ maxWidth: '400px', width: '100%' }}>
        <h2 className="mb-4 text-center fw-bold text-primary">Login</h2>

        {error && <div className="alert alert-danger">{error}</div>}

        <form noValidate onSubmit={handleSubmit}>
          <div className="mb-3">
            <input
              name="username"
              value={form.username}
              onChange={handleChange}
              className={`form-control ${validationErrors.username ? 'is-invalid' : ''}`}
              placeholder="Username"
              required
              autoFocus
            />
            {validationErrors.username && (
              <div className="invalid-feedback">{validationErrors.username}</div>
            )}
          </div>

          <div className="mb-4">
            <input
              name="password"
              type="password"
              value={form.password}
              onChange={handleChange}
              className={`form-control ${validationErrors.password ? 'is-invalid' : ''}`}
              placeholder="Password"
              required
            />
            {validationErrors.password && (
              <div className="invalid-feedback">{validationErrors.password}</div>
            )}
          </div>

          <button type="submit" className="btn btn-primary w-100 fw-semibold">
            Login
          </button>
        </form>
      </div>
    </div>
  );
};

export default Login;
