using System;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;

public class EmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromEmail = "selingbook@gmail.com";
        var fromPassword = "fsudhvspvdzlvhko";

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage(fromEmail, toEmail)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
    public async Task SendEmailAsyncObject(string toEmail, string subject, EmailResponse body)
    {
        var fromEmail = "selingbook@gmail.com";
        var fromPassword = "fsudhvspvdzlvhko";

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
            EnableSsl = true,
        };

        var bodyString = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.5; color: #333; margin: 0; padding: 0;'>
                        <table role='presentation' style='width: 100%; border: 0; border-spacing: 0;'>
                            <tr>
                                <td style='padding: 20px; background-color: #f4f7fc;'>
                                    <table role='presentation' style='width: 100%; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px;'>
                                        <tr>
                                            <td style='background-color: #007bff; padding: 20px; text-align: center; border-top-left-radius: 8px; border-top-right-radius: 8px;'>
                                                <h2 style='margin: 0; color: #ffffff;'>Exam Schedule</h2>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 20px; background-color: #ffffff;'>
                                                <p style='font-size: 16px; margin: 10px 0;'><strong>Exam:</strong> {body.examTitle}</p>
                                                <p style='font-size: 16px; margin: 10px 0;'><strong>Time:</strong> {body.selectedTime}</p>
                                                <p style='font-size: 16px; margin: 10px 0;'><strong>Date:</strong> {body.selectedDate}</p>
                                                <p style='font-size: 16px; margin: 10px 0;'><strong>Message:</strong> {body.comment}</p>
                                            </td>
                                        </tr>
                                        <tr>
                                         <td style='padding: 20px; background-color: #ffffff; text-align: center;'>
                                            <h3> Please use this account to log in. <h3>
                                            <p style='font-size: 16px; margin: 10px 0;'><strong>UserName:</strong> {body.UserName}</p>
                                            <p style='font-size: 16px; margin: 10px 0;'><strong>Password:</strong> {body.Password}</p>
                                         </td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 20px; background-color: #f4f7fc; text-align: center; border-bottom-left-radius: 8px; border-bottom-right-radius: 8px;'>
                                                <p style='margin: 0; font-size: 14px; color: #666;'>If you have any questions, feel free to reach out.</p>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </body>
                </html>";


        var mailMessage = new MailMessage(fromEmail, toEmail)
        {
            Subject = subject,
            Body = bodyString,
            IsBodyHtml = true,
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
