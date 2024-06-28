namespace Infrastructure.Messaging.Settings
{
    public sealed class MessageBrokerSettings
    {
        public const string SettingsKey = "MessageBroker";

        public string HostName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string QueueName { get; set; }
    }
}
