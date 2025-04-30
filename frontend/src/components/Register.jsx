import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';

/**
 * @file Register.js
 * @brief Component that provides user registration functionality.
 */

/**
 * @component
 * @name Register
 * @description
 * Renders a registration form where users can enter their name, email, password,
 * and role (student or teacher). On successful registration, it triggers a login callback.
 *
 * @param {Object} props
 * @param {Function} props.onLogin - Callback function invoked with token upon successful registration.
 *
 * @returns {JSX.Element} The registration form component.
 */
function Register({ onLogin }) {
    /// State to store user-entered name
    const [name, setName] = useState('');

    /// State to store user-entered email
    const [email, setEmail] = useState('');

    /// State to store user-entered password
    const [password, setPassword] = useState('');

    /// State to store selected user role (student or teacher)
    const [role, setRole] = useState('student');

    /// State to hold any registration-related error message
    const [error, setError] = useState('');

    /**
     * @function handleSubmit
     * @async
     * @description
     * Handles form submission by sending a registration request to the API.
     * Updates the error state on failure or calls `onLogin` on success.
     *
     * @param {Event} e - Form submit event
     */
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const response = await fetch(API_BASE_URL + '/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    name,
                    email,
                    password,
                    role
                }),
            });

            if (response.ok) {
                const data = await response.json();
                onLogin(data.token);
            } else if (response.status === 409) {
                setError('User already exists with this email');
            } else {
                const errorData = await response.json();
                setError(errorData || 'Registration failed');
            }
        } catch (error) {
            setError('Registration failed. Please try again.');
        }
    };

    // Render the registration form
    return (
        <div className="auth-form">
            <h2>Register</h2>
            {error && <p className="error">{error}</p>}
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Name:</label>
                    <input
                        type="text"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Email:</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Role:</label>
                    <select value={role} onChange={(e) => setRole(e.target.value)}>
                        <option value="student">Student</option>
                        <option value="teacher">Teacher</option>
                    </select>
                </div>
                <button type="submit">Register</button>
            </form>
            <p>
                Already have an account? <Link to="/login">Login</Link>
            </p>
        </div>
    );
}

export default Register;
