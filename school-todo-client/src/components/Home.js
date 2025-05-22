import React from 'react';
import { Link } from 'react-router-dom';

function Home() {
  return (
    <div className="jumbotron">
      <h1 className="display-4">School Todo Management System</h1>
      <p className="lead">
        A simple application to manage schools, students, and related todo items.
      </p>
      <hr className="my-4" />
      <div className="row">
        <div className="col-md-4 mb-3">
          <div className="card">
            <div className="card-body">
              <h5 className="card-title">Students</h5>
              <p className="card-text">Manage student records</p>
              <Link to="/students" className="btn btn-primary">View Students</Link>
            </div>
          </div>
        </div>
        <div className="col-md-4 mb-3">
          <div className="card">
            <div className="card-body">
              <h5 className="card-title">Schools</h5>
              <p className="card-text">Manage school records</p>
              <Link to="/schools" className="btn btn-primary">View Schools</Link>
            </div>
          </div>
        </div>
        <div className="col-md-4 mb-3">
          <div className="card">
            <div className="card-body">
              <h5 className="card-title">Todo Items</h5>
              <p className="card-text">Manage todo items</p>
              <Link to="/todos" className="btn btn-primary">View Todos</Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Home;