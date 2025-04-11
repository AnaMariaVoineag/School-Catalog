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
            // Save grade endpoint
            app.MapPost("/api/grades", [Authorize] async (
                [FromBody] GradeDto gradeDto,
                MariaDbContext db,
                ClaimsPrincipal claims) =>
            {
                var grade = new Grade
                {
                    Value = gradeDto.Value,
                    GradeType = gradeDto.GradeType,
                    Feedback = gradeDto.Feedback,
                    AssignedBy = gradeDto.AssignedBy,
                    StudentId = gradeDto.StudentId,
                    CourseId = gradeDto.CourseId,
                    DateAssigned = DateTime.UtcNow
                };

                db.Grades.Add(grade);
                await db.SaveChangesAsync();
                return Results.Created($"/api/grades/{grade.GradeId}", grade);
            }).Produces<Grade>(StatusCodes.Status201Created);

            // Retrieve grades endpoint
            app.MapGet("/api/grades", [Authorize] async (
                MariaDbContext db,
                [FromQuery] int? studentId,
                [FromQuery] int? courseId) =>
            {
                var query = db.Grades
                    .Include(g => g.Student)
                    .Include(g => g.Course)
                    .AsQueryable();

                if (studentId.HasValue)
                    query = query.Where(g => g.StudentId == studentId);

                if (courseId.HasValue)
                    query = query.Where(g => g.CourseId == courseId);

                var grades = await query.ToListAsync();
                return Results.Ok(grades);
            }).Produces<List<Grade>>(StatusCodes.Status200OK);

            return app;
        }
    }
}