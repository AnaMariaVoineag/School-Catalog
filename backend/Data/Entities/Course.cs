public class Course
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string Semester { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Foreign Key to Teacher
    public Teacher Teacher { get; set; }

    // Navigation Properties
    public List<Grade> Grades { get; set; } = new List<Grade>();
    public List<Student> EnrolledStudents { get; set; } = new List<Student>();
}