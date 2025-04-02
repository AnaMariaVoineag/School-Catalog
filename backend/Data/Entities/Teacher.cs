using System.Collections.Generic;
using backend.Data.Entities;

namespace backend.Data.Entities
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!; // Null-forgiving operator

        public List<Course> Courses { get; set; } = new List<Course>();
    }
}