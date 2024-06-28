using Api.Contracts;
using Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public sealed class FriendshipsController : ApiController
    {
        public FriendshipsController(IMediator mediator) : base(mediator)
        {
        }

    }
}
