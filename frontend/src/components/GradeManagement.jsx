// components/GradeManagement.js
import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';

function GradeManagement() {
    const [grades, setGrades] = useState([]);
    const [courses, setCourses] = useState([]);
    const [students, setStudents] = useState([]);
    const [formData, setFormData] = useState({
        value: '',
        gradeType: '',
        feedback: '',
        studentId: '',
        courseId: ''
    });
    const [selectedCourse, setSelectedCourse] = useState('');
    const [error, setError] = useState('');

    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchCourses();
        fetchStudents();
    }, []);

    useEffect(() => {
        if (selectedCourse) {
            fetchGrades(selectedCourse);
        }
    }, [selectedCourse]);

    const fetchCourses = async () => {
        try {
            const response = await fetch(API_BASE_URL + '/courses', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setCourses(data);
            }
        } catch (error) {
            console.error('Error fetching courses:', error);
        }
    };

    const fetchStudents = async () => {
        try {
            // This endpoint is not provided in your backend files
            // You would need to implement it or modify this approach
            const response = await fetch(API_BASE_URL + '/api/students', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setStudents(data);
            }
        } catch (error) {
            console.error('Error fetching students:', error);
        }
    };

    const fetchGrades = async (courseId) => {
        try {
            const response = await fetch(API_BASE_URL + `/api/grades?courseId=${courseId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setGrades(data);
            }
        } catch (error) {
            console.error('Error fetching grades:', error);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };

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
                if (selectedCourse) {
                    fetchGrades(selectedCourse);
                }
            } else {
                setError('Failed to save grade');
            }
        } catch (error) {
            console.error('Error saving grade:', error);
            setError('Failed to save grade');
        }
    };

    const deleteGrade = async (id) => {
        try {
            const response = await fetch(API_BASE_URL + `/api/grades/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                if (selectedCourse) {
                    fetchGrades(selectedCourse);
                }
            } else {
                setError('Failed to delete grade');
            }
        } catch (error) {
            console.error('Error deleting grade:', error);
            setError('Failed to delete grade');
        }
    };

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