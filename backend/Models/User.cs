public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }

    public void Login() {}
    public void Logout() {}
    public void ResetPassword() {}
    public void UpdateProfile() {}
}