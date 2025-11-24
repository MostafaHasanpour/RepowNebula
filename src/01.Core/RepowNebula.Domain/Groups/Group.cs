using System;

namespace RepowNebula.Domain.Groups;

public class RepositoryGroup
{
    // Parameterless ctor for EF / serialization
    private RepositoryGroup()
    {
        Id = string.Empty;
        Name = string.Empty;
        ParentGroupId = null;
        ParentGroup = null;
        Url = null;
    }

    // Primary ctor - ensures required invariants at creation time
    private RepositoryGroup(string id, string name, string? parentGroupId = null, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be empty.", nameof(name));

        Id = id;
        Name = name.Trim();
        ParentGroupId = string.IsNullOrWhiteSpace(parentGroupId) ? null : parentGroupId;
        ParentGroup = null;
        Url = NormalizeUrl(url);
    }

    // Factory method
    public static RepositoryGroup Create(string id, string name, string? parentGroupId = null, string? url = null)
    {
        return new RepositoryGroup(id, name, parentGroupId, url);
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string? ParentGroupId { get; private set; }

    public RepositoryGroup? ParentGroup { get; private set; }

    public string? Url { get; private set; }

    // Behaviors

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("New name must not be empty.", nameof(newName));

        var trimmed = newName.Trim();
        if (Name == trimmed)
            return; // no-op if unchanged

        Name = trimmed;
    }

    public void SetParent(string parentGroupId)
    {
        if (string.IsNullOrWhiteSpace(parentGroupId))
            throw new ArgumentException("Parent group id must not be empty.", nameof(parentGroupId));

        if (parentGroupId == Id)
            throw new InvalidOperationException("A group cannot be parent of itself.");

        ParentGroupId = parentGroupId;
        ParentGroup = null; // clear navigation; repository / loader can rehydrate if needed
    }

    public void ClearParent()
    {
        ParentGroupId = null;
        ParentGroup = null;
    }

    public void UpdateUrl(string url)
    {
        Url = NormalizeUrl(url);
    }

    // Helpers

    private static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var trimmed = url.Trim();

        // Accept absolute or relative URIs; validate basic well-formedness
        if (Uri.IsWellFormedUriString(trimmed, UriKind.Absolute) ||
            Uri.IsWellFormedUriString(trimmed, UriKind.Relative))
        {
            return trimmed;
        }

        throw new ArgumentException("The provided url is not a valid URI.", nameof(url));
    }

    public override string ToString() => $"RepositoryGroup {{ Id = {Id}, Name = {Name} }}";

    public override bool Equals(object obj)
    {
        if (obj is not RepositoryGroup other)
            return false;
        return string.Equals(Id, other.Id, StringComparison.Ordinal);
    }

    public override int GetHashCode() => Id?.GetHashCode(StringComparison.Ordinal) ?? 0;
}
