using RepowNebula.Domain.Groups;

namespace RepowNebula.Domain.Repositories;

public class Repository
{
    public Repository(string id, string name, string groupId, RepositoryGroup group, string url)
    {
        Id = id;
        Name = name;
        GroupId = groupId;
        Group = group;
        Url = url;
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string GroupId { get; private set; }

    public RepositoryGroup Group { get; private set; }

    public string Url { get; private set; }

    public static Repository Create(string id, string name, RepositoryGroup group, string url)
    {
        if (id is null) throw new ArgumentNullException(nameof(id));
        if (name is null) throw new ArgumentNullException(nameof(name));
        if (group is null) throw new ArgumentNullException(nameof(group));
        if (url is null) throw new ArgumentNullException(nameof(url));

        id = id.Trim();
        name = name.Trim();
        url = url.Trim();

        if (id.Length == 0) throw new ArgumentException("Id cannot be empty or whitespace.", nameof(id));
        if (name.Length == 0) throw new ArgumentException("Name cannot be empty or whitespace.", nameof(name));
        if (url.Length == 0) throw new ArgumentException("Url cannot be empty or whitespace.", nameof(url));

        return new Repository(id, name, group.Id, group, url);
    }
}
