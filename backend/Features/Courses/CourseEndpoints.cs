using backend.Data;
using Microsoft.EntityFrameworkCore;
using SchoolCatalogProject;

namespace SchoolCatalogProject.Features.Courses
{
    /// \class CourseEndpoints
    /// \brief Contains API endpoint mappings for course-related operations such as creating, updating, deleting, and retrieving courses.
    public static class CourseEndpoints
    {
        /// \brief Maps the course-related API endpoints to the IEndpointRouteBuilder instance.
        /// \param routes The IEndpointRouteBuilder instance to which the course endpoints are mapped.
        public static void MapCourseEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/courses");

            // Create Course
            /// \brief Endpoint for creating a new course.
            ///
            /// This endpoint allows the creation of a new course by adding it to the database.
            /// \param course The course details to be created.
            /// \return HTTP 201 Created with the created course if successful.
            group.MapPost("/", async (Course course, MariaDbContext db) =>
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                return Results.Created($"/courses/{course.Id}", course);
            });

            // Update Course
            /// \brief Endpoint for updating an existing course.
            ///
            /// This endpoint allows updating a course's details based on its ID.
            /// \param id The ID of the course to be updated.
            /// \param updatedCourse The updated course details.
            /// \return HTTP 200 OK with the updated course if successful, or HTTP 404 Not Found if the course does not exist.
            group.MapPut("/{id}", async (int id, Course updatedCourse, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                if (course == null) return Results.NotFound();

                course.Name = updatedCourse.Name;
                // update other fields if needed

                await db.SaveChangesAsync();
                return Results.Ok(course);
            });

            // Delete Course
            /// \brief Endpoint for deleting a course.
            ///
            /// This endpoint allows deleting a course based on its ID.
            /// \param id The ID of the course to be deleted.
            /// \return HTTP 204 No Content if successful, or HTTP 404 Not Found if the course does not exist.
            group.MapDelete("/{id}", async (int id, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                if (course == null) return Results.NotFound();

                db.Courses.Remove(course);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // Retrieve Course
            /// \brief Endpoint for retrieving a course by its ID.
            ///
            /// This endpoint allows fetching the details of a course based on its ID.
            /// \param id The ID of the course to be retrieved.
            /// \return HTTP 200 OK with the course details if successful, or HTTP 404 Not Found if the course does not exist.
            group.MapGet("/{id}", async (int id, MariaDbContext db) =>
            {
                var course = await db.Courses.FindAsync(id);
                return course is not null ? Results.Ok(course) : Results.NotFound();
            });
        }
    }
}
