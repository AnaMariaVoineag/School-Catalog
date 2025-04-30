import React, { useState, useEffect } from 'react';
import { API_BASE_URL } from '../config';

/**
 * @file StudentGrades.js
 * @brief Component to display a student's grades grouped by course.
 */

/**
 * @component
 * @name StudentGrades
 * @description 
 * Fetches and displays the student's grades, grouped by course. 
 * Handles loading errors and organizes the data in a table format per course.
 * 
 * @returns {JSX.Element} Rendered component displaying student grades.
 */
function StudentGrades() {
    /// State to store fetched grades
    const [grades, setGrades] = useState([]);

    /// State to store any error message from the fetch operation
    const [error, setError] = useState('');

    /// Authentication token retrieved from local storage
    const token = localStorage.getItem('token');

    /**
     * @function useEffect
     * @description Triggers the initial grade fetch on component mount.
     */
    useEffect(() => {
        fetchGrades();
    }, []);

    /**
     * @function fetchGrades
     * @async
     * @description 
     * Asynchronously fetches grades from the API and updates component state.
     * Sets an error message if the request fails.
     */
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

    /**
     * @var gradesByCourse
     * @description 
     * Groups grades by the associated course ID. Each entry contains
     * course information and the list of related grades.
     */
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

    // Render the grouped grades or an error/no data message
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
