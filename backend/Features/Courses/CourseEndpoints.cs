using Microsoft.EntityFrameworkCore;
using SchoolCatalogProject.Data;
using SchoolCatalogProject.Entities;

namespace SchoolCatalogProject.Features.Courses
{
    public static class CourseEndpoints
    {
        public static void MapCourseEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/courses");

            // Create Course
            group.MapPost("/", async (Course course, MariaDbContext db) =>
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return Results.Created($"/courses/{course.Id}", course);
            });

            // Update Course
            group.MapPut("/{id}", async (int id, Course updatedCourse, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                if (course == null) return Results.NotFound();

                course.Title = updatedCourse.Title;
                course.Description = updatedCourse.Description;
                // update other fields if needed

                await db.SaveChangesAsync();
                return Results.Ok(course);
            });

            // Delete Course
            group.MapDelete("/{id}", async (int id, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                if (course == null) return Results.NotFound();

                db.Courses.Remove(course);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // Retrieve Course
            group.MapGet("/{id}", async (int id, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                return course is not null ? Results.Ok(course) : Results.NotFound();
            });
        }
    }
}
