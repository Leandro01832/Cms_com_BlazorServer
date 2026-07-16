using business.business.sistema;
using Microsoft.AspNetCore.Identity;


public class EmailSender : IEmailSender<UserModel>
{
    public Task SendConfirmationLinkAsync(UserModel user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendEmailAsync(UserModel user, string subject, string message)
    {
        // lógica para enviar e-mail
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(UserModel user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(UserModel user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}
