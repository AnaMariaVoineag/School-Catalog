using backend.Data;
using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Courses;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");

        // Create course
        group.MapPost("/", async ([FromBody] CreateCourseDto dto, MariaDbContext db) =>
        {
            var course = new Course
            {
                CourseName = dto.CourseName,
                Semester = dto.Semester,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Teacher = await db.Teachers.FindAsync(dto.TeacherId)
            };

            if (course.Teacher == null)
                return Results.BadRequest("Teacher not found");

            db.Courses.Add(course);
            await db.SaveChangesAsync();

            var result = new CourseDto
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Semester = course.Semester,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                TeacherId = course.Teacher.TeacherId
            };

            return Results.Created($"/api/courses/{course.CourseId}", result);
        });

        // Get all courses
        group.MapGet("/", async (MariaDbContext db) =>
        {
            var courses = await db.Courses
                .Include(c => c.Teacher)
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Semester = c.Semester,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    TeacherId = c.Teacher.TeacherId
                })
                .ToListAsync();

            return Results.Ok(courses);
        });

        // Get single course
        group.MapGet("/{id}", async (int id, MariaDbContext db) =>
        {
            var course = await db.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return Results.NotFound();

            var result = new CourseDto
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                Semester = course.Semester,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                TeacherId = course.Teacher.TeacherId
            };

            return Results.Ok(result);
        });

        // Update course
        group.MapPut("/{id}", async (int id, [FromBody] UpdateCourseDto dto, MariaDbContext db) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course == null)
                return Results.NotFound();

            if (dto.CourseName != null) course.CourseName = dto.CourseName;
            if (dto.Semester != null) course.Semester = dto.Semester;
            if (dto.StartDate != null) course.StartDate = dto.StartDate.Value;
            if (dto.EndDate != null) course.EndDate = dto.EndDate.Value;

            if (dto.TeacherId != null)
            {
                var teacher = await db.Teachers.FindAsync(dto.TeacherId.Value);
                if (teacher == null)
                    return Results.BadRequest("Teacher not found");
                course.Teacher = teacher;
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Delete course
        group.MapDelete("/{id}", async (int id, MariaDbContext db) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course == null)
                return Results.NotFound();

            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}