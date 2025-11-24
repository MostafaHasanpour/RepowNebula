namespace RepowNebula.Domain.Users;

public class GitLabCredential
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string EncryptedAccessToken { get; set; }
    public string? GitLabUsername { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}