import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const Signup = () => {
  const [form, setForm] = useState({ username: '', email: '', password: '', confirmPassword: '', role: 'Student' });
  const [error, setError] = useState('');
  const [validationErrors, setValidationErrors] = useState({});
  const navigate = useNavigate();

  const validate = () => {
    const errors = {};

    if (!form.username.trim()) errors.username = 'Username is required';
    if (!form.email.trim()) errors.email = 'Email is required';
    else if (!/\S+@\S+\.\S+/.test(form.email)) errors.email = 'Email is invalid';

    if (!form.password) errors.password = 'Password is required';
    else if (form.password.length < 6) errors.password = 'Password must be at least 6 characters';

    if (!form.confirmPassword) errors.confirmPassword = 'Please confirm password';
    else if (form.password !== form.confirmPassword) errors.confirmPassword = 'Passwords do not match';

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
      const { confirmPassword, ...payload } = form;
      await api.post('/auth/signup', payload);
      navigate('/login');
    } catch (err) {
      setError('Signup failed');
    }
  };

  return (
    <div
      className="d-flex align-items-center justify-content-center vh-100 bg-light"
      style={{ padding: '1rem' }}
    >
      <div className="card shadow-sm p-4 rounded" style={{ maxWidth: '400px', width: '100%' }}>
        <h2 className="mb-4 text-center fw-bold text-primary">Create an Account</h2>

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
            {validationErrors.username && <div className="invalid-feedback">{validationErrors.username}</div>}
          </div>

          <div className="mb-3">
            <input
              name="email"
              type="email"
              value={form.email}
              onChange={handleChange}
              className={`form-control ${validationErrors.email ? 'is-invalid' : ''}`}
              placeholder="Email"
              required
            />
            {validationErrors.email && <div className="invalid-feedback">{validationErrors.email}</div>}
          </div>

          <div className="mb-3">
            <input
              name="password"
              type="password"
              value={form.password}
              onChange={handleChange}
              className={`form-control ${validationErrors.password ? 'is-invalid' : ''}`}
              placeholder="Password"
              required
            />
            {validationErrors.password && <div className="invalid-feedback">{validationErrors.password}</div>}
          </div>

          <div className="mb-3">
            <input
              name="confirmPassword"
              type="password"
              value={form.confirmPassword}
              onChange={handleChange}
              className={`form-control ${validationErrors.confirmPassword ? 'is-invalid' : ''}`}
              placeholder="Confirm Password"
              required
            />
            {validationErrors.confirmPassword && <div className="invalid-feedback">{validationErrors.confirmPassword}</div>}
          </div>

          <div className="mb-4">
            <select
              name="role"
              value={form.role}
              onChange={handleChange}
              className="form-select"
              aria-label="Select role"
            >
              <option value="Student">Student</option>
              <option value="Admin">Admin</option>
            </select>
          </div>

          <button type="submit" className="btn btn-primary w-100 fw-semibold">
            Sign Up
          </button>
        </form>
      </div>
    </div>
  );
};

export default Signup;
