import React from 'react';
import { Link } from 'react-router-dom';

/**
 * @file Dashboard.js
 * @brief Displays user-specific dashboard actions and profile information.
 */

/**
 * @component
 * @name Dashboard
 * @description
 * Renders a dashboard for the logged-in user. Shows profile information
 * and role-based navigation links for teachers and students.
 *
 * @param {Object} props
 * @param {Object} props.user - The currently authenticated user.
 * @param {string} props.user.name - User's full name.
 * @param {string} props.user.email - User's email address.
 * @param {string} props.user.role - Role of the user (e.g., "teacher" or "student").
 * @param {string|Date} props.user.lastLogin - Timestamp of last login.
 *
 * @returns {JSX.Element} The user dashboard.
 */
function Dashboard({ user }) {
    return (
        <div className="dashboard">
            <h2>Welcome, {user?.name || 'User'}</h2>

            <div className="profile-info">
                <p><strong>Email:</strong> {user?.email}</p>
                <p><strong>Role:</strong> {user?.role}</p>
                <p><strong>Last Login:</strong> {new Date(user?.lastLogin).toLocaleString()}</p>
            </div>

            <div className="dashboard-actions">
                {user?.role === 'teacher' && (
                    <div className="teacher-actions">
                        <h3>Teacher Actions</h3>
                        <Link to="/courses" className="action-button">Manage Courses</Link>
                        <Link to="/grades" className="action-button">Manage Grades</Link>
                    </div>
                )}

                {user?.role === 'student' && (
                    <div className="student-actions">
                        <h3>Student Actions</h3>
                        <Link to="/my-grades" className="action-button">View My Grades</Link>
                    </div>
                )}
            </div>
        </div>
    );
}

export default Dashboard;
