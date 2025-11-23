namespace RepowNebula.Domain.Groups;

public class RepositoryGroup
{
    public string Id { get; private set; }

    public string Name { get; private set; }

    public string ParentGroupId { get; private set; }

    public RepositoryGroup ParentGroup { get; private set; }

    public string Url { get; private set; }
}
