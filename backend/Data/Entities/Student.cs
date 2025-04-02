public class Student
{
    public int StudentId { get; set; }
    public User User { get; set; }
    public List<Course> EnrolledCourses { get; set; } = [];
}