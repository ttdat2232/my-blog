using Carter;

namespace MyBlog.Auth.Endpoints;

public class TokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/token");
        group.MapGet("", () => "token");
    }
}
