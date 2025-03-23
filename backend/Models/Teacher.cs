public class Teacher
{
    public int TeacherId { get; set; }
    
    // Foreign Key to User
    public int UserId { get; set; }
    public User User { get; set; }

    // Navigation Property for Courses
    public List<Course> Courses { get; set; } = new List<Course>();
}