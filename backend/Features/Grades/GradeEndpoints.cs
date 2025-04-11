using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Features.Grades
{
    public static class GradeEndpoints
    {
        public static WebApplication MapGradeEndpoints(this WebApplication app)
        {
            // Save grade (Teacher only)
            app.MapPost("/api/grades", [Authorize(Roles = "teacher")] async (
                [FromBody] GradeDto gradeDto,
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var userId = int.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var teacher = await db.Teachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.User.ID == userId);

                if (teacher == null) return Results.Forbid();

                var grade = new Grade
                {
                    Value = gradeDto.Value,
                    GradeType = gradeDto.GradeType,
                    Feedback = gradeDto.Feedback,
                    StudentId = gradeDto.StudentId,
                    CourseId = gradeDto.CourseId,
                    DateAssigned = DateTime.UtcNow,
                    AssignedBy = teacher.TeacherId.ToString()
                };

                db.Grades.Add(grade);
                await db.SaveChangesAsync();
                return Results.Created($"/api/grades/{grade.GradeId}", grade);
            });

            // Get grades (Student sees own, Teacher sees all)
            app.MapGet("/api/grades", [Authorize] async (
                MariaDbContext db,
                [FromQuery] int? courseId,
                ClaimsPrincipal claims) =>
            {
                var userId = int.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = claims.FindFirst(ClaimTypes.Role)?.Value;

                var query = db.Grades
                    .Include(g => g.Student)
                        .ThenInclude(s => s.User)
                    .Include(g => g.Course)
                    .AsQueryable();

                if (role == "student")
                {
                    var student = await db.Students.FirstOrDefaultAsync(s => s.User.ID == userId);
                    if (student == null) return Results.Forbid();
                    query = query.Where(g => g.StudentId == student.StudentId);
                }
                else if (role == "teacher")
                {
                    if (courseId.HasValue)
                        query = query.Where(g => g.CourseId == courseId);
                }

                return Results.Ok(await query.ToListAsync());
            });

            // Delete grade (Teacher only)
            app.MapDelete("/api/grades/{id:int}", [Authorize(Roles = "teacher")] async (
                int id,
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var userId = int.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.User.ID == userId);
                var grade = await db.Grades.FindAsync(id);

                if (grade == null) return Results.NotFound();
                if (teacher == null || grade.AssignedBy != teacher.TeacherId.ToString())
                    return Results.Forbid();

                db.Grades.Remove(grade);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            return app;
        }
    }
}