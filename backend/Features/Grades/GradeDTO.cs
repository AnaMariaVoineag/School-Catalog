namespace backend.Features.Grades
{
    public class GradeDto
    {
        public float Value { get; set; }
        public string GradeType { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}