using MediatR;
using MyBlog.Core.Aggregates.Clients;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Commands.Auth.RegisterClient;

public class RegisterClientCommandHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<RegisterClientCommand, IResult>
{
    public async Task<IResult> Handle(
        RegisterClientCommand request,
        CancellationToken cancellationToken
    )
    {
        var clientRepo = _unitOfWork.Repository<ClientAggregate, ClientId>();
        var client = ClientAggregate.Create(
            Guid.NewGuid().ToString().Replace("-", ""),
            request.RedirectUris,
            request.AllowScopes
        );

        await clientRepo.AddAsync(client, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return Result<string>.Success("Client registered");
    }
}
