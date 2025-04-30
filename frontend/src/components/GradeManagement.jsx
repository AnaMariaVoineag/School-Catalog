import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';

/**
 * @file GradeManagement.js
 * @brief Component for managing grades, including fetching, adding, and deleting.
 */

/**
 * @component
 * @name GradeManagement
 * @description
 * Admin/teacher interface for managing student grades within courses.
 * Allows selecting a course, adding new grades, and deleting existing ones.
 *
 * @returns {JSX.Element} The grade management interface.
 */
function GradeManagement() {
    /// List of all grades for the selected course
    const [grades, setGrades] = useState([]);

    /// List of all courses available to the teacher
    const [courses, setCourses] = useState([]);

    /// List of all students
    const [students, setStudents] = useState([]);

    /// Form input data for creating a new grade
    const [formData, setFormData] = useState({
        value: '',
        gradeType: '',
        feedback: '',
        studentId: '',
        courseId: ''
    });

    /// Currently selected course ID
    const [selectedCourse, setSelectedCourse] = useState('');

    /// Error message string
    const [error, setError] = useState('');

    /// Token for authorization
    const token = localStorage.getItem('token');

    // Fetch course and student data on initial render
    useEffect(() => {
        fetchCourses();
        fetchStudents();
    }, []);

    // Fetch grades when a course is selected
    useEffect(() => {
        if (selectedCourse) {
            fetchGrades(selectedCourse);
        }
    }, [selectedCourse]);

    /**
     * @function fetchCourses
     * @description Fetches available courses from the API.
     */
    const fetchCourses = async () => {
        try {
            const response = await fetch(API_BASE_URL + '/courses', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (response.ok) {
                const data = await response.json();
                setCourses(data);
            }
        } catch (error) {
            console.error('Error fetching courses:', error);
        }
    };

    /**
     * @function fetchStudents
     * @description Fetches student list from the API.
     * Note: Endpoint `/api/students` must be implemented in the backend.
     */
    const fetchStudents = async () => {
        try {
            const response = await fetch(API_BASE_URL + '/api/students', {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (response.ok) {
                const data = await response.json();
                setStudents(data);
            }
        } catch (error) {
            console.error('Error fetching students:', error);
        }
    };

    /**
     * @function fetchGrades
     * @description Fetches grades for a specific course.
     * @param {string} courseId - ID of the course to fetch grades for.
     */
    const fetchGrades = async (courseId) => {
        try {
            const response = await fetch(`${API_BASE_URL}/api/grades?courseId=${courseId}`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (response.ok) {
                const data = await response.json();
                setGrades(data);
            }
        } catch (error) {
            console.error('Error fetching grades:', error);
        }
    };

    /**
     * @function handleInputChange
     * @description Handles form input change and updates `formData`.
     * @param {Event} e - The input change event.
     */
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    /**
     * @function handleSubmit
     * @async
     * @description Submits new grade data to the API and refreshes the grade list.
     * @param {Event} e - The form submit event.
     */
    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const response = await fetch(API_BASE_URL + '/api/grades', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(formData),
            });

            if (response.ok) {
                resetForm();
                if (selectedCourse) fetchGrades(selectedCourse);
            } else {
                setError('Failed to save grade');
            }
        } catch (error) {
            console.error('Error saving grade:', error);
            setError('Failed to save grade');
        }
    };

    /**
     * @function deleteGrade
     * @description Deletes a grade by ID.
     * @param {string} id - The ID of the grade to delete.
     */
    const deleteGrade = async (id) => {
        try {
            const response = await fetch(`${API_BASE_URL}/api/grades/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok && selectedCourse) {
                fetchGrades(selectedCourse);
            } else {
                setError('Failed to delete grade');
            }
        } catch (error) {
            console.error('Error deleting grade:', error);
            setError('Failed to delete grade');
        }
    };

    /**
     * @function resetForm
     * @description Resets the grade form input values.
     */
    const resetForm = () => {
        setFormData({
            value: '',
            gradeType: '',
            feedback: '',
            studentId: '',
            courseId: selectedCourse
        });
    };

    return (
        <div className="grade-management">
            <h2>Grade Management</h2>
            {error && <p className="error">{error}</p>}

            <div className="course-selector">
                <label>Select Course:</label>
                <select
                    value={selectedCourse}
                    onChange={(e) => {
                        setSelectedCourse(e.target.value);
                        setFormData({ ...formData, courseId: e.target.value });
                    }}
                >
                    <option value="">-- Select Course --</option>
                    {courses.map(course => (
                        <option key={course.id} value={course.id}>{course.title}</option>
                    ))}
                </select>
            </div>

            {selectedCourse && (
                <>
                    <form onSubmit={handleSubmit} className="grade-form">
                        <h3>Add New Grade</h3>
                        <div>
                            <label>Student:</label>
                            <select
                                name="studentId"
                                value={formData.studentId}
                                onChange={handleInputChange}
                                required
                            >
                                <option value="">-- Select Student --</option>
                                {students.map(student => (
                                    <option key={student.id} value={student.id}>{student.name}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label>Grade Value:</label>
                            <input
                                type="text"
                                name="value"
                                value={formData.value}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div>
                            <label>Grade Type:</label>
                            <input
                                type="text"
                                name="gradeType"
                                value={formData.gradeType}
                                onChange={handleInputChange}
                                required
                            />
                        </div>
                        <div>
                            <label>Feedback:</label>
                            <textarea
                                name="feedback"
                                value={formData.feedback}
                                onChange={handleInputChange}
                            />
                        </div>
                        <button type="submit">Save Grade</button>
                    </form>

                    <div className="grades-list">
                        <h3>Grades for Selected Course</h3>
                        {grades.length === 0 ? (
                            <p>No grades available for this course</p>
                        ) : (
                            <table>
                                <thead>
                                    <tr>
                                        <th>Student</th>
                                        <th>Value</th>
                                        <th>Type</th>
                                        <th>Feedback</th>
                                        <th>Date</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {grades.map(grade => (
                                        <tr key={grade.gradeId}>
                                            <td>{grade.student?.user?.name || 'Unknown'}</td>
                                            <td>{grade.value}</td>
                                            <td>{grade.gradeType}</td>
                                            <td>{grade.feedback}</td>
                                            <td>{new Date(grade.dateAssigned).toLocaleDateString()}</td>
                                            <td>
                                                <button onClick={() => deleteGrade(grade.gradeId)}>Delete</button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        )}
                    </div>
                </>
            )}
        </div>
    );
}

export default GradeManagement;
