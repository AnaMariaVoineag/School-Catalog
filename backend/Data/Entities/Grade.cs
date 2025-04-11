public class Grade
{
    public int GradeId { get; set; }
    public float Value { get; set; }
    public DateTime DateAssigned { get; set; }
    public string GradeType { get; set; }
    public string Feedback { get; set; }
    public string AssignedBy { get; set; }

    // Foreign Keys
    public Student Student { get; set; }
    public Course Course { get; set; }
}