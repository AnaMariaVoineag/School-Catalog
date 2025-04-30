// components/StudentGrades.js
import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';

function StudentGrades() {
    const [grades, setGrades] = useState([]);
    const [error, setError] = useState('');

    const token = localStorage.getItem('token');

    useEffect(() => {
        fetchGrades();
    }, []);

    const fetchGrades = async () => {
        try {
            const response = await fetch(API_BASE_URL + '/api/grades', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setGrades(data);
            } else {
                setError('Failed to load grades');
            }
        } catch (error) {
            console.error('Error fetching grades:', error);
            setError('Failed to load grades');
        }
    };

    // Group grades by course
    const gradesByCourse = grades.reduce((acc, grade) => {
        const courseId = grade.course?.id || 'unknown';
        if (!acc[courseId]) {
            acc[courseId] = {
                courseName: grade.course?.title || 'Unknown Course',
                grades: []
            };
        }
        acc[courseId].grades.push(grade);
        return acc;
    }, {});

    return (
        <div className="student-grades">
            <h2>My Grades</h2>
            {error && <p className="error">{error}</p>}

            {Object.keys(gradesByCourse).length === 0 ? (
                <p>No grades available</p>
            ) : (
                Object.values(gradesByCourse).map((course, index) => (
                    <div key={index} className="course-grades">
                        <h3>{course.courseName}</h3>
                        <table>
                            <thead>
                                <tr>
                                    <th>Grade</th>
                                    <th>Type</th>
                                    <th>Feedback</th>
                                    <th>Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                {course.grades.map((grade, gradeIndex) => (
                                    <tr key={gradeIndex}>
                                        <td>{grade.value}</td>
                                        <td>{grade.gradeType}</td>
                                        <td>{grade.feedback}</td>
                                        <td>{new Date(grade.dateAssigned).toLocaleDateString()}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ))
            )}
        </div>
    );
}

export default StudentGrades;