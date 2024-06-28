using Application.Core.Abstractions.Common;
using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Notifications;
using Contracts.Emails;
using Domain.Core.Primitives.Result;
using Domain.Entities;
using Domain.Repositories;

namespace BackgroundTasks.Services;

internal sealed class EmailNotificationsConsumer : IEmailNotificationsConsumer
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public EmailNotificationsConsumer(
        INotificationRepository notificationRepository,
        IEmailNotificationService emailNotificationService,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _notificationRepository = notificationRepository;
        _emailNotificationService = emailNotificationService;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task ConsumeAsync(
              int batchSize,
              int allowedNotificationTimeDiscrepancyInMinutes,
              CancellationToken cancellationToken = default)
    {
        (Notification Notification, Event Event, User User)[] notificationsToBeSent =
            await _notificationRepository.GetNotificationsToBeSentIncludingUserAndEvent(
                batchSize,
                _dateTime.UtcNow,
                allowedNotificationTimeDiscrepancyInMinutes);

        var sendNotificationEmailTasks = new List<Task>();

        foreach ((Notification notification, Event @event, User user) in notificationsToBeSent)
        {
            Result result = notification.MarkAsSent();

            if (result.IsFailure)
            {
                continue;
            }

            _notificationRepository.Update(notification);

            (string subject, string body) = notification.CreateNotificationEmail(@event, user);

            var notificationEmail = new NotificationEmail(user.Email, subject, body);

            sendNotificationEmailTasks.Add(_emailNotificationService.SendNotificationEmail(notificationEmail));
        }

        await Task.WhenAll(sendNotificationEmailTasks);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
