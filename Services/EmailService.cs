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
                                                <p style='font-size: 16px; margin: 10px 0;'>Hello {body.name}</p>
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
        
                                            <br/>
                                            <p>Click link: <a href='http://46.202.178.139:5173/candidate-login'> Login Exam </a> </p>
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
  public async Task SendEmailResult(string toEmail, string name, string subject, bool testPassed, int newPoint)
{
    var fromEmail = "selingbook@gmail.com";
    var fromPassword = "fsudhvspvdzlvhko";

    var smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential(fromEmail, fromPassword),
        EnableSsl = true,
    };

    var cssStyles = @"
    <style>
        body { font-family: 'Arial', sans-serif; background-color: #f4f4f9; color: #333; margin: 0; padding: 0; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); }
        .header { background-color: #4CAF50; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }
        .content { margin-top: 20px; padding: 20px; font-size: 16px; line-height: 1.6; }
        .footer { margin-top: 30px; text-align: center; font-size: 12px; color: #777; padding: 10px 0; }
        .btn { display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #4CAF50; border-radius: 5px; text-decoration: none; }
        .btn:hover { background-color: #45a049; }
        .result { font-weight: bold; font-size: 18px; color: #333; }
        .passed { color: green; }
        .failed { color: red; }
    </style>";

    string resultMessage = testPassed ? "<span class='result passed'>You passed the test!</span>" : "<span class='result failed'>You failed the test.</span>";

    var emailBody = $@"
    <html>
        <head>{cssStyles}</head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Exam Results Announcement</h2>
                </div>
                <div class='content'>
                    <p>Dear {name},</p>
                    <p>Your exam result is: {newPoint}</p>
                    <p>{resultMessage}</p>
                    <p>Thank you for taking the test. We encourage you to continue improving your skills!</p>
                </div>
                <div class='footer'>
                    <p>Sent from SelingBook</p>
                    <p>&copy; {DateTime.Now.Year} SelingBook. All rights reserved.</p>
                </div>
            </div>
        </body>
    </html>";

    var mailMessage = new MailMessage(fromEmail, toEmail)
    {
        Subject = subject,
        Body = emailBody,
        IsBodyHtml = true,
    };

    await smtpClient.SendMailAsync(mailMessage);
}

public async Task SendInterviewSchedule(string name, string toEmail, string subject, EmailRequest2 emailRequest2)
{
    var fromEmail = "selingbook@gmail.com";
    var fromPassword = "fsudhvspvdzlvhko";

    var smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential(fromEmail, fromPassword),
        EnableSsl = true,
    };

    var cssStyles = @"
    <style>
        body { font-family: 'Arial', sans-serif; background-color: #f4f4f9; color: #333; margin: 0; padding: 0; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); }
        .header { background-color: #4CAF50; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center; }
        .content { margin-top: 20px; padding: 20px; font-size: 16px; line-height: 1.6; }
        .footer { margin-top: 30px; text-align: center; font-size: 12px; color: #777; padding: 10px 0; }
        .btn { display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #4CAF50; border-radius: 5px; text-decoration: none; }
        .btn:hover { background-color: #45a049; }
    </style>";


    var emailBody = $@"
    <html>
        <head>{cssStyles}</head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>Interview Schedule</h2>
                </div>
                <div class='content'>
                    <p>Dear {name},</p>
                    <p>We are pleased to inform you that your interview has been scheduled.</p>
                    <p><strong>Interview Date:</strong> {emailRequest2.Date} <strong> Time:</strong> {emailRequest2.Time}</p>
                    <p><strong>Interview Location:</strong> 8a Ton That Thuyet, Nam Tu Liem, HN</p>
                    <p>Please make sure to arrive at least 15 minutes early and bring any necessary documents with you.</p>
                    <p>We look forward to meeting you!</p>
                </div>
                <div class='footer'>
                    <p>Sent from SelingBook</p>
                    <p>&copy; {DateTime.Now.Year} SelingBook. All rights reserved.</p>
                </div>
            </div>
        </body>
    </html>";

    var mailMessage = new MailMessage(fromEmail, toEmail)
    {
        Subject = subject,
        Body = emailBody,
        IsBodyHtml = true,
    };

    await smtpClient.SendMailAsync(mailMessage);
}


}
