namespace backend.Features.Users
{
    public class UserDto
    {
        public record UserUpdateDto(string? Name = null, string? Email = null);
    }
}
