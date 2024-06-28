using MediatR;

namespace Application.Core.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
