// components/CourseManagement.js
import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';

function CourseManagement() {
    const [courses, setCourses] = useState([]);
    const [name, setName] = useState('');
    const [semester, setSemester] = useState('');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');
    const [editingCourse, setEditingCourse] = useState(null);
    const [error, setError] = useState('');

    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchCourses();
    }, []);

    const fetchCourses = async () => {
        try {
            const response = await fetch(API_BASE_URL + '/api/courses', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setCourses(data);
            } else {
                setError('Failed to load courses');
            }
        } catch (error) {
            console.error('Error fetching courses:', error);
            setError('Failed to load courses');
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            let url = '/api/courses';
            let method = 'POST';
            let body = {
                name,
                semester,
                startDate,
                endDate
            };

            if (editingCourse) {
                url = `/api/courses/${editingCourse.id}`;
                method = 'PUT';
            }

            const response = await fetch(API_BASE_URL + url, {
                method,
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(body),
            });

            if (response.ok) {
                await fetchCourses();
                resetForm();
            } else {
                setError('Failed to save course');
            }
        } catch (error) {
            console.error('Error saving course:', error);
            setError('Failed to save course');
        }
    };

    const deleteCourse = async (id) => {
        try {
            const response = await fetch(API_BASE_URL + `/api/courses/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                await fetchCourses();
            } else {
                setError('Failed to delete course');
            }
        } catch (error) {
            console.error('Error deleting course:', error);
            setError('Failed to delete course');
        }
    };

    const editCourse = (course) => {
        setEditingCourse(course);
        setName(course.name);
        setSemester(course.semester);
        setStartDate(course.startDate ? course.startDate.substring(0, 10) : '');
        setEndDate(course.endDate ? course.endDate.substring(0, 10) : '');
    };

    const resetForm = () => {
        setName('');
        setSemester('');
        setStartDate('');
        setEndDate('');
        setEditingCourse(null);
    };

    return (
        <div className="course-management">
            <h2>Course Management</h2>
            {error && <p className="error">{error}</p>}

            <form onSubmit={handleSubmit} className="course-form">
                <h3>{editingCourse ? 'Edit Course' : 'Add New Course'}</h3>
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
                    <label>Semester:</label>
                    <input
                        type="text"
                        value={semester}
                        onChange={(e) => setSemester(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Start Date:</label>
                    <input
                        type="date"
                        value={startDate}
                        onChange={(e) => setStartDate(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>End Date:</label>
                    <input
                        type="date"
                        value={endDate}
                        onChange={(e) => setEndDate(e.target.value)}
                        required
                    />
                </div>
                <div className="form-buttons">
                    <button type="submit">{editingCourse ? 'Update' : 'Create'}</button>
                    {editingCourse && <button type="button" onClick={resetForm}>Cancel</button>}
                </div>
            </form>

            <div className="courses-list">
                <h3>Courses</h3>
                {courses.length === 0 ? (
                    <p>No courses available</p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Semester</th>
                                <th>Start Date</th>
                                <th>End Date</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {courses.map(course => (
                                <tr key={course.id}>
                                    <td>{course.name}</td>
                                    <td>{course.semester}</td>
                                    <td>{course.startDate ? new Date(course.startDate).toLocaleDateString() : ''}</td>
                                    <td>{course.endDate ? new Date(course.endDate).toLocaleDateString() : ''}</td>
                                    <td>
                                        <button onClick={() => editCourse(course)}>Edit</button>
                                        <button onClick={() => deleteCourse(course.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}

export default CourseManagement;
