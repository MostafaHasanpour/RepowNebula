using Microsoft.AspNetCore.Identity;

using System.Collections.ObjectModel;

namespace RepowNebula.Domain.Users;

public class ApplicationUser : IdentityUser
{
    // Profile & domain fields
    public string FullName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Auditing
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastSeenAt { get; private set; }

    // Backing collection for credentials to enforce invariants and encapsulation
    private readonly List<GitLabCredential> _gitLabCredentials = new();
    public IReadOnlyCollection<GitLabCredential> GitLabCredentials => new ReadOnlyCollection<GitLabCredential>(_gitLabCredentials);

    // Parameterless ctor for EF / serialization - made private for factory pattern
    private ApplicationUser()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    // Private ctor used by factory method
    private ApplicationUser(string userName, string email, string fullName) : base(userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName is required.", nameof(userName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("FullName is required.", nameof(fullName));

        UserName = userName;
        Email = email;
        FullName = fullName;
        DisplayName = fullName;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        SecurityStamp = Guid.NewGuid().ToString();
    }

    // Factory method for creating ApplicationUser instances
    public static ApplicationUser Create(string userName, string email, string fullName)
    {
        return new ApplicationUser(userName, email, fullName);
    }

    // PROFILE OPERATIONS

    public void UpdateProfile(string fullName, string? displayName = null, string? bio = null)
    {
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("FullName is required.", nameof(fullName));

        FullName = fullName;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? fullName : displayName!;
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatar(string? avatarUrl)
    {
        AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearAvatar()
    {
        AvatarUrl = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void TouchLastSeen()
    {
        LastSeenAt = DateTime.UtcNow;
    }

    // GITLAB CREDENTIALS MANAGEMENT

    public GitLabCredential AddGitLabCredential(string name, string encryptedAccessToken, string? gitLabUsername = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Credential name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(encryptedAccessToken)) throw new ArgumentException("Encrypted access token is required.", nameof(encryptedAccessToken));

        if (_gitLabCredentials.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A GitLab credential with the name '{name}' already exists for this user.");

        var credential = new GitLabCredential
        {
            Name = name,
            EncryptedAccessToken = encryptedAccessToken,
            GitLabUsername = gitLabUsername,
            ApplicationUser = this,
            ApplicationUserId = Id ?? string.Empty
        };

        _gitLabCredentials.Add(credential);
        UpdatedAt = DateTime.UtcNow;
        return credential;
    }

    public bool RemoveGitLabCredential(int id)
    {
        var credential = _gitLabCredentials.FirstOrDefault(c => c.Id == id);
        if (credential == null) return false;

        _gitLabCredentials.Remove(credential);
        // Clear back-reference if the credential instance is tracked further
        credential.ApplicationUser = null!;
        credential.ApplicationUserId = string.Empty;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool RemoveGitLabCredentialByName(string name)
    {
        var credential = _gitLabCredentials.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (credential == null) return false;

        _gitLabCredentials.Remove(credential);
        credential.ApplicationUser = null!;
        credential.ApplicationUserId = string.Empty;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public GitLabCredential? GetGitLabCredentialById(int id)
    {
        return _gitLabCredentials.FirstOrDefault(c => c.Id == id);
    }

    public void UpdateGitLabCredentialToken(int id, string newEncryptedToken)
    {
        if (string.IsNullOrWhiteSpace(newEncryptedToken)) throw new ArgumentException("Encrypted token is required.", nameof(newEncryptedToken));
        var credential = _gitLabCredentials.FirstOrDefault(c => c.Id == id) ?? throw new InvalidOperationException("Credential not found.");
        credential.EncryptedAccessToken = newEncryptedToken;
        UpdatedAt = DateTime.UtcNow;
    }

    // Utility / convenience

    public override string ToString()
    {
        return $"{DisplayName} ({Id ?? UserName})";
    }
}
