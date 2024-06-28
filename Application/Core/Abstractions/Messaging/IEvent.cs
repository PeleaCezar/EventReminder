﻿using MediatR;

namespace Application.Core.Abstractions.Messaging;

/// <summary>
/// Represents the event interface.
/// </summary>
public interface IEvent : INotification
{
}
