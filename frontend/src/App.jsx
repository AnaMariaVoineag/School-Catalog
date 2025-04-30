// App.js
import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import CourseManagement from './components/CourseManagement';
import GradeManagement from './components/GradeManagement';
import StudentGrades from './components/StudentGrades';
import { API_BASE_URL } from './config';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      fetchUserProfile(token);
    }
  }, []);

  const fetchUserProfile = async (token) => {
    try {
      const response = await fetch(API_BASE_URL + '/api/users/me', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        const userData = await response.json();
        setUser(userData);
        setUserRole(userData.role);
        setIsAuthenticated(true);
      } else {
        logout();
      }
    } catch (error) {
      console.error('Error fetching user profile:', error);
      logout();
    }
  };

  const login = (token) => {
    localStorage.setItem('token', token);
    fetchUserProfile(token);
  };

  const logout = () => {
    localStorage.removeItem('token');
    setIsAuthenticated(false);
    setUserRole(null);
    setUser(null);
  };

  return (
    <Router>
      <div className="app">
        <header>
          <h1>School Management System</h1>
          {isAuthenticated && (
            <nav>
              <Link to="/dashboard">Dashboard</Link>
              {userRole === 'teacher' && (
                <>
                  <Link to="/courses">Course Management</Link>
                  <Link to="/grades">Grade Management</Link>
                </>
              )}
              {userRole === 'student' && (
                <Link to="/my-grades">My Grades</Link>
              )}
              <button onClick={logout}>Logout</button>
            </nav>
          )}
        </header>

        <main>
          <Routes>
            <Route path="/login" element={!isAuthenticated ? <Login onLogin={login} /> : <Navigate to="/dashboard" />} />
            <Route path="/register" element={!isAuthenticated ? <Register onLogin={login} /> : <Navigate to="/dashboard" />} />
            <Route path="/dashboard" element={isAuthenticated ? <Dashboard user={user} /> : <Navigate to="/login" />} />

            {/* Teacher Routes */}
            <Route path="/courses" element={
              isAuthenticated && userRole === 'teacher'
                ? <CourseManagement />
                : <Navigate to={isAuthenticated ? "/dashboard" : "/login"} />
            } />
            <Route path="/grades" element={
              isAuthenticated && userRole === 'teacher'
                ? <GradeManagement />
                : <Navigate to={isAuthenticated ? "/dashboard" : "/login"} />
            } />

            {/* Student Routes */}
            <Route path="/my-grades" element={
              isAuthenticated && userRole === 'student'
                ? <StudentGrades studentId={user?.ID} />
                : <Navigate to={isAuthenticated ? "/dashboard" : "/login"} />
            } />

            <Route path="/" element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;