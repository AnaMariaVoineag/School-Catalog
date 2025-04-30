namespace backend.Features.Grades
{
    /// \class GradeDto
    /// \brief Represents the data transfer object for grade information.
    ///
    /// This DTO contains the details of a grade including its value, type, feedback, and the associated student and course.
    public class GradeDto
    {
        /// \brief The value of the grade.
        /// \return The numeric value of the grade.
        public float Value { get; set; }

        /// \brief The type of the grade (e.g., letter grade, percentage).
        /// \return A string representing the type of grade.
        public string GradeType { get; set; } = string.Empty;

        /// \brief Feedback provided for the grade.
        /// \return A string containing feedback associated with the grade.
        public string Feedback { get; set; } = string.Empty;

        /// \brief The ID of the student associated with this grade.
        /// \return The student ID.
        public int StudentId { get; set; }

        /// \brief The ID of the course associated with this grade.
        /// \return The course ID.
        public int CourseId { get; set; }
    }
}
