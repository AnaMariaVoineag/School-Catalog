using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data.Entities
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Course name must be 3-100 characters")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester is required")]
        [RegularExpression("^(Fall|Spring|Summer|Winter) [0-9]{4}$",
            ErrorMessage = "Semester must be in format 'Season Year' (e.g., 'Fall 2023')")]
        public string Semester { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Course), "ValidateEndDate")]
        public DateTime EndDate { get; set; }

        // Foreign Key to Teacher
        [Required(ErrorMessage = "Teacher is required")]
        public int TeacherId { get; set; }

        // Navigation Properties
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;

        public List<Grade> Grades { get; set; } = new List<Grade>();
        public List<Student> EnrolledStudents { get; set; } = new List<Student>();

        // Custom validation method
        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = (Course)context.ObjectInstance;
            return endDate > instance.StartDate
                ? ValidationResult.Success
                : new ValidationResult("End date must be after start date");
        }
    }
}