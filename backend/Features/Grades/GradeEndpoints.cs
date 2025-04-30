using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Features.Grades
{
    /// \class GradeEndpoints
    /// \brief Contains API endpoint mappings for grade-related operations such as saving, retrieving, and deleting grades.
    public static class GradeEndpoints
    {
        /// \brief Maps the grade-related API endpoints to the WebApplication instance.
        /// \param app The WebApplication instance to which the endpoints are mapped.
        /// \return The WebApplication instance with the grade-related endpoints mapped.
        public static WebApplication MapGradeEndpoints(this WebApplication app)
        {
            // Save grade (Teacher only)
            /// \brief Endpoint for saving a grade for a student (restricted to teachers).
            /// 
            /// This endpoint allows teachers to assign grades to students. It requires the user to have a "teacher" role.
            /// \param gradeDto The grade details to be assigned.
            /// \return HTTP 201 Created with the created grade if successful, or HTTP 403 Forbidden if the user is not a teacher.
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
            /// \brief Endpoint for retrieving grades.
            ///
            /// This endpoint returns grades based on the user's role. Students can only see their own grades, 
            /// while teachers can view grades for all students within a specified course (optional).
            /// \param courseId The optional course ID to filter grades by course (only for teachers).
            /// \return HTTP 200 OK with the list of grades, or HTTP 403 Forbidden if the user is not authorized.
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
            /// \brief Endpoint for deleting a grade (restricted to teachers).
            ///
            /// This endpoint allows teachers to delete grades they have assigned. It ensures the user is the teacher who assigned the grade.
            /// \param id The ID of the grade to be deleted.
            /// \return HTTP 204 No Content if successful, or HTTP 403 Forbidden or HTTP 404 Not Found if unauthorized or grade doesn't exist.
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
