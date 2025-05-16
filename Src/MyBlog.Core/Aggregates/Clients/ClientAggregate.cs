using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Clients;

public sealed class ClientAggregate : AggregateRoot<ClientId>
{
    public string ClientSecret { get; set; }
    public IReadOnlyCollection<string> RedirectUris => _redirectUris.AsReadOnly();
    public IReadOnlyCollection<string> AllowScopes => _allowScopes.AsReadOnly();

    //TODO: convert this into separate entity
    private readonly List<string> _redirectUris;

    //TODO: convert this into separate entity
    private readonly List<string> _allowScopes;

    public static ClientAggregate Create(
        string clientSecret,
        List<string> redirectUris,
        List<string> allowScopes
    ) => new(ClientId.New(), clientSecret, redirectUris, allowScopes);

    private ClientAggregate(
        ClientId id,
        string clientSecret,
        List<string> redirectUris,
        List<string> allowScopes
    )
        : base(id)
    {
        ClientSecret = clientSecret;
        _redirectUris = redirectUris;
        _allowScopes = allowScopes;
    }

    // EF Core require
#pragma warning disable CS8618
    private ClientAggregate()
#pragma warning restore CS8618
        : base(ClientId.New()) { }
}
