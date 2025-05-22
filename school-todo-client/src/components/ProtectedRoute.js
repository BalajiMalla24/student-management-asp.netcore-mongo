import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const ProtectedRoute = ({ children, roleRequired }) => {
  const { token, role } = useAuth();

  if (!token) return <Navigate to="/login" />;

  if (!roleRequired) return children;

  const rolesAllowed = Array.isArray(roleRequired) ? roleRequired : [roleRequired];

  if (!rolesAllowed.includes(role)) {
    return <Navigate to="/" />;
  }

  return children;
};

export default ProtectedRoute;
