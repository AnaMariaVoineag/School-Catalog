namespace backend.Features.Auth
{
    public class AuthDto
    {
        public record RegistrationDto(string Name, string Email, string Password);
        public record LoginDto(string Email, string Password);
    }
}