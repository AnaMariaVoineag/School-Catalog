public class Teacher
{
    public int TeacherId { get; set; }

    public User User { get; set; } = null!;

    public List<Course> Courses { get; set; } = [];
}
