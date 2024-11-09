# Event Reminder

## Process Flow

When an invitation is accepted, several events and handlers work together to update the system's state, create new entities, and notify relevant parties.

#### 1. Adding the Domain Event

Within the domain layer, when an invitation is accepted, the entity adds a new domain event by invoking a method defined in the `AggregateRoot` class:

```csharp
AddDomainEvent(new InvitationAcceptedDomainEvent(this));
```

#### 2. This action triggers all handlers associated with the `InvitationAcceptedDomainEvent`:

##### **CreateAttendeeOnInvitationAcceptedDomainEventHandler**

- **Purpose**: Creates a new attendee based on the accepted invitation and publish related event.
- **Process**:

```csharp
        public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
        {
            var attendee = new Attendee(notification.Invitation);

            _attendeeRepository.Insert(attendee);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new AttendeeCreatedEvent(attendee.Id), cancellationToken);
        }
```
##### **PublishIntegrationEventOnInvitationAcceptedDomainEventHandler**

- **Purpose**: Creates a new integration event based on the accepted invitation and publish it.
- **Process**:

```csharp
        public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
        {
            _integrationEventPublisher.Publish(new InvitationAcceptedIntegrationEvent(notification));

            await Task.CompletedTask;
        }
```
Once the `InvitationAcceptedIntegrationEvent` is published, it is handled by the following integration event handler:

##### **NotifyEventOwnerOnInvitationAcceptedIntegrationEventHandler**
- **Purpose**: Notifies the event owner that his invitation has been accepted.
- **Process**:

```csharp
        public async Task Handle(InvitationAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
        {
          ...
             await _emailNotificationService.SendInvitationAcceptedEmail(invitationAcceptedEmail);
        }
```

