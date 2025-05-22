import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link, Navigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

import { useAuth } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';

// Import components
import StudentList from './components/StudentList';
import StudentForm from './components/StudentForm';
import SchoolList from './components/SchoolList';
import SchoolForm from './components/SchoolForm';
import TodoList from './components/TodoList';
import TodoForm from './components/TodoForm';
import Home from './components/Home';
import Login from './components/Login';
import Signup from './components/Signup';

export default function App() {
  const { token, role, logout } = useAuth();

  const handleLogout = () => {
    logout();
    window.location.href = '/';
  };

  return (
    <Router>
      {/* Full-width navbar */}
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4 w-100">
        <div className="container-fluid">
          <Link to="/" className="navbar-brand">School Todo App</Link>
          <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
            <span className="navbar-toggler-icon"></span>
          </button>
          <div className="collapse navbar-collapse" id="navbarNav">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <Link to="/" className="nav-link">Home</Link>
              </li>
              {(token && (role === 'Admin' || role === 'Student')) && (
                <>
                  <li className="nav-item">
                    <Link to="/students" className="nav-link">Students</Link>
                  </li>
                  <li className="nav-item">
                    <Link to="/schools" className="nav-link">Schools</Link>
                  </li>
                </>
              )}
              {token && (
                <li className="nav-item">
                  <Link to="/todos" className="nav-link">Todo Items</Link>
                </li>
              )}
            </ul>
            <ul className="navbar-nav">
              {!token ? (
                <>
                  <li className="nav-item">
                    <Link to="/login" className="nav-link">Login</Link>
                  </li>
                  <li className="nav-item">
                    <Link to="/signup" className="nav-link">Signup</Link>
                  </li>
                </>
              ) : (
                <li className="nav-item">
                  <button onClick={handleLogout} className="btn btn-outline-light">Logout</button>
                </li>
              )}
            </ul>
          </div>
        </div>
      </nav>

      {/* Page content container */}
      <div className="container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={!token ? <Login /> : <Navigate to="/" />} />
          <Route path="/signup" element={!token ? <Signup /> : <Navigate to="/" />} />

          <Route path="/students" element={
            <ProtectedRoute roleRequired={['Admin', 'Student']}>
              <StudentList />
            </ProtectedRoute>
          } />
          <Route path="/schools" element={
            <ProtectedRoute roleRequired={['Admin', 'Student']}>
              <SchoolList />
            </ProtectedRoute>
          } />
          <Route path="/students/add" element={
            <ProtectedRoute roleRequired="Admin">
              <StudentForm />
            </ProtectedRoute>
          } />
          <Route path="/students/edit/:id" element={
            <ProtectedRoute roleRequired="Admin">
              <StudentForm />
            </ProtectedRoute>
          } />
          <Route path="/schools/add" element={
            <ProtectedRoute roleRequired="Admin">
              <SchoolForm />
            </ProtectedRoute>
          } />
          <Route path="/schools/edit/:id" element={
            <ProtectedRoute roleRequired="Admin">
              <SchoolForm />
            </ProtectedRoute>
          } />
          <Route path="/todos" element={
            <ProtectedRoute>
              <TodoList />
            </ProtectedRoute>
          } />
          <Route path="/todos/add" element={
            <ProtectedRoute>
              <TodoForm />
            </ProtectedRoute>
          } />
          <Route path="/todos/edit/:id" element={
            <ProtectedRoute>
              <TodoForm />
            </ProtectedRoute>
          } />
        </Routes>
      </div>
    </Router>
  );
}
  