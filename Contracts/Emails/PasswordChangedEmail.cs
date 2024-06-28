namespace Contracts.Emails;

public sealed class PasswordChangedEmail
{
    public PasswordChangedEmail(string emailTo, string name)
    {
        EmailTo = emailTo;
        Name = name;
    }

    public string EmailTo { get; }

    public string Name { get; }
}
