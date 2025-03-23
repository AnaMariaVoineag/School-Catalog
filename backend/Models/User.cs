public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsActive { get; set; }

    // Navigation Properties
    public Teacher Teacher { get; set; }
    public Student Student { get; set; }
}