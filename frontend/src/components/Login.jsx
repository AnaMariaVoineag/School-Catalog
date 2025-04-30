import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { API_BASE_URL } from '../config';

/**
 * @file Login.js
 * @brief Component for user login functionality.
 */

/**
 * @component
 * @name Login
 * @description
 * Renders a login form allowing users to input their email and password.
 * On successful authentication, it invokes a callback to store the token.
 *
 * @param {Object} props
 * @param {Function} props.onLogin - Callback invoked with the auth token on successful login.
 *
 * @returns {JSX.Element} The login form component.
 */
function Login({ onLogin }) {
    /// State to store user-entered email
    const [email, setEmail] = useState('');

    /// State to store user-entered password
    const [password, setPassword] = useState('');

    /// State to display login errors
    const [error, setError] = useState('');

    /**
     * @function handleSubmit
     * @async
     * @description
     * Handles form submission by sending a login request to the API.
     * Sets the error message on failure, or calls `onLogin` on success.
     *
     * @param {Event} e - The form submit event.
     */
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const response = await fetch(API_BASE_URL + '/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            if (response.ok) {
                const data = await response.json();
                onLogin(data.token);
            } else {
                setError('Invalid email or password');
            }
        } catch (error) {
            setError('Login failed. Please try again.');
        }
    };

    // Render the login form
    return (
        <div className="auth-form">
            <h2>Login</h2>
            {error && <p className="error">{error}</p>}
            <form onSubmit={handleSubmit}>
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
                <button type="submit">Login</button>
            </form>
            <p>
                Don't have an account? <Link to="/register">Register</Link>
            </p>
        </div>
    );
}

export default Login;
