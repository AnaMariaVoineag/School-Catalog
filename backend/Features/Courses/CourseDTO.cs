namespace backend.Features.Courses;

public class CourseDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string Semester { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TeacherId { get; set; }
}

public class CreateCourseDto
{
    public string CourseName { get; set; }
    public string Semester { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TeacherId { get; set; }
}

public class UpdateCourseDto
{
    public string? CourseName { get; set; }
    public string? Semester { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? TeacherId { get; set; }
}