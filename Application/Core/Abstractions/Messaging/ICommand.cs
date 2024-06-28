using MediatR;

namespace Application.Core.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
