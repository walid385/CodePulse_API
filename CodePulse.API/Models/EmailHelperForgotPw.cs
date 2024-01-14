using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;

namespace Identity.Models
{
    public class EmailHelperForgotPw
    {
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("webps.info@gmail.com");
                mailMessage.To.Add(new MailAddress(userEmail));

                mailMessage.Subject = "Reset your Password";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Confirmation</title>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }}

                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                background-color: #ffffff;
                                border-radius: 5px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}

                            h1 {{
                                color: #333333;
                            }}

                            p {{
                                color: #666666;
                            }}

                            a {{
                                display: inline-block;
                                padding: 10px 20px;
                                background-color: #4CAF50;
                                color: #ffffff;
                                text-decoration: none;
                                border-radius: 3px;
                            }}

                            a:hover {{
                                background-color: #45a049;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h1>Reset Password</h1>
                            <p>Please reset your password by clicking the button below:</p>
                            <a href='{confirmationLink}' target='_blank'>Reset Password</a>
                            <p>If you didn't sign up for our service, you can ignore this email.</p>
                        </div>
                    </body>
                    </html>
                ";


                using (SmtpClient client = new SmtpClient())
                {
                    client.Credentials = new System.Net.NetworkCredential("webps.info@gmail.com", "uvpm ioua bhkz jrjh");
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(mailMessage);
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

    }
}