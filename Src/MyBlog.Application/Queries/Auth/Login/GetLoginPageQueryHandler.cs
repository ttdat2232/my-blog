using MediatR;
using MyBlog.Core.Aggregates.Clients;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Queries.Auth.Login;

public class GetLoginPageQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<GetLoginPageQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        GetLoginPageQuery request,
        CancellationToken cancellationToken
    )
    {
        var clientRepo = _unitOfWork.Repository<ClientAggregate, ClientId>();
        Serilog.Log.Information("fetching data of client {Id}", request.ClientId);
        if (!Guid.TryParse(request.ClientId.Trim(), out var clientId))
            return Result<bool>.Failure(new("Invalidate Client Id", 401));

        var client = await clientRepo.FindById(ClientId.From(clientId), cancellationToken);
        if (client == null)
            return Result<bool>.Failure(new("Client not found", 404));

        if (!client.RedirectUris.Contains(request.RedirectUri))
            return Result<bool>.Failure(new("Invalid redirect uri", 400));

        if (request.Scopes.Any(scope => !client.AllowScopes.Contains(scope)))
            return Result<bool>.Failure(new("Invalid scope(s)", 400));

        if (
            string.IsNullOrWhiteSpace(request.CodeChallenge)
            || string.IsNullOrWhiteSpace(request.ChallengeMethod)
        )
            return Result<bool>.Failure(new("Invalid code challenge or method", 400));

        return Result<bool>.Success(true);
    }
}
