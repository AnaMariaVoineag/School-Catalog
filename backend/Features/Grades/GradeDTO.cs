namespace backend.Features.Grades
{
    public class GradeDto
    {
        public float Value { get; set; }
        public string GradeType { get; set; }
        public string Feedback { get; set; }
        public string AssignedBy { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}