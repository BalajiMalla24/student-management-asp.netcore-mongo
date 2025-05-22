import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const TodoList = () => {
  const [todos, setTodos] = useState([]);
  const { role, token } = useAuth();
  console.log(todos);

  useEffect(() => {
    const fetchTodos = async () => {
      try {
        let url = '/todoitems';
        if (role === 'Student') {
          url += '?mine=true'; // Adjust based on your backend logic
        }
        const { data } = await api.get(url, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setTodos(data);
      } catch (err) {
        console.error('Error fetching todos:', err);
      }
    };

    if (role) {
      fetchTodos();
    }
  }, [role, token]);

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure?')) {
      try {
        await api.delete(`/todoitems/${id}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setTodos(todos.filter((t) => t.id !== id));
      } catch (err) {
        console.error('Error deleting todo:', err);
        alert('Failed to delete todo.');
      }
    }
  };

  return (
    <div className="container mt-5">
      <h2>Todo Items</h2>

      {(role === 'Admin' || role === 'Student') && (
        <Link to="/todos/add" className="btn btn-primary mb-3">
          Add Todo
        </Link>
      )}

      {todos.length === 0 ? (
        <p>No todo items found.</p>
      ) : (
        <table className="table table-striped">
          <thead className="table-dark">
            <tr>
              <th>Title</th>
              <th>Description</th>
              <th>Related To</th>
              <th>Due Date</th>
              <th>Status</th>
              {(role === 'Admin' || role === 'Student') && <th>Actions</th>}
            </tr>
          </thead>
          <tbody>
            {todos.map((todo) => (
              <tr key={todo.id}>
                <td>{todo.title}</td>
                <td>{todo.description}</td>
                <td>{todo.relatedEntityType  || 'N/A'}</td>
                <td>{new Date(todo.dueDate).toLocaleDateString()}</td>
                <td>{todo.isCompleted ? 'Completed' : 'Pending'}</td>
                {(role === 'Admin' || role === 'Student')&& (
                  <td>
                    <Link to={`/todos/edit/${todo.id}`} className="btn btn-sm btn-warning me-2">
                      Edit
                    </Link>
                    <button
                      onClick={() => handleDelete(todo.id)}
                      className="btn btn-sm btn-danger"
                    >
                      Delete
                    </button>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default TodoList;
