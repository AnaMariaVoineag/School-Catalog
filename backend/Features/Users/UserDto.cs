/// \namespace backend.Features.Users
/// \brief Contains classes related to user features, including data transfer objects.
namespace backend.Features.Users
{
    /// \class UserDto
    /// \brief Container class for user-related data transfer objects.
    public class UserDto
    {
        /// \record UserUpdateDto
        /// \brief DTO used for updating user information.
        /// 
        /// This record allows optional updates to a user's name and email.
        /// \param Name Optional new name for the user.
        /// \param Email Optional new email address for the user.
        public record UserUpdateDto(string? Name = null, string? Email = null);
    }
}