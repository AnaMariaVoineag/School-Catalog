namespace backend.Features.Auth
{
    /// \class AuthDto
    /// \brief Contains data transfer objects (DTOs) for authentication-related operations, including registration and login.
    ///
    /// This class holds the data structures used for user registration and login requests.
    public class AuthDto
    {
        /// \brief Data transfer object for user registration.
        ///
        /// This DTO is used for user registration requests, including the user's name, email, password, and role.
        public record RegistrationDto(string Name, string Email, string Password, string Role);

        /// \brief Data transfer object for user login.
        ///
        /// This DTO is used for user login requests, which include the user's email and password.
        public record LoginDto(string Email, string Password);
    }
}
