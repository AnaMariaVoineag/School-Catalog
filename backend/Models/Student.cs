public class Student
{
    public int StudentId { get; set; }
    
    // Foreign Key to User
    public int UserId { get; set; }
    public User User { get; set; }

    // Many-to-Many with Course (handled via Fluent API)
    public List<Course> EnrolledCourses { get; set; } = new List<Course>();
}