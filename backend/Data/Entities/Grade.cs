public class Grade
{
    public int GradeId { get; set; }
    public float Value { get; set; }
    public DateTime DateAssigned { get; set; }
    public string GradeType { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;

    // Foreign Keys
    public int StudentId { get; set; }
    public int CourseId { get; set; }

    // Navigation Properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}