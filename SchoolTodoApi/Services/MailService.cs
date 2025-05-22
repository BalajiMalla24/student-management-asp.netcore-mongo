using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

public class MailService
{
    public async Task SendEmail(string toEmail, string username, string message)
    {
        var apiKey = "";
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress("bijuvishnu442@gmail.com", "TodoApp");
        var to = new EmailAddress(toEmail, username);
        var subject = "Todo Created";
        var plainTextContent = message;
        var htmlContent = $"<p>{message.Replace("\n", "<br>")}</p>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            string errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid email failed. Status: {response.StatusCode}, Body: {errorBody}");
        }
    }
}
