using System.Collections.Generic;
using backend.Data.Entities;

namespace backend.Data.Entities
{
    public class Student
    {
        public int StudentId { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!; // Null-forgiving operator

        public List<Course> EnrolledCourses { get; set; } = new List<Course>();
    }
}